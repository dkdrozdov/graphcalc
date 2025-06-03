using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using GraphCalc.Controls;
using GraphCalc.ViewModels;

namespace GraphCalc.Views;

public partial class MainWindow : Window
{

    private readonly ZoomBorder? _zoomBorder;
    private readonly Button? _resetViewButton;
    private readonly GraphGridControl? _graphGridControl;
    // public Matrix? Matrix => _zoomBorder?.Matrix;

    public MainWindow()
    {
        InitializeComponent();
        this.AttachDevTools();

        _zoomBorder = this.Find<ZoomBorder>("ZoomBorder");
        _resetViewButton = this.Find<Button>("ResetViewButton");
        _graphGridControl = this.Find<GraphGridControl>("GraphGrid");


        if (_resetViewButton != null)
        {
            _resetViewButton.Click += ResetView;
        }

        if (_zoomBorder != null)
        {
            _zoomBorder.KeyDown += ZoomBorder_KeyDown;
            _zoomBorder.ZoomChanged += ZoomBorder_ZoomChanged;
            _zoomBorder.PointerPressed += GraphGrid_PointerPressed;
            _zoomBorder.PointerReleased += GraphGrid_PointerReleased;
            _zoomBorder.PointerMoved += GraphGrid_PointerMoved;
        }

        // if (_graphGridControl != null)
        // {
        // }
    }

    private void GraphGrid_PointerMoved(object? sender, PointerEventArgs e)
    {
        _graphGridControl?.Moved(e);
    }

    private void GraphGrid_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _graphGridControl?.Released(e);
    }

    private void GraphGrid_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _graphGridControl?.Pressed(e);
    }

    private async void LoadFromFile(object sender, RoutedEventArgs args)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = GetTopLevel(this);

        if (topLevel == null) return;

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            // Open reading stream from the first file.
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            // Reads all the content of file as a text.
            var fileContent = await streamReader.ReadToEndAsync();

            if (((StyledElement)sender).DataContext is DrawableGraphViewModel drawableGraphViewModel)
                await drawableGraphViewModel.ParseStringInputAsync(fileContent);

        }
    }


    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ResetView(object? sender, RoutedEventArgs e)
    {
        ResetView();
    }

    public void ResetView()
    {
        _zoomBorder?.ResetMatrix();
    }

    private void ZoomBorder_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.R:
                ResetView();
                break;
        }
    }

    private void ZoomBorder_ZoomChanged(object sender, ZoomChangedEventArgs e)
    {
        // Debug.WriteLine($"[ZoomChanged] {e.ZoomX} {e.ZoomY} {e.OffsetX} {e.OffsetY}");
        // Debug.WriteLine($"[ZoomChanged] {Matrix!.Value.M11} {Matrix!.Value.M12} {Matrix!.Value.M13} {Matrix!.Value.M21} {Matrix!.Value.M22} {Matrix!.Value.M23} {Matrix!.Value.M31} {Matrix!.Value.M32} {Matrix!.Value.M33}");
    }

}
