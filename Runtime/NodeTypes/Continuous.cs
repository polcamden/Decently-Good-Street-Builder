using DecentlyGoodStreetBuilder.Roadway;
using Unity.VisualScripting;
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

        public override void Draw()
        {
			if (MyNode.ConnectionCount != 2)
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

				if (data1 != null && data2 != null)
				{
                    //Vector3 mergeNormal = GeometryF.Normal(s1.GetHandleWorldPosition(MyNode), s2.GetHandleWorldPosition(MyNode));

                    Vector3 a1 = s1.GetEndPointsWorldPosition(MyNode);
                    Vector3 a2 = s1.GetEndPointsWorldPosition(MyNode);

                    Vector3 s1Normal = GeometryF.Normal(s1.GetEndPointsWorldPosition(MyNode), s1.GetHandleWorldPosition(MyNode));
                    Vector3 s2Normal = GeometryF.Normal(s2.GetEndPointsWorldPosition(MyNode), s2.GetHandleWorldPosition(MyNode));

                    float dist = Vector3.Distance();

                    Vector2[] r1points = data1.CrossSectionPoints();

                    Handles.DrawBezier(data1.);
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
    }
}