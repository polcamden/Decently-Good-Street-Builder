using System.Collections.Generic;
using DecentlyGoodStreetBuilder;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public struct CubicBezierCurve
{
    [SerializeField] private Vector3 anchor1;
    [SerializeField] private Vector3 anchor2;
    [SerializeField] private Vector3 handle1;
    [SerializeField] private Vector3 handle2;
    [SerializeField] private float angle1;
    [SerializeField] private float angle2;

    public CubicBezierCurve(Vector3 anchor1, Vector3 anchor2, Vector3 handle1, Vector3 handle2)
    {
        this.anchor1 = anchor1;
        this.anchor2 = anchor2;
        this.handle1 = handle1;
        this.handle2 = handle2;
        angle1 = 0;
        angle2 = 0;
    }

    public CubicBezierCurve(Vector3 anchor1, Vector3 anchor2, Vector3 handle1, Vector3 handle2, float angle1, float angle2)
    {
        this.anchor1 = anchor1;
        this.anchor2 = anchor2;
        this.handle1 = handle1;
        this.handle2 = handle2;
        this.angle1 = angle1;
        this.angle2 = angle2;
    }

    /// <summary>
    /// Returns curves points and spine
    /// </summary>
    /// <param name="resolution"></param>
    /// <returns></returns>
    public (Vector3[], Vector3[]) curvePointsSpine(float resolution)
    {
        Vector3[] points = curvePoints(resolution);
        Vector3[] spine = curveSpine(points);

        return (points, spine);
    }

    /// <summary>
    /// return curve points with evenly spaced points
    /// </summary>
    /// <param name="resolution"></param>
    /// <returns></returns>
    public Vector3[] curvePoints(float resolution)
    {
        //get length
        float curveDist = 0;
        Vector3 prevPoint = anchor1;
        for (int i = 1; i < 50; i++)
        {
            Vector3 point = CubicBezierCurvePoint(i / 50f);
            curveDist += Vector3.Distance(prevPoint, point);
            prevPoint = point;
        }

        //calcs the new resolution so theres no uneven points
        float fixedResolution = (curveDist % resolution) / (int)(curveDist / resolution) + resolution;

        //find points
        List<Vector3> points = new List<Vector3>
            {
                anchor1
            };

        for (int i = 0; i < 1000; i++)
        {
            Vector3 p = CubicBezierCurvePoint(i / 1000f);
            if (Vector3.Distance(points[points.Count - 1], p) >= fixedResolution)
            {
                points.Add(p);
            }
        }

        points.RemoveAt(points.Count - 1);
        points.Add(anchor2);

        return points.ToArray();
    }

    /// <summary>
    /// calculates spine of curve as normals local to point at i
    /// </summary>
    /// <param name="curvePoints"></param>
    /// <returns></returns>
    public Vector3[] curveSpine(Vector3[] curvePoints)
    {
        Vector3[] spine = new Vector3[curvePoints.Length];

        for (int i = 0; i < curvePoints.Length; i++)
        {
            Vector3 p;
            float angle = Mathf.Lerp(angle1, angle2, (float)i / curvePoints.Length);

            if (i == 0) //start
            {
                p = GeometryF.NormalLeft(curvePoints[i], handle1, angle1);
            }
            else if (i == curvePoints.Length - 1) //end
            {
                p = GeometryF.NormalLeft(handle2, curvePoints[i], angle2);
            }
            else //middle points
            {
                p = GeometryF.NormalLeft(curvePoints[i - 1], curvePoints[i + 1], angle);
            }

            spine[i] = p;
        }

        return spine;
    }

    /// <summary>
    /// return estimated curve distance
    /// </summary>
    /// <returns></returns>
    public float curveDistance()
    {
        float dist = 0;
        Vector3 p0 = anchor1;
        for (float i = 0; i <= 1; i += 0.1f)
        {
            Vector3 p1 = CubicBezierCurvePoint(i);
            dist += Vector3.Distance(p0, p1);
            p0 = p1;
        }

        return dist;
    }

    /// <summary>
    /// Returns a single point along this bezier curve given a float
    /// </summary>
    /// <param name="a1"></param>
    /// <param name="a2"></param>
    /// <param name="h1"></param>
    /// <param name="h2"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    private Vector3 CubicBezierCurvePoint(float t)
    {
        Vector3 p3 = Vector3.Lerp(handle1, handle2, t);
        return Vector3.Lerp(Vector3.Lerp(
            Vector3.Lerp(anchor1, handle1, t), p3, t),
            Vector3.Lerp(p3, Vector3.Lerp(handle2, anchor2, t), t),
            t);
    }

    /// <summary>
    /// Creates a new Bezier curve
    /// TODO
    /// - Tiller-Hanson algorithm
    /// - useful https://math.stackexchange.com/questions/465782/control-points-of-offset-bezier-curve
    /// - also useful https://raphlinus.github.io/curves/2022/09/09/parallel-beziers.html
    /// </summary>
    /// <param name="offset"></param>
    /// <returns>new CubicBezierCurve</returns>
    public CubicBezierCurve offsetCurve(float offset)
    {
        Vector3 left1 = GeometryF.NormalLeft(handle1 - anchor1, angle1);
        Vector3 left2 = GeometryF.NormalLeft(handle2 + anchor2, angle2);

        Vector3 a1 = anchor1 + left1 * offset;
        Vector3 a2 = anchor2 + left2 * offset;
        Vector3 h1 = handle1 + left1 * offset;
        Vector3 h2 = handle2 + left2 * offset;

        return new CubicBezierCurve(a1, a2, h1, h2, angle1, angle2);
    }
}
