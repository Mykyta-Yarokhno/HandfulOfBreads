namespace HandfulOfBreads.Views
{
    public partial class MainPage : ContentPage
    {
        private double scale = 0.0;
        private double minScale = 0.5;

        public MainPage()
        {
            InitializeComponent();
            CreatePixelCanvas();
        }

        private bool _isPanelVisible = false;

        private async void OnToggleClicked(object sender, EventArgs e)
        {
            if (_isPanelVisible)
            {
                await SidePanel.TranslateTo(-SidePanel.Width, 0, 250);
                SidePanel.WidthRequest = 0;
            }
            else
            {
                SidePanel.WidthRequest = 50;
                await SidePanel.TranslateTo(0, 0, 250);
            }

            _isPanelVisible = !_isPanelVisible;
        }

        private const int CellSize = 20;
        private const int Rows = 40;
        private const int Columns = 10;
        private Color currentColor = Colors.Black;

        private void CreatePixelCanvas()
        {
            for (int i = 0; i < Rows; i++)
            {
                PixelCanvas.RowDefinitions.Add(new RowDefinition { Height = new GridLength(CellSize) });
            }

            for (int j = 0; j < Columns; j++)
            {
                PixelCanvas.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(CellSize) });
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var box = new BoxView
                    {
                        BackgroundColor = Colors.White
                    };

                    var frame = new Frame
                    {
                        Content = box,
                        Padding = 0,
                        Margin = 0.5,
                        BorderColor = Colors.Gray,
                        CornerRadius = 0
                    };

                    var tapGesture = new TapGestureRecognizer();
                    tapGesture.Tapped += OnCellTapped;
                    frame.GestureRecognizers.Add(tapGesture);

                    Grid.SetRow(frame, i);
                    Grid.SetColumn(frame, j);

                    PixelCanvas.Children.Add(frame);
                }
            }
        }

        private void OnCellTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.Content is BoxView box)
            {
                box.BackgroundColor = currentColor;
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
            
            scale = Math.Max(minScale, Math.Min(scale, 1.0));

            PixelCanvas.Scale = scale;
        }

        private void OnEqualed(object sender, EventArgs e)
        {
            if (sender is not Button button)
                return;

            scale = Math.Min(CanvasScroll.Width / PixelCanvas.Width, CanvasScroll.Height / PixelCanvas.Height);

            PixelCanvas.Scale = scale;

            CenterPixelCanvas();
        }

        private async void OnCanvasScrollLoaded(object sender, EventArgs e)
        {
            CanvasScroll.SizeChanged += OnCanvasScrollSizeChanged;
        }

        private void OnCanvasScrollSizeChanged(object sender, EventArgs e)
        {
            if (CanvasScroll.Width > 0)
            {
                SetPadding();
                CanvasScroll.SizeChanged -= OnCanvasScrollSizeChanged;
            }
        }

        private void SetPadding()
        {

            double leftRightPadding = CanvasScroll.Width * 0.5;

            double upDownPadding = CanvasScroll.Height * 0.5;

            PixelCanvas.Padding = new Thickness(leftRightPadding, upDownPadding, leftRightPadding, upDownPadding);
        }

        private async void OnPixelCanvasLoaded(object sender, EventArgs e)
        {
            PixelCanvas.SizeChanged += OnPixelCanvasChanged;
        }

        private async void OnPixelCanvasChanged(object sender, EventArgs e)
        {
            if (CanvasScroll.Width > 0)
            {
                SetScale();
                
                PixelCanvas.SizeChanged -= OnPixelCanvasChanged;

                Dispatcher.Dispatch(() =>
                {
                    CenterPixelCanvas();
                });
            }
        }

        private void SetScale()
        {
            scale = minScale;
            PixelCanvas.Scale = scale;
        }

        

        private void OnCenter(object sender, EventArgs e)
        {
            CenterPixelCanvas();
        }

        private async void CenterPixelCanvas()
        {

            Dispatcher.Dispatch(() => CanvasScroll.ScrollToAsync(PixelCanvas, ScrollToPosition.Center, false));

            //await CanvasScroll.ScrollToAsync(PixelCanvas, ScrollToPosition.Center, animated: false);
        }
    }
}
