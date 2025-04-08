using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using GraphCalc.Models;

namespace GraphCalc.Models;

public interface IDrawableGraph
{
    /// <summary>
    /// Returns graph points inside the box defined by parameters.
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="x2"></param>
    /// <param name="y1"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public List<Vector2> PointsInBox(float x1, float x2, float y1, float y2, float step);
}
