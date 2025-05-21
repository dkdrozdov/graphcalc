// using System;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Windows.Input;
// using Avalonia.Media;
// using Avalonia.Media.Immutable;
// using CommunityToolkit.Mvvm.ComponentModel;
// using CommunityToolkit.Mvvm.Input;
// using GraphCalc.Models;

// namespace GraphCalc.ViewModels;


// public partial class DrawableGraphViewModel : ObservableObject
// {
//     [ObservableProperty]
//     private IDrawableGraph _graph;

//     [ObservableProperty]
//     private float _lineWidth;

//     [ObservableProperty]
//     private float _lineOpacity;

//     [ObservableProperty]
//     private string? expressionLog;
//     [ObservableProperty]
//     private string? _userExpression;
//     [ObservableProperty]
//     private bool expressionLogNotEmpty;
//     [ObservableProperty]
//     private bool _isHidden;
//     [ObservableProperty]
//     private IBrush _brush;
//     [ObservableProperty]
//     private bool _colorPalette;
//     public ICommand RemoveCommand { get; }
//     private readonly DrawableGraphsViewModel drawableGraphsViewModel;

//     [RelayCommand]
//     private void SetColor(ImmutableSolidColorBrush color)
//     {
//         Brush = color;
//     }

//     public DrawableGraphViewModel(IDrawableGraph graph, DrawableGraphsViewModel _drawableGraphsViewModel)
//     {
//         Graph = graph;
//         LineWidth = 1;
//         LineOpacity = 1;
//         expressionLogNotEmpty = false;
//         RemoveCommand = new RelayCommand(Remove);
//         drawableGraphsViewModel = _drawableGraphsViewModel;
//         IsHidden = false;
//         Brush = new SolidColorBrush(Colors.Black, LineOpacity);
//     }

//     public void Remove()
//     {
//         drawableGraphsViewModel.Graphs.Remove(this);
//     }

//     protected override void OnPropertyChanged(PropertyChangedEventArgs e)
//     {
//         base.OnPropertyChanged(e);

//         if (e.PropertyName == nameof(UserExpression))
//         {

//             Dictionary<string, object> variables = [];

//             for (char i = 'a'; i <= 'z'; i++)
//             {
//                 variables.Add(i.ToString(), new { Value = 0, Exists = false });
//             }

//             Graph = new DrawableFunction(UserExpression ?? "", out string resultMessage);
//             ExpressionLog = resultMessage;
//             ExpressionLogNotEmpty = resultMessage != "";

//         }
//         if (e.PropertyName == nameof(LineOpacity))
//         {
//             Brush = new SolidColorBrush(Colors.Black, LineOpacity);
//         }
//     }
// }
