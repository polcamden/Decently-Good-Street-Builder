using System.Collections.Generic;
using UnityEngine;

namespace DecentlyGoodStreetBuilder
{
	public static class GeometryF
	{
		/// <summary>
		/// returns a Vector3 that is one unit towards the two points 
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static Vector3 Normal(Vector3 from, Vector3 to)
		{
			Vector3 forward = to - from;

			forward.Normalize();

			return forward;
		}

		/// <summary>
		/// takes a normal returns the left direction
		/// </summary>
		/// <param name="normal"></param>
		/// <returns></returns>
		public static Vector3 NormalLeft(Vector3 normal, float angle)
		{
			Quaternion flatLeft = Quaternion.AngleAxis(90, Vector3.up);
			Vector3 flatForward = new Vector3(normal.x, 0, normal.z);
			Vector3 left = flatLeft * flatForward;

			Quaternion banking = Quaternion.AngleAxis(angle, normal);
			return Vector3.Normalize(banking * left);
		}

		/// <summary>
		/// takes a normal returns the left direction
		/// </summary>
		/// <param name="normal"></param>
		/// <returns></returns>
		public static Vector3 NormalLeft(Vector3 from, Vector3 to, float angle)
		{
			//forward.Normalize();
			return NormalLeft(Normal(from, to), angle);
		}

		/// <summary>
		/// reterns the intersection of two vectors
		/// </summary>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <param name="h1"></param>
		/// <param name="h2"></param>
		/// <returns></returns>
        public static Vector2 Vector2Intersection(Vector2 a1, Vector2 a2, Vector2 h1, Vector2 h2)
        {
            float slope1 = -h1.y / -h1.x;
            float slope2 = -h2.y / -h2.x;

            //gets intersection of two lines on a 2D graph then puts it in the flat corners array
            float x = (-slope2 * a2.x + a2.y - a1.y + slope1 * a1.x) / (slope1 - slope2);
            float y = slope1 * (x - a1.x) + a1.y;

            Vector2 intersection = new Vector2();
            bool error = false;
            if (float.IsNaN(x) || float.IsNaN(y) || float.IsInfinity(x) || float.IsInfinity(y))
            {
                error = true;
            }
            else
            {
                intersection = new Vector2(x, y);
            }

            if (error)
            {
                Vector2 cunt = Vector2.Lerp(a1, a2, 0.5f);
                intersection = cunt;
            }

            return intersection;
        }

        public static bool Vector3Intersection(out Vector3 intersection, Vector3 linePoint1,
        Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {
            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parallel
            if (Mathf.Abs(planarFactor) < 0.0001f
                    && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2)
                        / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        /// <summary>
        /// Takes anchors and handles for a BezierCurve returns a Array of points along the BezierCurve. equal distances
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="h1"></param>
        /// <param name="h2"></param>
        /// <param name="resolution">in unity units</param>
        /// <returns></returns>
        public static Vector3[] CubicBezierCurvePoints(Vector3 a1, Vector3 a2, Vector3 h1, Vector3 h2, float resolution)
		{
			//get length
			float curveDist = 0;
			Vector3 prevPoint = a1;
			for (int i = 1; i < 50; i++)
			{
				Vector3 point = CubicBezierCurvePoint(a1, a2, h1, h2, i / 50f);
				curveDist += Vector3.Distance(prevPoint, point);
				prevPoint = point;
			}

			//calcs the new resolution so theres no uneven points
			float fixedResolution = (curveDist % resolution) / (int)(curveDist / resolution) + resolution;

			//find points
			List<Vector3> points = new List<Vector3>
			{
				a1
			};

			for (int i = 0; i < 1000; i++)
			{
				Vector3 p = CubicBezierCurvePoint(a1, a2, h1, h2, i / 1000f);
				if (Vector3.Distance(points[points.Count - 1], p) >= fixedResolution)
				{
					points.Add(p);
				}
			}

			points.RemoveAt(points.Count - 1);
			points.Add(a2);

			return points.ToArray();
		}

		/// <summary>
		/// Take anchors and handles for a bezierCurve return a Array of points along the bezier curve no equal
		/// </summary>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <param name="h1"></param>
		/// <param name="h2"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		/*public static Vector3[] CubicBezierCurvePoints(Vector3 a1, Vector3 a2, Vector3 h1, Vector3 h2, int length)
		{
			Vector3[] points = new Vector3[length];

			for (int i = 0; i < length; i++)
			{
				points[i] = CubicBezierCurvePoint(a1, a2, h1, h2, i / (float)(length-1));
			}

			return points;
		}*/

        /// <summary>
        /// takes anchors and handles for a BezierCurve returns a single point on a bezier curve
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="h1"></param>
        /// <param name="h2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 CubicBezierCurvePoint(Vector3 a1, Vector3 a2, Vector3 h1, Vector3 h2, float t)
		{
			Vector3 p3 = Vector3.Lerp(h1, h2, t);
			return Vector3.Lerp(Vector3.Lerp(
				Vector3.Lerp(a1, h1, t), p3, t),
				Vector3.Lerp(p3, Vector3.Lerp(h2, a2, t), t),
				t);
		}

		/// <summary>
		/// returns the distance of a bezier curve
		/// </summary>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <param name="h1"></param>
		/// <param name="h2"></param>
		/// <returns></returns>
		public static float CubicBezierCurveDistance(Vector3 a1, Vector3 a2, Vector3 h1, Vector3 h2)
		{
			float dist = 0;
			Vector3 p0 = a1;
			for (float i = 0; i <= 1; i += 0.1f)
			{
				Vector3 p1 = CubicBezierCurvePoint(a1, a2, h1, h2, i);
				dist += Vector3.Distance(p0, p1);
				p0 = p1;
			}

			return dist;
		}

        /// <summary>
        /// returns the distance of a bezier curve
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="h1"></param>
        /// <param name="h2"></param>
        /// <returns></returns>
        public static float QuadraticBezierCurveDistance(Vector3 a1, Vector3 a2, Vector3 h, float handleInfluence)
        {
            Vector3 h1 = Vector3.Lerp(a1, h, handleInfluence);
            Vector3 h2 = Vector3.Lerp(a2, h, handleInfluence);

            return CubicBezierCurveDistance(a1, a2, h1, h2);
        }

        /// <summary>
        /// Quadratic bezier curve with equally spaced Point
        /// </summary>
        /// <param name="a1">end point 1</param>
        /// <param name="a2">end point 2</param>
        /// <param name="h">handle</param>
        /// <param name="resolution">spacing between points </param>
        /// <param name="handleInfluence">1-0 specifies the weight of the handle, h</param>
        /// <returns>Array of points using Bezier curve formula</returns>
        public static Vector3[] QuadraticBezierCurvePoints(Vector3 a1, Vector3 a2, Vector3 h, float resolution, float handleInfluence)
		{
			Vector3 h1 = Vector3.Lerp(a1, h, handleInfluence);
			Vector3 h2 = Vector3.Lerp(a2, h, handleInfluence);

			return CubicBezierCurvePoints(a1, a2, h1, h2, resolution);
		}

		/// <summary>
		/// Returns the spine normals given the curves points, handle 1 and 2, 
		/// </summary>
		/// <param name="points"></param>
		/// <param name="h1"></param>
		/// <param name="h2"></param>
		/// <param name="startDegree">degree at points[0]</param>
		/// <param name="endDegree">degree at points[points.length]</param>
		/// <returns></returns>
		public static Vector3[] BezierCurveToSpine(Vector3[] points, Vector3 h1, Vector3 h2, float startDegree = 0, float endDegree = 0)
		{
			Vector3[] spine = new Vector3[points.Length];

			for (int i = 0; i < points.Length; i++)
			{
				Vector3 p;
				float angle = Mathf.Lerp(startDegree, endDegree, (float)i / points.Length);

                if (i == 0) //start
                {
                    p = NormalLeft(points[i], h1, startDegree);
                }
                else if (i == points.Length - 1) //end
                {
                    p = NormalLeft(h2, points[i], endDegree);
                }
                else //middle points
                {
                    p = NormalLeft(points[i - 1], points[i + 1], angle);
                }

				spine[i] = p;
            }

			return spine;
		}

		/*public static (Vector3, Vector3, Vector3, Vector3) OffsetBezierCurve(Vector3 a1, Vector3 a2, Vector3 h1, Vector3 h2, float offset)
		{
			Vector3 newH1;
			Vector3 newH2;
			Vector3 newA1;
			Vector3 newA2;

			return (newA1, newA2, newH1, newH2);
		}*/

        /// <summary>
        /// Takes a value from 0-1 returns a value from 0-1 along Cosine 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float EaseInOutCos(float t)
		{
			t *= Mathf.PI;
			return -Mathf.Cos(t) * 0.5f + 0.5f;
		}
	
		/// <summary>
		/// normal will be the z-axis, x-axis is NormalLeft, y-axis is cross of z and x
		/// </summary>
		/// <param name="normal"></param>
		/// <param name="angle"></param>
		/// <returns>x-axis, y-axis</returns>
		public static Matrix4x4 GetOrthogonalPlane(Vector3 position, Vector3 normal, float angle)
		{
			Vector4 x = NormalLeft(normal, angle);
			Vector4 y = Vector3.Cross(normal, x);
			Vector4 z = normal;
			Vector4 w = new Vector4(position.x, position.y, position.z, 1);

			Matrix4x4 transform = new Matrix4x4(x, y, z, w);

			return transform;
		}

		/// <summary>
		/// Places points on xy-plane positions at origin
		/// </summary>
		/// <param name="points"></param>
		/// <param name="xAxis">normalized</param>
		/// <param name="yAxis">normalized othogonal to xAxis</param>
		/// <param name="origin">Origin of x-y plane</param>
		/// <returns></returns>
		public static Vector3[] Vector2sToPlane(Vector2[] points, Matrix4x4 transform)
		{
			Vector3[] p = new Vector3[points.Length];

			for (int i = 0; i < points.Length; i++)
			{
				Vector3 point = new Vector3(points[i].x, points[i].y, 0);

				p[i] = transform.MultiplyPoint3x4(point);
			}

			return p;
		}
	}
}