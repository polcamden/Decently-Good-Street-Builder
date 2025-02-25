using UnityEngine;
using System.Collections.Generic;
using System;

namespace DecentlyGoodStreetBuilder.Roadway
{
    [CreateAssetMenu(menuName = "DG Street Builder/Mesh Array")]
    public class MeshArray : RoadwayPart, IRoadwayMesh
    {
		[SerializeField] public bool collidable;
		[SerializeField] public Material material;

		//TODO make private and make GUI for the cross section
		[SerializeField] public List<Vector2> meshCrossSection;
        [SerializeField] public bool[] sharpVerticies;
		[SerializeField] public float[] vAxis;
        [SerializeField] public float horizontalScale;

        private const float resolution = 1;

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

        public Mesh GenerateMesh(CubicBezierCurve baseCurve, RoadwayData data)
        {
            MeshArrayData castedData = (MeshArrayData)data;

            if (meshCrossSection.Count < 2)
            {
                return null;
            }

            //get curve points
            CubicBezierCurve offsetCurve = baseCurve.offsetCurve(data.offset);
            MeshArrayData castData = (MeshArrayData)data;

            (Vector3[] points, Vector3[] spine) = offsetCurve.curvePointsSpine(castData.resolution);

            Mesh mesh = new Mesh();

            //TODO roadway data
            int mirrorMultiplier = castedData.mirror ? -1 : 1;
            Vector3[] vertices = new Vector3[points.Length * meshCrossSection.Count];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[(points.Length - 1) * (meshCrossSection.Count - 1) * 6];
            int triangleCount = 0;
            float dist = 0;
            for (int i = 0; i < points.Length; i++)
            {

                if (i != 0)
                {
                    dist += Vector3.Distance(points[i - 1], points[i]);
                }

                for (int v = 0; v < meshCrossSection.Count; v++)
                {
                    int index = i * meshCrossSection.Count + v;

                    vertices[index] = (meshCrossSection[v].x * spine[i] * mirrorMultiplier) + (meshCrossSection[v].y * Vector3.up) + points[i];

                    //TODO UVS
                    uvs[index] = new Vector2(dist * horizontalScale, vAxis[v]);
                }

                if (i != 0)
                {
                    for (int y = 0; y < meshCrossSection.Count - 1; y++)
                    {
                        int start = i * meshCrossSection.Count + y;
                        if (start - (meshCrossSection.Count) < 0)
                        {
                            Debug.Log("asdasd");
                        }

                        if (castedData.mirror) 
                        {
                            triangles[triangleCount] = start;
                            triangles[triangleCount + 1] = start - (meshCrossSection.Count);
                            triangles[triangleCount + 2] = start - (meshCrossSection.Count) + 1;

                            triangles[triangleCount + 4] = start;
                            triangles[triangleCount + 3] = start + 1;
                            triangles[triangleCount + 5] = start - (meshCrossSection.Count) + 1;
                        }
                        else
                        {
                            triangles[triangleCount] = start;
                            triangles[triangleCount + 2] = start - (meshCrossSection.Count);
                            triangles[triangleCount + 1] = triangles[triangleCount + 2] + 1;

                            triangles[triangleCount + 3] = start;
                            triangles[triangleCount + 4] = start + 1;
                            triangles[triangleCount + 5] = triangles[triangleCount + 1];
                        }

                        triangleCount += 6;
                    }


                }
            }

            /*for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] -= segment.Position;
            }*/

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}