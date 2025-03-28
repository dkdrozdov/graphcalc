using CommunityToolkit.Mvvm.ComponentModel;

namespace GraphCalc.ViewModels;

public partial class PointViewModel : ViewModelBase
{
    [ObservableProperty]
    private double _x;
    [ObservableProperty]
    private double _y;
}