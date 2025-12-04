using System;
using System.Linq;
using DecentlyGoodStreetBuilder.Roadway;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
    /// <summary>
    /// Given to Nodes with 2 Connections. 
    /// </summary>
    public class Continuous : NodeType
    {
        [SerializeField] float mergeDistance = 7;
        [SerializeField] bool merge = true;

        [Range(0f, 1f)]
        [SerializeField] float leftTransition = 0.5f;
		[Range(0f, 1f)]
		[SerializeField] float rightTransition = 0.5f;

        [SerializeField] int subDivision = 16;

		public override void Draw(string[] args)
        {
            if (!args.Contains<string>("selected"))
                return;

			if (MyNode.ConnectionCount != 2)
				return;

            Handles.color = Color.blue;

			Segment s1 = MyNode.GetConnectionLink(0);
			Segment s2 = MyNode.GetConnectionLink(1);

			if (s1.Roadway != null && s2.Roadway != null && s1.Roadway != s2.Roadway)
			{
                Vector3[] p1World = GetEndingVerts(0);
                Vector3[] p2World = GetEndingVerts(1);

                if(p1World != null && p2World != null)
                {
                    Vector3 p1LeftMid = p1World[2];
                    Vector3 p1RightMid = p1World[3];
                    Vector3 p2LeftMid = p2World[3];
                    Vector3 p2RightMid = p2World[2];

                    Handles.DrawDottedLine(p1LeftMid, p2LeftMid, 2);
                    Handles.DrawDottedLine(p1RightMid, p2RightMid, 2);

					leftTransition = Slider(p1LeftMid, p2LeftMid, leftTransition);
                    rightTransition = Slider(p1RightMid, p2RightMid, rightTransition);
				}


                //TESTING
                Vector3[] p1Ends = p1World;
                Vector3[] p2Ends = p2World;

				CubicBezierCurve[] curves = new CubicBezierCurve[4];

				Vector3 p1NormalPush = GetEndingNormal(0) * mergeDistance / 2;
				Vector3 p2NormalPush = GetEndingNormal(1) * mergeDistance / 2;

				for (int i = 0; i < 2; i += 2)
				{
					Vector3 leftP1 = i >= p1Ends.Length ? p1Ends[p1Ends.Length - 1] : p1Ends[i];
					Vector3 leftP2 = i >= p2Ends.Length ? p2Ends[p1Ends.Length - 1] : p2Ends[i];
					Vector3 rightP1 = i + 1 >= p1Ends.Length ? p2Ends[p1Ends.Length - 1] : p2Ends[i + 1];
					Vector3 rightP2 = i + 1 >= p2Ends.Length ? p2Ends[p1Ends.Length - 1] : p2Ends[i + 1];

					curves[i] = new CubicBezierCurve(leftP1, leftP2, leftP1 + p1NormalPush, leftP2 + p2NormalPush);
					curves[i + 1] = new CubicBezierCurve(rightP1, rightP2, rightP1 + p1NormalPush, rightP2 + p2NormalPush);
				}

                for (int i = 0; i < curves.Length; i++)
                {
                    curves[i].DrawUnityBezier(new Color(((float)i) / 4f, 1 - (((float)i) / 4f), 0.5f));
                }
			}
		}

        public override Mesh GenerateSurfaceMesh()
        {
            if(MyNode.ConnectionCount != 2)
                return null;

			Vector3[] p1Ends = GetEndingVerts(0);
			Vector3[] p2Ends = GetEndingVerts(1);

            if (p1Ends == null || p2Ends == null)
                return null;

            ///Translate ends from world to local
            Vector3 nodePos = MyNode.Position;
            for (int i = 0; i < p1Ends.Length; i++)
            {
                p1Ends[i] = p1Ends[i] - nodePos;
            }
            for (int i = 0; i < p2Ends.Length; i++)
            {
				p2Ends[i] = p2Ends[i] - nodePos;
			}

            int topEndsCount = 4;
            if (p1Ends.Length == 2 && p2Ends.Length == 2)
            {
                topEndsCount = 2;
            }

            //make curves between endPoints
            CubicBezierCurve[] curves = new CubicBezierCurve[topEndsCount];
			Segment s1 = MyNode.GetConnectionLink(0);
			Segment s2 = MyNode.GetConnectionLink(1);

			Vector3 p1NormalPush = GetEndingNormal(0) * mergeDistance / 2;
            Vector3 p2NormalPush = GetEndingNormal(1) * mergeDistance / 2;

			for (int i = 0; i < curves.Length; i+=2)
            {
                Vector3 leftP1  = i >= p1Ends.Length   ? p1Ends[p1Ends.Length - 1] : p1Ends[i];
                Vector3 leftP2  = i >= p1Ends.Length   ? p2Ends[p1Ends.Length - 1] : p2Ends[i];
                Vector3 rightP1 = i+1 >= p1Ends.Length ? p2Ends[p1Ends.Length - 1] : p2Ends[i+1];
				Vector3 rightP2 = i+1 >= p1Ends.Length ? p2Ends[p1Ends.Length - 1] : p2Ends[i+1];

                curves[i] = new CubicBezierCurve(leftP1, leftP2, leftP1 + p1NormalPush, leftP2 + p2NormalPush);
                curves[i+1] = new CubicBezierCurve(rightP1, rightP2, rightP1 + p1NormalPush, rightP2 + p2NormalPush);
            }



			Vector3[] verts = new Vector3[topEndsCount * subDivision];
			int[] trigs = new int[verts.Length * 6];




			Mesh mesh = new Mesh();
            mesh.vertices = verts;
            mesh.triangles = trigs;
			mesh.RecalculateNormals();

			return mesh;
        }

        public override void HandleUpdate()
        {
            if (MyNode.ConnectionCount != 2)
            {
				Debug.LogError("Continuous is being used on a node that doesn't have 2 connections");
                return;
			}
            
            Node c1 = MyNode.GetConnection(0);
            Node c2 = MyNode.GetConnection(1);
            Segment s1 = MyNode.GetConnectionLink(0);
            Segment s2 = MyNode.GetConnectionLink(1);

            if (s1.Roadway != null && s2.Roadway != null && s1.Roadway != s2.Roadway) //causes automerge to be made
            {
				Vector3 mergeDir = GeometryF.Normal(c2.Position, c1.Position) * mergeDistance / 2;
                s1.SetEndPointRelativeToNode(MyNode, mergeDir);
                s2.SetEndPointRelativeToNode(MyNode, -mergeDir);
            }
            else
            {
				s1.SetEndPointRelativeToNode(MyNode, Vector3.zero);
				s2.SetEndPointRelativeToNode(MyNode, Vector3.zero);
			}
        }

        public float Slider(Vector3 start, Vector3 end, float t)
        {
            Vector3 sliderPos = Vector3.Lerp(start, end, t);

			EditorGUI.BeginChangeCheck();
			sliderPos = Handles.Slider(sliderPos, (end - start).normalized, 0.2f, Handles.SphereHandleCap, 0.1f);
			if (EditorGUI.EndChangeCheck())
			{
				float lineLength = Vector3.Distance(start, end);
				return Mathf.Clamp01(Vector3.Dot(sliderPos - start, (end - start).normalized) / lineLength);
			}

            return t;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionIndex"></param>
        /// <returns>reutrns array where the first pair of index 0 and 1 are the road surface</returns>
        public Vector3[] GetEndingVerts(int connectionIndex)
        {
			Segment s = MyNode.GetConnectionLink(connectionIndex);
            if(s != null && s.Roadway != null)
            {
				CarriagewayMeshData data = (CarriagewayMeshData)s.Roadway.FindDataByType(typeof(CarriagewayMeshData));

                if (data != null)
                {
                    Vector2[] endPointsPlane = data.CrossSectionPoints();

					Vector3 anchor = s.GetEndPointWorldPosition(MyNode);
                    Vector3 handle = GeometryF.Normal(s.GetEndPointWorldPosition(MyNode), s.GetHandleWorldPosition(MyNode));
                    float angle = s.getAngle(MyNode);

                    Matrix4x4 transform = GeometryF.OrthogonalToTransform(anchor, handle, angle);

                    Vector3[] points = GeometryF.Vector2sToPlane(endPointsPlane, transform);

                    //flips order
                    for (int i = 0; i < points.Length; i += 2)
                    {
                        Vector3 temp = points[i];
                        points[i] = points[i + 1];
                        points[i + 1] = temp;
                    }

                    return points;
				}
			}

			return null;
        }
    
        /// <summary>
        /// points into node
        /// </summary>
        /// <returns></returns>
        public Vector3 GetEndingNormal(int connectionIndex)
        {
			Segment s = MyNode.GetConnectionLink(connectionIndex);

            return -GeometryF.Normal(s.GetEndPointWorldPosition(MyNode), s.GetHandleWorldPosition(MyNode));

		}
    }
}