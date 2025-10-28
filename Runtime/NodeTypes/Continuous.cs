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

                Debug.Log(p1World);

                if(p1World != null && p2World != null)
                {
                    Vector3 p1LeftMid = Vector3.Lerp(p1World[0], p1World[1], 0.5f);
                    Vector3 p1RightMid = Vector3.Lerp(p1World[2], p1World[3], 0.5f);
                    Vector3 p2LeftMid = Vector3.Lerp(p2World[2], p2World[3], 0.5f);
                    Vector3 p2RightMid = Vector3.Lerp(p2World[0], p2World[1], 0.5f);

					leftTransition = Slider(p1LeftMid, p2LeftMid, leftTransition);
                    rightTransition = Slider(p1RightMid, p2RightMid, rightTransition);

					float s1Dist = Vector3.Distance(p1World[0], p2World[3]);
				}
			}
		}

        public override void GenerateMesh()
        {
            if(MyNode.ConnectionCount != 2)
                return;

			Segment s1 = MyNode.GetConnectionLink(0);
			Segment s2 = MyNode.GetConnectionLink(1);

            Mesh mesh = null;

            if (s1.Roadway != null && s2.Roadway != null && s1.Roadway != s2.Roadway)
            {
                RoadwayBlueprint r1 = s1.Roadway;
				RoadwayBlueprint r2 = s2.Roadway;
                CarriagewayMeshData data1 = (CarriagewayMeshData)r1.FindDataByType(typeof(CarriagewayMeshData));
                CarriagewayMeshData data2 = (CarriagewayMeshData)r2.FindDataByType(typeof(CarriagewayMeshData));

                if(data1 != null && data2 != null)
                {
					mesh = GenerateMerge(data1, data2);
				}
			}
            

        }

        public Mesh GenerateMerge(CarriagewayMeshData data1, CarriagewayMeshData data2)
        {
            return null;
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

                    return points;
				}
			}

			return null;
        }
    }
}