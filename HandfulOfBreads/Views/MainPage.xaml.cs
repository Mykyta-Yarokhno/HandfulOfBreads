using CommunityToolkit.Maui.Views;
using HandfulOfBreads.ViewModels;
using HandfulOfBreads.Views.Popups;

namespace HandfulOfBreads.Views
{
    public partial class MainPage : ContentPage
    {
        #region Basic
        private readonly MainPageViewModel _viewModel;
        private const int _pixelSize = 40;
        private double scale;
        private double minScale;
        private int _columns;
        private int _rows;

        public MainPage(int columns, int rows, string selectedPattern, List<List<Color>>? grid = null)
        {
            InitializeComponent();

            Shell.SetNavBarIsVisible(this, false);

            _columns = columns;
            _rows = rows;

            _viewModel = App.MainViewModel;

            _viewModel.Initialize(columns, rows, selectedPattern, grid);
            //_viewModel = new MainPageViewModel(columns, rows, selectedPattern, grid);
            BindingContext = _viewModel;
            
            _viewModel.InvalidateRequested += OnInvalidateRequested;

            PixelGraphicsView.WidthRequest = columns * _pixelSize;
            PixelGraphicsView.HeightRequest = selectedPattern == "Brick"
                ? rows * _pixelSize + 20
                : rows * _pixelSize;

            PixelGraphicsViewContainer.WidthRequest = columns * _pixelSize * 10;
            PixelGraphicsViewContainer.HeightRequest = rows * _pixelSize * 10;

            _viewModel.RequestInvalidate += () =>
            {
                PixelGraphicsView.Invalidate();
            };

            var paletteView = ColorPaletteViewCache.GetPaletteView("Preciosa Rocialles");

            if (paletteView != null)
                PaletteScrollView.Content = paletteView;

            //OnAppearing();
        }


        //protected override async void OnAppearing()
        //{
        //    base.OnAppearing();

        //    await _viewModel.LoadPaletteAsync("Preciosa Rocialles");
        //}
        private void OnInvalidateRequested()
        {
            PixelGraphicsView.Invalidate();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            minScale = Math.Min(width / PixelGraphicsView.Width, height / PixelGraphicsView.Height);

            //scale = minScale;
            scale = 0.5;

            //PixelGraphicsViewContainer.Scale = minScale;
            PixelGraphicsViewContainer.Scale = scale;
        }
        #endregion

        #region Canvas Interaction

        //move, scale, paint
        private PointF? _previousTouchPoint = null;
        private PointF _onePoint;
        private Point _startPosition1, _startPosition2;

        private bool _isDrawing;
        private bool _isDragging;
        

        private float _initialDistance;
        private double _initialScale;

        //selection
        private (int Row, int Col)? _selectionStartCell;
        private (int Row, int Col)? _selectionEndCell;
        private bool _isSelecting = false;
        private bool _isMovingPastePreview;

        private void OnStartInteraction(object sender, TouchEventArgs e)
        {
            if (e.Touches.Count() == 1)
            {
                if (_isSelecting)
                {
                    var touch = e.Touches[0];

                    _selectionStartCell = (
                        (int)(touch.Y / _pixelSize),
                        (int)(touch.X / _pixelSize)
                    );
                    _selectionEndCell = _selectionStartCell;

                    HandleSelection(touch);
                }
                else if(_isMovingPastePreview)
                {
                    (int row, int col)? targetCell = GetCellFromTouchPoint(e.Touches[0]);

                    _viewModel.CurrentPattern.BeginPasteMove(targetCell.Value.row, targetCell.Value.col);

                    _isSelecting = false;
                }
                else
                {
                    _previousTouchPoint = null;
                    _onePoint = e.Touches[0];
                }
            }
            else if (e.Touches.Count() == 2)
            {
                _isDragging = true;

                if (_isSelecting)
                    _isDragging = false;

                _startPosition1 = e.Touches[0];
                _startPosition2 = e.Touches[1];

                //
                _initialDistance = Distance(_startPosition1, _startPosition2);
                _initialScale = PixelGraphicsView.Scale;
            }
        }

        private void OnDragInteraction(object sender, TouchEventArgs e)
        {
            if (e.Touches.Count() == 1)
            {
                if (_isDrawing)
                    HandleInteraction(e.Touches[0]);
                else if (_isSelecting)
                    HandleSelection(e.Touches[0]);
                else if (_isMovingPastePreview)
                {
                    (int row, int col)? targetCell = GetCellFromTouchPoint(e.Touches[0]);

                    if (targetCell.HasValue)
                    {
                        _viewModel.CurrentPattern.SetPastePosition(targetCell.Value.row, targetCell.Value.col);
                        PixelGraphicsView.Invalidate();
                    }
                }
            }
            else if (_isDragging && e.Touches.Count() == 2)
            {
                var currentTouch1 = e.Touches[0];
                var currentTouch2 = e.Touches[1];

                double avgOffsetX = ((currentTouch1.X - _startPosition1.X) + (currentTouch2.X - _startPosition2.X)) / 2;
                double avgOffsetY = ((currentTouch1.Y - _startPosition1.Y) + (currentTouch2.Y - _startPosition2.Y)) / 2;

                PixelGraphicsView.TranslationX += avgOffsetX;
                PixelGraphicsView.TranslationY += avgOffsetY;

                //
                float currentDistance = Distance(currentTouch1, currentTouch2);

                if (_initialDistance > 0)
                {
                    double scaleFactor = currentDistance / _initialDistance;
                    PixelGraphicsView.Scale = _initialScale * scaleFactor;

                    PixelGraphicsView.Scale = Math.Clamp(PixelGraphicsView.Scale, 0.5, 5.0);
                }
            }
        }

        private float Distance(PointF p1, PointF p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        private void HandleInteraction(PointF touchPoint)
        {
            if (_previousTouchPoint.HasValue)
            {
                var previousPoint = _previousTouchPoint.Value;
                float deltaX = touchPoint.X - previousPoint.X;
                float deltaY = touchPoint.Y - previousPoint.Y;
                int steps = (int)Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));

                for (int i = 1; i <= steps; i++)
                {
                    float interpolatedX = previousPoint.X + deltaX * i / steps;
                    float interpolatedY = previousPoint.Y + deltaY * i / steps;
                    _viewModel.CurrentPattern.TogglePixel(interpolatedX, interpolatedY);
                }
            }

            _viewModel.CurrentPattern.TogglePixel(touchPoint.X, touchPoint.Y);
            PixelGraphicsView.Invalidate();

            _previousTouchPoint = touchPoint;
        }

        private void HandleSelection(PointF touchPoint)
        {
            int col = (int)(touchPoint.X / _pixelSize);
            int row = (int)(touchPoint.Y / _pixelSize);

            col = Math.Clamp(col, 0, _columns - 1);
            row = Math.Clamp(row, 0, _rows - 1);

            _selectionEndCell = (row, col);

            _viewModel.CurrentPattern.UpdateSelectionCells(_selectionStartCell, _selectionEndCell);
            PixelGraphicsView.Invalidate();
        }

        private void OnEndInteraction(object sender, TouchEventArgs e)
        {
            if (_isDrawing)
                _viewModel.CurrentPattern.TogglePixel(_onePoint.X, _onePoint.Y);

            _previousTouchPoint = null;
            PixelGraphicsView.Invalidate();
        }
        #endregion

        #region Buttons Interaction 

        private Button _brushButton;

        private void OnStartDrawingClicked(object sender, EventArgs e)
        {
            if (_isSelecting)
                return;

            _isDrawing = !_isDrawing;

            if (sender is Button button)
            {
                _brushButton = button;

                _brushButton.BackgroundColor = _isDrawing
                    ? Color.FromArgb("#553d3a")
                    : Color.FromArgb("#98694d");
            }
                
        }

        private void OnZoomChanged(object sender, EventArgs e)
        {
            if (sender is not Button button)
                return;

            if (button.Text == "+")
                scale *= 1.1;
            else if (button.Text == "-")
                scale /= 1.1;

            //scale = Math.Max(minScale, Math.Min(scale, 1.0));

            PixelGraphicsViewContainer.Scale = scale;
        }

        private void OnMinimum(object sender, EventArgs e)
        {
            if (sender is not Button button)
                return;

            scale = minScale;
            PixelGraphicsViewContainer.Scale = minScale;
        }


        private void OnCenter(object sender, EventArgs e)
        {
            PixelGraphicsView.TranslationX = 0;
            PixelGraphicsView.TranslationY = 0;

            //Dispatcher.Dispatch(() => CanvasScroll.ScrollToAsync(PixelCanvas, ScrollToPosition.Center, false));
        }

        private Button _selectButton;

        private void OnSelectClicked(object sender, EventArgs e)
        {
            if (_isDrawing)
            {
                _isDrawing = false;
                _brushButton.BackgroundColor = Color.FromArgb("#98694d");
            }

            if (sender is Button button)
                _selectButton = button;

            if (_viewModel.CurrentPattern.IsPasting)
            {
                _viewModel.CurrentPattern.CancelPaste();
                _viewModel.CurrentPattern.UpdateSelectionCells(null, null);
                _isMovingPastePreview = false;
                _isSelecting = false;
            }
            else
            {
                _isSelecting = !_isSelecting;

                if (!_isSelecting)
                {
                    _viewModel.CurrentPattern.UpdateSelectionCells(null, null);
                    _isMovingPastePreview = false;
                }
            }

            UpdateUIState();
        }


        private void OnCopyClicked(object sender, EventArgs e)
        {
            _viewModel.CurrentPattern.CopySelection();

            if (_viewModel.CurrentPattern.IsPasting)
            {
                _isSelecting = false;
                _isMovingPastePreview = true;
            }

            UpdateUIState();
        }

        private void OnCutClicked(object sender, EventArgs e)
        {
            _viewModel.CurrentPattern.CutSelection();

            if (_viewModel.CurrentPattern.IsPasting)
            {
                _isSelecting = false;
                _isMovingPastePreview = true;
            }

            UpdateUIState();
        }

        private void OnDoneClicked(object sender, EventArgs e)
        {
            _viewModel.CurrentPattern.ConfirmPaste();
            _isSelecting = true;
            _isMovingPastePreview = false;

            UpdateUIState();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            _viewModel.CurrentPattern.CancelPaste();
            _isSelecting = true;
            _isMovingPastePreview = false;

            UpdateUIState();
        }

        private void UpdateUIState()
        {
            CutCopyButtonsContainer.IsVisible = _isSelecting && !_viewModel.CurrentPattern.IsPasting;
            CancelDoneButtonsContainer.IsVisible = _viewModel.CurrentPattern.IsPasting;

            if (_selectButton != null)
            {
                _selectButton.BackgroundColor = _isSelecting ? Color.FromArgb("#553d3a") : Color.FromArgb("#98694d");
            }

            PixelGraphicsView.Invalidate();
        }

        private const double CollapsedHeight = 55;
        private const double ExpandedHeight = 300;

        private bool _isCollapsed = false;

        private async void OnTogglePanelClicked(object sender, EventArgs e)
        {
            _isCollapsed = !_isCollapsed;

            bool isTablet = DeviceInfo.Idiom == DeviceIdiom.Tablet;

            PaletteScrollView.IsVisible = !_isCollapsed;

            await ResponsivePanel.FadeTo(0.8, 100);

            if (isTablet)
            {
                ResponsivePanel.WidthRequest = _isCollapsed ? 60 : 400;
                ToggleButton.Text = _isCollapsed ? "←" : "→";
            }
            else
            {
                ResponsivePanel.HeightRequest = _isCollapsed ? 60 : 300;
                ToggleButton.Text = _isCollapsed ? "↑" : "↓";
            }

            await ResponsivePanel.FadeTo(1.0, 100);
        }

        public (int row, int col)? GetCellFromTouchPoint(Point touchPoint)
        {
            int col = (int)(touchPoint.X / _pixelSize);
            int row = (int)(touchPoint.Y / _pixelSize);

            if (row >= 0 && row < _rows && col >= 0 && col < _columns)
            {
                return (row, col);
            }
            else
            {
                return null;
            }
        }
        #endregion

        private string _currentPaletteName = "Preciosa Rocialles";
        private async void OnPaletteButtonClicked(object sender, EventArgs e)
        {
            if (BindingContext is MainPageViewModel vm)
            {
                string currentPalette = _currentPaletteName;

                var popup = new ChoosePalettePopup(currentPalette);

                //popup.PaletteSelected += async (selectedPalette) =>
                //{
                //    await vm.LoadPaletteAsync(selectedPalette);
                //    PaletteScrollView.ForceLayout();
                //};

                popup.PaletteSelected += async (selectedPalette) =>
                {
                    PaletteScrollView.Content = ColorPaletteViewCache.GetPaletteView(selectedPalette);
                    _currentPaletteName = selectedPalette;
                    //PaletteScrollView.ForceLayout();
                };

                await this.ShowPopupAsync(popup);
            }
        }

        private async void OnSearchButtonClicked(object sender, EventArgs e)
        {
            var currentPaletteColors = ColorPaletteViewCache.GetPaletteColors(_currentPaletteName);
            if (currentPaletteColors is null)
                return;

            var popup = new SearchColorPopup(currentPaletteColors);

            popup.ColorSelected += async selectedColor =>
            {
                foreach (var c in currentPaletteColors)
                    c.IsSelected = false;

                selectedColor.IsSelected = true;

                _viewModel.SelectColorCommand.Execute(selectedColor);
            };

            await Application.Current.MainPage.ShowPopupAsync(popup);

        }

    }
}
