namespace HandfulOfBreads.Views
{
    public partial class MainPage : ContentPage
    {
        private double scale = 1.0;

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
        private const int Rows = 60;
        private const int Columns = 30;
        private Color currentColor = Colors.Black;

        private void CreatePixelCanvas()
        {
            //PixelCanvas.RowDefinitions.Clear();
            //PixelCanvas.ColumnDefinitions.Clear();
            //PixelCanvas.Children.Clear();

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

            double minScale = Math.Min(CanvasScroll.Width / PixelCanvas.Width, CanvasScroll.Height / PixelCanvas.Height);

            if (button.Text == "+")
                scale *= 1.1;
            else if (button.Text == "-")
                scale /= 1.1;

            scale = Math.Max(minScale, Math.Min(scale, 3.0));

            PixelCanvas.Scale = scale;
        }
    }
}
