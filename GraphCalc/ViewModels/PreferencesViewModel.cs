using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraphCalc.Models;

namespace GraphCalc.ViewModels;


public partial class PreferencesViewModel : ViewModelBase
{
    [ObservableProperty]
    double _renderStep = 0.002;

    [ObservableProperty]
    bool _showGridlines = true;
    [ObservableProperty]
    bool _showMinorGridlines = true;

    [ObservableProperty]
    bool _showAxisNumbers = true;
    [ObservableProperty]
    bool _showXAxis = true;
    [ObservableProperty]
    bool _showYAxis = true;


    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(RenderStep))
        {
            if (RenderStep < 0.0001)
            {
                RenderStep = 0.0001;
            }
        }
    }

    public PreferencesViewModel()
    {

    }

}
