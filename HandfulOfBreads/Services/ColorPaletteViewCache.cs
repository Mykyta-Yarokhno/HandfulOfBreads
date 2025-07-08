using System;
using System.Collections.Generic;
using HandfulOfBreads.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

public static class ColorPaletteViewCache
{
    private static readonly Dictionary<string, View> _paletteViews = new();

    private static Dictionary<string, List<ColorItemViewModel>> _allPalettes;

    public static void InitializeAllPalettes(
        Dictionary<string, List<ColorItemViewModel>> allPalettes,
        Action<ColorItemViewModel> onColorTapped)
    {
        AppLogger.Info($">>{nameof(InitializeAllPalettes)}");
        _allPalettes = allPalettes;

        foreach (var kvp in allPalettes)
        {
            string paletteName = kvp.Key;
            List<ColorItemViewModel> colors = kvp.Value;

            var collectionView = new CollectionView
            {
                ItemsSource = colors,
                ItemsLayout = new GridItemsLayout(ItemsLayoutOrientation.Vertical)
                {
                    Span = 5
                },
                ItemTemplate = new DataTemplate(() =>
                {
                    var frame = new Frame
                    {
                        Padding = 0,
                        Margin = 5,
                        HeightRequest = 50,
                        WidthRequest = 50,
                        CornerRadius = 5,
                        HasShadow = true
                    };

                    frame.SetBinding(Frame.BackgroundColorProperty, nameof(ColorItemViewModel.HexColor));
                    frame.SetBinding(Frame.BorderColorProperty, new Binding(
                        nameof(ColorItemViewModel.IsSelected),
                        converter: (IValueConverter)Application.Current.Resources["BoolToColorConverter"]));

                    var grid = new Grid();

                    grid.Children.Add(CreateLabel(nameof(ColorItemViewModel.Sign), LayoutOptions.Start, LayoutOptions.Start));
                    grid.Children.Add(CreateLabel(nameof(ColorItemViewModel.Sign), LayoutOptions.End, LayoutOptions.Start));
                    grid.Children.Add(CreateLabel(nameof(ColorItemViewModel.Sign), LayoutOptions.Start, LayoutOptions.End));
                    grid.Children.Add(CreateLabel(nameof(ColorItemViewModel.Sign), LayoutOptions.End, LayoutOptions.End));

                    var centerLabel = new Label
                    {
                        FontSize = 10,
                        TextColor = Colors.Black,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };
                    centerLabel.SetBinding(Label.TextProperty, nameof(ColorItemViewModel.Code));

                    grid.Children.Add(centerLabel);

                    var tap = new TapGestureRecognizer();
                    tap.Tapped += (s, e) =>
                    {
                        if (frame.BindingContext is ColorItemViewModel selectedVm)
                        {
                            foreach (var c in colors)
                                c.IsSelected = c == selectedVm;

                            onColorTapped?.Invoke(selectedVm);
                        }
                    };
                    frame.GestureRecognizers.Add(tap);

                    frame.Content = grid;
                    return frame;
                })
            };

            _paletteViews[paletteName] = collectionView;
        }

        AppLogger.Info($"<<{nameof(InitializeAllPalettes)}");
    }

    //public static void InitializeAllPalettes(
    //Dictionary<string, List<ColorItemViewModel>> allPalettes,
    //Action<ColorItemViewModel> onColorTapped)
    //{
    //    _allPalettes = allPalettes;

    //    foreach (var kvp in allPalettes)
    //    {
    //        string paletteName = kvp.Key;
    //        List<ColorItemViewModel> colors = kvp.Value;

    //        var grid = new Grid
    //        {
    //            ColumnSpacing = 5,
    //            RowSpacing = 5,
    //            Padding = new Thickness(10)
    //        };

    //        for (int col = 0; col < 5; col++)
    //            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

    //        int rows = (int)Math.Ceiling(colors.Count / 5.0);
    //        for (int row = 0; row < rows; row++)
    //            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

    //        for (int i = 0; i < colors.Count; i++)
    //        {
    //            var color = colors[i];

    //            var frame = new Frame
    //            {
    //                Padding = 0,
    //                Margin = 0,
    //                HeightRequest = 50,
    //                WidthRequest = 50,
    //                CornerRadius = 5,
    //                HasShadow = true,
    //                BindingContext = color
    //            };

    //            frame.SetBinding(Frame.BackgroundColorProperty, nameof(ColorItemViewModel.HexColor));
    //            frame.SetBinding(Frame.BorderColorProperty, new Binding(
    //                nameof(ColorItemViewModel.IsSelected),
    //                converter: (IValueConverter)Application.Current.Resources["BoolToColorConverter"]));

    //            var innerGrid = new Grid();

    //            innerGrid.Children.Add(CreateLabel(nameof(ColorItemViewModel.Sign), LayoutOptions.Start, LayoutOptions.Start));
    //            innerGrid.Children.Add(CreateLabel(nameof(ColorItemViewModel.Sign), LayoutOptions.End, LayoutOptions.Start));
    //            innerGrid.Children.Add(CreateLabel(nameof(ColorItemViewModel.Sign), LayoutOptions.Start, LayoutOptions.End));
    //            innerGrid.Children.Add(CreateLabel(nameof(ColorItemViewModel.Sign), LayoutOptions.End, LayoutOptions.End));

    //            var centerLabel = new Label
    //            {
    //                FontSize = 10,
    //                TextColor = Colors.Black,
    //                HorizontalOptions = LayoutOptions.Center,
    //                VerticalOptions = LayoutOptions.Center
    //            };
    //            centerLabel.SetBinding(Label.TextProperty, nameof(ColorItemViewModel.Code));

    //            innerGrid.Children.Add(centerLabel);

    //            var tap = new TapGestureRecognizer();
    //            tap.Tapped += (s, e) =>
    //            {
    //                foreach (var c in colors)
    //                    c.IsSelected = c == color;

    //                onColorTapped?.Invoke(color);
    //            };
    //            frame.GestureRecognizers.Add(tap);

    //            frame.Content = innerGrid;

    //            Grid.SetRow(frame, i / 5);
    //            Grid.SetColumn(frame, i % 5);

    //            grid.Children.Add(frame);
    //        }

    //        _paletteViews[paletteName] = grid;
    //    }

    //    AppLogger.Info("InitializeAllPalettes done");
    //}

    public static View? GetPaletteView(string paletteName)
    {
        AppLogger.Info("GetPaletteView done");

        return _paletteViews.TryGetValue(paletteName, out var view) ? view : null;
    }

    private static Label CreateLabel(string bindingPath, LayoutOptions hAlign, LayoutOptions vAlign)
    {
        var label = new Label
        {
            FontSize = 10,
            TextColor = Colors.Black,
            Margin = 5,
            HorizontalOptions = hAlign,
            VerticalOptions = vAlign
        };
        label.SetBinding(Label.TextProperty, bindingPath);
        return label;
    }

    public static List<ColorItemViewModel>? GetPaletteColors(string paletteName)
    {
        AppLogger.Info("GetPaletteColors done");

        return _allPalettes.TryGetValue(paletteName, out var colors) ? colors : null;
    }
}
