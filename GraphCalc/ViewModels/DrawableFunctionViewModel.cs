using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraphCalc.Models;

namespace GraphCalc.ViewModels;


public partial class DrawableFunctionViewModel(IDrawableGraph graph, DrawableGraphsViewModel _drawableGraphsViewModel) : DrawableGraphViewModel(graph, _drawableGraphsViewModel)
{
    [ObservableProperty]
    private string? expressionLog;
    [ObservableProperty]
    private string? _userExpression;
    [ObservableProperty]
    private bool expressionLogNotEmpty = false;

    public override async Task ParseStringInputAsync(string? content)
    {
        UserExpression = content;
        await Task.CompletedTask;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(UserExpression))
        {

            Dictionary<string, object> variables = [];

            for (char i = 'a'; i <= 'z'; i++)
            {
                variables.Add(i.ToString(), new { Value = 0, Exists = false });
            }

            Graph = new DrawableFunction(UserExpression ?? "", out string resultMessage);
            ExpressionLog = resultMessage;
            ExpressionLogNotEmpty = resultMessage != "";

        }
    }
}
