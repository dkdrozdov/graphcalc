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

    public SplinesViewModel Splines { get; }
    public DrawableGraphsViewModel Graphs { get; }
    public ICommand AddFunctionCommand { get; }
    public ICommand AddSplineCommand { get; }



    public void AddFunction()
    {
        Graphs.Graphs.Add(new DrawableFunctionViewModel(new DrawableFunction("", out _), Graphs));
    }

    public void AddSpline()
    {
        Graphs.Graphs.Add(new DrawableSplineViewModel(SplineBuilder.BuildQuadraticSpline([]), Graphs));
    }


    public MainWindowViewModel()
    {
        Splines = new();
        Graphs = new();

        AddFunctionCommand = new RelayCommand(AddFunction);
        AddSplineCommand = new RelayCommand(AddSpline);

        AddFunction();
    }


}
