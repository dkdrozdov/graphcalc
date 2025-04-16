using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathEvaluation;
using MathEvaluation.Context;
using MathEvaluation.Extensions;
using MathNet.Numerics;

namespace GraphCalc.Models;

public class DrawableFunction : IDrawableGraph
{
    public DrawableFunction(string expression, out string resultMessage)
    {
        ExpressionString = expression;
        try
        {
            Expression = BuildExpression(out resultMessage);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"{exception}");
            resultMessage = exception.Message;
        }

    }

    public string ExpressionString { get; }
    public MathExpression? Expression { get; }

    private MathExpression? BuildExpression(out string resultMessage)
    {
        resultMessage = $"";

        if (string.IsNullOrWhiteSpace(ExpressionString))
        {
            resultMessage = $"";
            return null;
        }

        var context = new ScientificMathContext();

        context.BindFunction(Math.Sqrt, "sqrt");
        context.BindFunction((double d) => Math.Floor(d), "floor");
        context.BindOperandOperator(d => SpecialFunctions.Gamma(d + 1), '!', isProcessingLeft: true);

        // context.BindConstant(Math.PI)
        // context.BindFunction(d => Math.Log(d), "ln");
        // context.BindFunction(Math.Sin, "sin");
        // context.BindFunction(Math.Cos, "cos");

        using var expression = new MathExpression(ExpressionString ?? "", context);

        //  test evaluation (for error message)
        try
        {
            var result = expression.Evaluate(new { x = 1 });

            // Console.WriteLine($"{ExpressionString} -> x=1: {result}");
            resultMessage = $"";
        }
        catch (Exception exception)
        {
            // if (exception is MathExpressionException expressionException)
            // {
            // Console.WriteLine($"{expressionException}\n");
            // }
            // Console.WriteLine($"{exception}\n Data:");
            foreach (var item in exception.Data)
            {
                // Console.WriteLine($"{item}\n");
                resultMessage = $"Can't resolve symbol: {exception.Data["mathString"]}";
            }
            return null;
        }
        return expression;
    }

    public List<Vector2> PointsInBox(float x1, float x2, float y1, float y2, float step)
    {
        List<Vector2> points = [];
        if (Expression == null) return points;

        List<float> grid = [];
        for (int i = 0; x1 + i * step < x2; i++) grid.Add(x1 + i * step);

        var compiledExpression = Expression.Compile(new { x = 0.0f });

        grid.ForEach(tick =>
        {
            try
            {
                var result = compiledExpression(new { x = tick });
                points.Add(new Vector2(tick, (float)result));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        });

        return points;
    }

}