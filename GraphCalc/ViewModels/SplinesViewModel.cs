using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GraphCalc.Models;

namespace GraphCalc.ViewModels;

public class SplinesViewModel : ObservableObject
{
    public ObservableCollection<Spline> Splines { get; set; } = [];
}
