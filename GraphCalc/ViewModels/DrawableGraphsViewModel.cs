using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GraphCalc.Models;

namespace GraphCalc.ViewModels;

public class DrawableGraphsViewModel : ObservableObject
{
    public ObservableCollection<DrawableGraphViewModel> Graphs { get; set; } = [];
    public ObservableCollection<SplineFactoryViewModel> SplineFactories { get; } =
        [
            new("Linear interpolation spline", SplineFactory.LinearInterpolationSpline),
            new("Quadratic interpolation spline", SplineFactory.QuadraticInterpolationSpline),
            new("Lagrange interpolation spline", SplineFactory.LagrangeInterpolationSpline),
            new("Cubic Hermite interpolation spline", SplineFactory.HermitCubicInterpolationSpline)
        ];

    public void SelectEdit(DrawableGraphViewModel drawableGraphViewModel)
    {
        foreach (var g in Graphs.OfType<DrawableSplineViewModel>())
        {
            if (g != drawableGraphViewModel) g.IsEditSelected = false;
        }
    }
}
