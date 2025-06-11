using System;
using System.Collections.Generic;
using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;
using GraphCalc.Models;

namespace GraphCalc.ViewModels
{
    public partial class SplineFactoryViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name;
        public Func<IEnumerable<Vector2>, Spline> FactoryMethod { get; }

        public SplineFactoryViewModel(string name, Func<IEnumerable<Vector2>, Spline> factoryMethod)
        {
            Name = name;
            FactoryMethod = factoryMethod;
        }
    }

}