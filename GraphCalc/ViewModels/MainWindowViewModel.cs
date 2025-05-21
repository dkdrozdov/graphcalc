using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraphCalc.Models;

namespace GraphCalc.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private PointViewModel? _selectedPoint;
    public SplinesViewModel Splines { get; }
    public DrawableGraphsViewModel Graphs { get; }
    public ObservableCollection<PointViewModel> SplinePoints { get; }
    public ICommand AddPointCommand { get; }
    public ICommand AddFunctionCommand { get; }
    public ICommand RemovePointCommand { get; }
    public ICommand RebuildSplineCommand { get; }

    public void RebuildSpline()
    {
        Splines.Splines.Clear();
        var spline = SplineBuilder.BuildQuadraticSpline([.. SplinePoints.OrderBy(p => p.X).Select(p => new Vector2((float)p.X, (float)p.Y))]);
        if (spline != null) Splines.Splines.Add(spline);
    }

    public void AddPoint()
    {
        SplinePoints.Add(new());
    }
    public void AddFunction()
    {
        Graphs.Graphs.Add(new(new DrawableFunction("", out _), Graphs));
    }


    public void RemovePoint()
    {
        if (SelectedPoint == null) return;

        SplinePoints.Remove(SelectedPoint);
    }

    public MainWindowViewModel()
    {
        Splines = new();
        Graphs = new();
        Graphs.Graphs.Add(new(new DrawableFunction("", out _), Graphs));
        SplinePoints = [];

        AddPointCommand = new RelayCommand(AddPoint);
        AddFunctionCommand = new RelayCommand(AddFunction);
        RemovePointCommand = new RelayCommand(RemovePoint);
        RebuildSplineCommand = new RelayCommand(RebuildSpline);
        // List<Vector2> points = [new(2, 1), new(5, 8), new(7, 3)];
        // List<SplineSegment> segments =
        // [
        //     new(points[0], points[1], [0, 1.5, -0.75, 0.25]),
        //     new(points[1], points[2], [4, -4.5, 2.25, -0.25])
        // ];

        // Splines.Splines.Add(new Spline(points, segments));

        // Splines.Splines.Add(SplineBuilder.BuildQuadraticSpline(points)!);
    }


}
