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
			//forward.Normalize();

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
        /// Takes a value from 0-1 returns a value from 0-1 along Cosine 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float EaseInOutCos(float t)
		{
			t *= Mathf.PI;
			return -Mathf.Cos(t) * 0.5f + 0.5f;
		}
	}
}