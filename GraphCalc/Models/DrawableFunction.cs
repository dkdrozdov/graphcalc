using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using MathEvaluation;
using MathEvaluation.Context;
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

    public IEnumerable<Vector2> PointsInBox(double x1, double x2, double y1, double y2, double step)
    {
        List<Vector2> points = [];
        if (Expression == null) return points;

        List<double> grid = [];
        for (int i = 0; x1 + i * step < x2; i++) grid.Add(x1 + i * step);
        grid.Add(x2);

        var compiledExpression = Expression.Compile(new { x = 0.0f });

        bool isOnScreen = true;
        bool isPrevOnScreen = true;

        bool outOfScreenBottom = false;
        bool outOfScreenLeft = false;
        bool outOfScreenTop = false;
        bool outOfScreenRight = false;

        bool prevOutOfScreenBottom = false;
        bool prevOutOfScreenLeft = false;
        bool prevOutOfScreenTop = false;
        bool prevOutOfScreenRight = false;

        bool gap = false;
        bool prevGap = false;

        double prevTick = grid.First();

        grid.ForEach(tick =>
        {
            try
            {
                var result = compiledExpression(new { x = (float)tick });

                isPrevOnScreen = isOnScreen;
                prevOutOfScreenBottom = outOfScreenBottom;
                prevOutOfScreenLeft = outOfScreenLeft;
                prevOutOfScreenTop = outOfScreenTop;
                prevOutOfScreenRight = outOfScreenRight;
                prevGap = gap;

                isOnScreen = tick <= x2 && tick >= x1 && (float)result <= y2 && (float)result >= y1;

                if (!isOnScreen)
                {
                    outOfScreenBottom = result < y1;
                    outOfScreenLeft = tick < x1;
                    outOfScreenTop = result > y2;
                    outOfScreenRight = tick > x2;
                }
                else
                {
                    outOfScreenBottom = false;
                    outOfScreenLeft = false;
                    outOfScreenTop = false;
                    outOfScreenRight = false;
                }

                gap = prevOutOfScreenBottom && outOfScreenTop || prevOutOfScreenLeft && outOfScreenRight ||
                    prevOutOfScreenTop && outOfScreenBottom || prevOutOfScreenRight && outOfScreenLeft;

                if (gap)
                    points.Add(new Vector2((float)prevTick + (float)step / 2f, float.NaN));

                if (!isPrevOnScreen && isOnScreen)
                {
                    var prevResult = compiledExpression(new { x = (float)prevTick });
                    points.Add(new Vector2((float)prevTick, (float)prevResult));
                }

                if (isOnScreen || isPrevOnScreen)
                    points.Add(new Vector2((float)tick, (float)result));

                prevTick = tick;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        });

        return points;
    }

    public IEnumerable<Vector2> SpecialPoints() => [];

    public Vector2? PointAt(double x)
    {
        if (Expression == null) return null;
        var compiledExpression = Expression.Compile(new { x = 0.0f });

        try
        {
            var result = compiledExpression(new { x = (float)x });
            if (!double.IsNaN(result)) return new Vector2((float)x, (float)result);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }

        return null;
    }
}