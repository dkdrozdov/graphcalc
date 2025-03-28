using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using GraphCalc.ViewModels;

namespace GraphCalc.Views;

public partial class MainWindow : Window
{
    private readonly ZoomBorder? _zoomBorder;
    // public Matrix? Matrix => _zoomBorder?.Matrix;

    public MainWindow()
    {
        InitializeComponent();
        this.AttachDevTools();

        _zoomBorder = this.Find<ZoomBorder>("ZoomBorder");
        if (_zoomBorder != null)
        {
            _zoomBorder.KeyDown += ZoomBorder_KeyDown;

            _zoomBorder.ZoomChanged += ZoomBorder_ZoomChanged;
        }
    }

    private void ZoomBorder_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.R:
                _zoomBorder?.ResetMatrix();
                break;
        }
    }

    private void ZoomBorder_ZoomChanged(object sender, ZoomChangedEventArgs e)
    {
        // Debug.WriteLine($"[ZoomChanged] {e.ZoomX} {e.ZoomY} {e.OffsetX} {e.OffsetY}");
        // Debug.WriteLine($"[ZoomChanged] {Matrix!.Value.M11} {Matrix!.Value.M12} {Matrix!.Value.M13} {Matrix!.Value.M21} {Matrix!.Value.M22} {Matrix!.Value.M23} {Matrix!.Value.M31} {Matrix!.Value.M32} {Matrix!.Value.M33}");
    }

}
