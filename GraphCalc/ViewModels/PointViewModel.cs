using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GraphCalc.ViewModels;

public partial class PointViewModel : ViewModelBase
{
    [ObservableProperty]
    private double _x;
    [ObservableProperty]
    private double _y;

    public delegate void AfterPropertyChangedEventHandler();
    public event AfterPropertyChangedEventHandler? AfterPropertyChanged;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        AfterPropertyChanged?.Invoke();
    }
}