using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
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
