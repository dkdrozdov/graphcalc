using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GraphCalc.Models;

namespace GraphCalc.ViewModels;

public class DrawableGraphsViewModel : ObservableObject
{
    public ObservableCollection<IDrawableGraph> Graphs { get; set; } = [];
}
