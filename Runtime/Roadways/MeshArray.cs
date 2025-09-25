using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using NUnit.Framework.Interfaces;

namespace DecentlyGoodStreetBuilder.Roadway
{
    [CreateAssetMenu(menuName = "Decently Good Street Builder/Mesh Array", order = 50)]
    public class MeshArray : RoadwayPart, IRoadwayMesh
    {
		[SerializeField] public bool collidable;
		[SerializeField] public Material material;

		//TODO make private and make GUI for the cross section
		[SerializeField] public List<Vector2> meshCrossSection;
        [SerializeField] public bool[] sharpVerticies;
		[SerializeField] public float[] vAxis;
        [SerializeField] public float uScale;

		public int Count
		{
			get { return meshCrossSection.Count; }
		}

		public bool Collidable
		{
			get{ return collidable; }
		}

		public Material Material
		{
			get{ return material; }
		}

        public override Type DataClass
        {
            get { return typeof(MeshArrayData); }
        }

        public virtual Mesh GenerateMesh(CubicBezierCurve baseCurve, RoadwayData data)
        {
            MeshArrayData castedData = (MeshArrayData)data;

            if (meshCrossSection.Count < 2)
            {
                return null;
            }

            //get curve points
            CubicBezierCurve offsetCurve = baseCurve.offsetCurve(data.offset);

			//TODO make vaxis and sharpVerts flip when mirrored 
			Vector2[] cross = meshCrossSection.ToArray();

            if (castedData.mirror)
            {
                for (int i = 0; i < cross.Length; i++)
                {
                    cross[meshCrossSection.Count - 1 - i] = new Vector2(-meshCrossSection[i].x, meshCrossSection[i].y);
                }
            }

            return GenerateMeshGivenSlice(offsetCurve, castedData.resolution, cross, sharpVerticies, vAxis, uScale);
        }

        /// <summary>
        /// Generates a mesh by placing a meshCrossSection along a bezier curve. 
        /// </summary>
        /// <param name="meshCrossSection">Array of vertices</param>
        /// <param name="sharpVerticies">Which vertices are sharp edges</param>
        /// <param name="vAxis">The v-axis of the uv for vertices</param>
        /// <param name="uScale">The u-axis of the uv along the bezier curve</param>
        /// <returns>returns a mesh</returns>
        public static Mesh GenerateMeshGivenSlice(CubicBezierCurve curve, float resolution, Vector2[] meshCrossSection, bool[] sharpVerticies, float[] vAxis, float uScale)
        {

			(Vector3[] points, Vector3[] spine) = curve.curvePointsSpine(resolution);

			Vector3[] vertices = new Vector3[points.Length * meshCrossSection.Length];
			Vector2[] uvs = new Vector2[vertices.Length];
			int[] triangles = new int[(points.Length - 1) * (meshCrossSection.Length - 1) * 6];
			int triangleCount = 0;
			float dist = 0;

			for (int i = 0; i < points.Length; i++)
			{
				if (i != 0)
				{
					dist += Vector3.Distance(points[i - 1], points[i]);
				}

				for (int v = 0; v < meshCrossSection.Length; v++)
				{
					int index = i * meshCrossSection.Length + v;

					vertices[index] = (meshCrossSection[v].x * spine[i]) + (meshCrossSection[v].y * Vector3.up) + points[i];

					//TODO UVS
					uvs[index] = new Vector2(dist * uScale, vAxis[v]);
				}

				if (i != 0)
				{
					for (int y = 0; y < meshCrossSection.Length - 1; y++)
					{
						int start = i * meshCrossSection.Length + y;

						triangles[triangleCount] = start;
						triangles[triangleCount + 2] = start - (meshCrossSection.Length);
						triangles[triangleCount + 1] = triangles[triangleCount + 2] + 1;

						triangles[triangleCount + 3] = start;
						triangles[triangleCount + 4] = start + 1;
						triangles[triangleCount + 5] = triangles[triangleCount + 1];

						triangleCount += 6;
					}


				}
			}

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;
			mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
			return mesh;
        }
    }
}