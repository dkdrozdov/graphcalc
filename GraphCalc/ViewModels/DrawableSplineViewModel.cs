using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraphCalc.Models;

namespace GraphCalc.ViewModels;


public partial class DrawableSplineViewModel : DrawableGraphViewModel
{
    public ICommand AddPointCommand { get; }
    public ICommand RemovePointCommand { get; }
    public ICommand RebuildSplineCommand { get; }
    public ObservableCollection<PointViewModel> SplinePoints { get; }

    public DrawableSplineViewModel(IDrawableGraph graph, DrawableGraphsViewModel _drawableGraphsViewModel) : base(graph, _drawableGraphsViewModel)
    {
        SplinePoints = [];
        AddPointCommand = new RelayCommand(AddPoint);
        RemovePointCommand = new RelayCommand(RemovePoint);
        RebuildSplineCommand = new RelayCommand(RebuildSpline);
    }

    public void AddPoint()
    {
        var point = new PointViewModel();
        point.AfterPropertyChanged += RebuildSpline;
        SplinePoints.Add(point);

        // RebuildSpline();
    }

    public void RemovePoint()
    {
        if (SplinePoints.Count < 1) return;

        var last = SplinePoints.Last();

        last.AfterPropertyChanged -= RebuildSpline;
        SplinePoints.Remove(last);
        RebuildSpline();
    }

    public void RebuildSpline()
    {
        var spline = SplineBuilder.BuildQuadraticSpline([.. SplinePoints.OrderBy(p => p.X).Select(p => new Vector2((float)p.X, (float)p.Y))]);
        if (spline != null) Graph = spline;
    }
}
