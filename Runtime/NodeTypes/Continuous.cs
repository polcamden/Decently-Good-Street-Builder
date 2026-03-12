using System;
using System.Linq;
using DecentlyGoodStreetBuilder.Roadway;
using UnityEditor;
using UnityEngine;

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
            base.Draw(args);

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

                for (int i = 0; i < p1World.Length; i++)
                {
                    Handles.Label(p1World[i], $"p1[{i}]");
                    Handles.Label(p2World[i], $"p2[{i}]");
                }

                if(p1World != null && p2World != null)
                {
                    Vector3 p1LeftMid = p1World[0];
                    Vector3 p1RightMid = p1World[p1World.Length - 1];
                    Vector3 p2LeftMid = p2World[0];
                    Vector3 p2RightMid = p2World[p2World.Length - 1];

                    Handles.DrawDottedLine(p1LeftMid, p2LeftMid, 2);
                    Handles.DrawDottedLine(p1RightMid, p2RightMid, 2);

					leftTransition = Slider(p1LeftMid, p2LeftMid, leftTransition);
                    rightTransition = Slider(p1RightMid, p2RightMid, rightTransition);
				}

                //TESTING
                Vector3[] p1Ends = p1World;
                Vector3[] p2Ends = p2World;

				CubicBezierCurve[] curves = new CubicBezierCurve[4];

                int topEndsCount = Mathf.Max(p1Ends.Length, p2Ends.Length);
                for (int i = 0; i < topEndsCount; i++)
                {
                    int a = i * p1Ends.Length / topEndsCount;
                    int b = i * p2Ends.Length / topEndsCount;

                    Vector3 p1 = p1Ends[a];
                    Vector3 p2 = p2Ends[b];

                    float side = leftTransition;
                    if (i > topEndsCount / 2)
                    {
                        side = rightTransition;
                    }

                    Vector3 p1Handle = p1 + GetEndingNormal(0) * mergeDistance * side;
				    Vector3 p2Handle = p2 + GetEndingNormal(1) * mergeDistance * (1 - side);

                    curves[i] = new CubicBezierCurve(p1, p2, p1Handle, p2Handle);
                }

                for (int i = 0; i < curves.Length; i++)
                {
                    curves[i].DrawUnityBezier(new Color(((float)i) / 4f, 1 - (((float)i) / 4f), 0.5f));
                }
			}
		}

        public override Mesh GenerateSurfaceMesh()
        {
            if(MyNode.ConnectionCount != 2 || !merge)
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

            int topEndsCount = Mathf.Max(p1Ends.Length, p2Ends.Length);

            //make curves between endPoints
            CubicBezierCurve[] curves = new CubicBezierCurve[topEndsCount];
			
            for (int i = 0; i < topEndsCount; i++)
            {
                int a = i * p1Ends.Length / topEndsCount;
                int b = i * p2Ends.Length / topEndsCount;

                Vector3 p1 = p1Ends[a];
                Vector3 p2 = p2Ends[b];

                float side = leftTransition;
                if (i > topEndsCount / 2)
                {
                    side = rightTransition;
                }

                Vector3 p1Handle = p1 + GetEndingNormal(0) * mergeDistance * side;
                Vector3 p2Handle = p2 + GetEndingNormal(1) * mergeDistance * (1 - side);

                curves[i] = new CubicBezierCurve(p1, p2, p1Handle, p2Handle);
            }

            Vector3[] verts = new Vector3[topEndsCount * subDivision];
			int[] trigs = new int[verts.Length * 6];

            //add verts for each bezier curve
            for (int i = 0; i < curves.Length; i++)
            {
                Vector3[] curveVerts = curves[i].CurvePoints(subDivision);
                Array.Copy(curveVerts, 0, verts, i * subDivision, subDivision);
            }

            int trigI = 0;
            for (int i = 1; i < curves.Length; i++)
            {
                int ri = i * subDivision;
                for (int li = (i - 1) * subDivision; li < i * subDivision - 1; li++)
                {
                    trigs[trigI] = li;
                    trigs[trigI + 1] = ri;
                    trigs[trigI + 2] = li + 1;
                    
                    trigs[trigI + 3] = ri;
                    trigs[trigI + 4] = ri + 1;
                    trigs[trigI + 5] = li + 1;
                    
                    trigI += 6;

                    ri++;
                }
            }

            //Debug.Log("build");

            //add left verts to verts and know the starting index
            /*Vector3[] leftVert = curves[0].CurvePoints(subDivision);
            Array.Copy(leftVert, verts, leftVert.Length);
            int leftEdgeStart = 0;

            //add right verts to verts remember start index
            Vector3[] rightVert = curves[1].CurvePoints(subDivision);
            Array.Copy(rightVert, 0, verts, leftVert.Length, leftVert.Length);
            int rightEdgeStart = leftVert.Length;

            int trigI = 0;
            for (int i = 0; i < 1; i++)
            {
                int ri = rightEdgeStart; 
                for (int li = leftEdgeStart; li < rightEdgeStart - 1; li++)
                {
                    trigs[trigI] = li;
                    trigs[trigI + 1] = ri;
                    trigs[trigI + 2] = li + 1;
                    
                    trigs[trigI + 3] = ri;
                    trigs[trigI + 4] = ri + 1;
                    trigs[trigI + 5] = li + 1;
                    
                    trigI += 6;

                    ri++;
                }
            }*/

			Mesh mesh = new Mesh();
            mesh.vertices = verts;
            mesh.triangles = trigs;
			mesh.RecalculateNormals();

			return mesh;
        }

        public override void HandleUpdate()
        {
            base.HandleUpdate();

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
                    Vector3 pos = MyNode.Position;
                    float d1 = Vector3.Dot(pos, points[0]);
                    float d2 = Vector3.Dot(pos, points[points.Length-1]);
                    if (d1 < d2)
                    {
                        Array.Reverse(points);
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