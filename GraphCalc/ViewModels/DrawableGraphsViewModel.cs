using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GraphCalc.Models;

namespace GraphCalc.ViewModels;

public class DrawableGraphsViewModel : ObservableObject
{
    public ObservableCollection<DrawableGraphViewModel> Graphs { get; set; } = [];
    public ObservableCollection<SplineFactoryViewModel> SplineFactories { get; } =
[
    new("Interpolating linear spline", SplineFactory.LinearSpline),
        new("Interpolating quadratic spline", SplineFactory.QuadraticSpline),
        new("Interpolating cubic spline", SplineFactory.CubicSpline)
];

}
