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
				RoadwayBlueprint r1 = s1.Roadway;
				RoadwayBlueprint r2 = s2.Roadway;
				CarriagewayMeshData data1 = (CarriagewayMeshData)r1.FindDataByType(typeof(CarriagewayMeshData));
				CarriagewayMeshData data2 = (CarriagewayMeshData)r2.FindDataByType(typeof(CarriagewayMeshData));

				if (data1 != null && data2 != null)
				{
                    Vector3 a1 = s1.GetEndPointWorldPosition(MyNode);
                    Vector3 a2 = s2.GetEndPointWorldPosition(MyNode);

                    Vector3 s1Normal = GeometryF.Normal(s1.GetEndPointWorldPosition(MyNode), s1.GetHandleWorldPosition(MyNode));
                    Vector3 s2Normal = GeometryF.Normal(s2.GetEndPointWorldPosition(MyNode), s2.GetHandleWorldPosition(MyNode));

                    Vector3 s1x, s1y;
                    (s1x, s1y) = GeometryF.GetOrthogonalPlane(s1Normal, s1.getAngle(MyNode));

					Vector3 s2x, s2y;
					(s2x, s2y) = GeometryF.GetOrthogonalPlane(s2Normal, s2.getAngle(MyNode));

					Vector2[] s1points = data1.CrossSectionPoints();
                    Vector2[] s2points = data2.CrossSectionPoints();

                    Vector3[] s1WorldPoints = GeometryF.Vector2sToPlane(s1points, s1x, s1y, a1);
					Vector3[] s2WorldPoints = GeometryF.Vector2sToPlane(s2points, s2x, s2y, a2);

					for (int i = 0; i < s1WorldPoints.Length; i++)
                    {
						Handles.SphereHandleCap(0, s1WorldPoints[i], Quaternion.identity, 0.1f, EventType.Repaint);
					}

					for (int i = 0; i < s2WorldPoints.Length; i++)
					{
						Handles.SphereHandleCap(0, s2WorldPoints[i], Quaternion.identity, 0.1f, EventType.Repaint);
					}

					//Vector3 leftHandlePos = Vector3.Lerp(s1WorldPoints[0], s2WorldPoints[3], leftTransition);

					leftTransition = Slider(s1WorldPoints[0], s2WorldPoints[3], leftTransition);


					float s1Dist = Vector3.Distance(s1WorldPoints[0], s2WorldPoints[3]);

                    Vector3 handle1 = s1WorldPoints[0] - s1Normal * s1Dist * leftTransition;
                    Vector3 handle2 = s2WorldPoints[3] - s2Normal * s1Dist * (1 - leftTransition);

                    Handles.DrawBezier(s1WorldPoints[0], s2WorldPoints[3], handle1, handle2, Color.white, null, 2);
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
			Segment s1 = MyNode.GetConnectionLink(connectionIndex);
            if(s1 != null && s1.Roadway != null)
            {
				CarriagewayMeshData data = (CarriagewayMeshData)s1.Roadway.FindDataByType(typeof(CarriagewayMeshData));

                if (data != null)
                {
					//Vector3 anchor = s1.GetEndPointsRelativeToNode(MyNode);
                    //Vector3 handle = s1.Ge;

				}
			}

			return null;
        }
    }
}