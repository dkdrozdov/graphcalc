using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraphCalc.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace GraphCalc.ViewModels;


public partial class DrawableSplineViewModel : DrawableGraphViewModel
{
    public ICommand AddPointCommand { get; }
    public ICommand RemovePointCommand { get; }
    public ICommand RebuildSplineCommand { get; }
    public ObservableCollection<PointViewModel> SplinePoints { get; }

    [ObservableProperty]
    SplineFactoryViewModel _selectedSplineFactory;

    [ObservableProperty]
    bool _splinePointsNotEmpty;

    public ObservableCollection<SplineFactoryViewModel> SplineFactories => drawableGraphsViewModel.SplineFactories;

    public DrawableSplineViewModel(IDrawableGraph graph, DrawableGraphsViewModel _drawableGraphsViewModel) : base(graph, _drawableGraphsViewModel)
    {
        SplinePoints = [];
        AddPointCommand = new RelayCommand(AddPoint);
        RemovePointCommand = new RelayCommand(RemovePoint);
        RebuildSplineCommand = new RelayCommand(RebuildSpline);
        SplinePointsNotEmpty = false;
        SelectedSplineFactory = drawableGraphsViewModel.SplineFactories.First(x => x.Name.Contains("lagrange", StringComparison.CurrentCultureIgnoreCase));
    }

    public void AddPoint(double x, double y)
    {
        var point = new PointViewModel
        {
            X = x,
            Y = y
        };

        point.AfterPropertyChanged += RebuildSpline;
        SplinePoints.Add(point);
        SplinePointsNotEmpty = true;

        RebuildSpline();
    }

    public void AddPoint()
    {
        AddPoint(0, 0);
    }

    public void RemovePoint()
    {
        if (SplinePoints.Count < 1) return;

        var last = SplinePoints.Last();

        last.AfterPropertyChanged -= RebuildSpline;
        SplinePoints.Remove(last);
        RebuildSpline();

        if (SplinePoints.Count == 0)
            SplinePointsNotEmpty = false;
    }


    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(SelectedSplineFactory)) RebuildSpline();
    }

    public void RebuildSpline()
    {
        var spline = SelectedSplineFactory.FactoryMethod([.. SplinePoints.OrderBy(p => p.X).Select(p => new Vector2((float)p.X, (float)p.Y))]);

        if (spline != null) Graph = spline;
    }

    public override async Task ParseStringInputAsync(string? content)
    {
        var lines = (content ?? " ").Split('\n');
        while (SplinePoints.Count != 0) RemovePoint();

        for (int i = 0; i < lines.Length; i++)
        {
            string? line = lines[i];
            var subs = line.Split().Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if (subs.Count != 2)
            {
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Error", $"Invalid amount of parameters at line {i}", ButtonEnum.Ok, Icon.Error);
                await box.ShowAsync();
                return;
            }


            if (double.TryParse(subs[0], out var x) && double.TryParse(subs[1], out var y))
            {
                AddPoint();
                var last = SplinePoints.Last();
                last.X = x;
                last.Y = y;
            }
            else
            {
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Error", $"Invalid parameters at line {i}", ButtonEnum.Ok, Icon.Error);
                await box.ShowAsync();
                return;
            }
        }

        await Task.CompletedTask;
    }
}
