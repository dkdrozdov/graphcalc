using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraphCalc.Models;

namespace GraphCalc.ViewModels;

public abstract partial class DrawableGraphViewModel : ObservableObject
{
    [ObservableProperty]
    private IDrawableGraph _graph;

    [ObservableProperty]
    private float _lineWidth;

    [ObservableProperty]
    private float _lineOpacity;

    [ObservableProperty]
    private string? expressionLog;
    [ObservableProperty]
    private bool _isHidden;
    [ObservableProperty]
    private IBrush _brush;
    [ObservableProperty]
    private bool _colorPalette;
    public ICommand RemoveCommand { get; }
    protected readonly DrawableGraphsViewModel drawableGraphsViewModel;

    [RelayCommand]
    private void SetColor(ImmutableSolidColorBrush color)
    {
        Brush = color;
    }

    public DrawableGraphViewModel(IDrawableGraph graph, DrawableGraphsViewModel _drawableGraphsViewModel)
    {
        Graph = graph;
        LineWidth = 2;
        LineOpacity = 1;
        RemoveCommand = new RelayCommand(Remove);
        drawableGraphsViewModel = _drawableGraphsViewModel;
        IsHidden = false;
        Brush = new SolidColorBrush(Colors.Black, LineOpacity);
    }

    public void Remove()
    {
        drawableGraphsViewModel.Graphs.Remove(this);
    }

    public abstract Task ParseStringInputAsync(string? content);
}
