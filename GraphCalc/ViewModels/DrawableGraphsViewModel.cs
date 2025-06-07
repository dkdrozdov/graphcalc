using System.Collections.ObjectModel;
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

}
