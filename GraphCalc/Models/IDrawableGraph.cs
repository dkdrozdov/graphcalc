using System.Collections.Generic;
using System.Numerics;

namespace GraphCalc.Models;

public interface IDrawableGraph
{
    /// <summary>
    /// Returns graph points inside the box defined by parameters.
    /// </summary>
    public IEnumerable<Vector2> PointsInBox(double x1, double x2, double y1, double y2, double step);

    /// <summary>
    /// Returns special points that should be specially rendered.
    /// </summary>
    public IEnumerable<Vector2> SpecialPoints();


    public Vector2? PointAt(double x);
}
