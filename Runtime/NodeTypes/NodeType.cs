using System;
using System.Collections.Generic;
using DecentlyGoodStreetBuilder.Roadway;
using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
    [System.Serializable]
    public abstract class NodeType : ScriptableObject
    {
        [SerializeField] private Node myNode;
        [SerializeField] private Material surfaceMaterial;

        [SerializeField] private long connectionId;
        [SerializeField] private List<NodeLineConnection> PartConnections = new List<NodeLineConnection>();
        [SerializeField] private List<NodeFillConnection> FillConnections= new List<NodeFillConnection>();

        public Node MyNode
        {
            get {  return myNode; }
        }

        public virtual void Init(Node myNode)
        {
			this.myNode = myNode;
		}

        public virtual void Draw(string[] args)
        {
            (Vector3[] endPoint, Matrix4x4[] transforms) = GetRelativeEnding();

            for (int i = 0; i < PartConnections.Count; i++)
            {
                PartConnections[i].Draw(MyNode.Position, endPoint, transforms);
            }
        }

        public virtual void HandleUpdate()
        {
            if(myNode.ConnectionCount != 0)
                SetConnectionHash();
            //TODO: search saved connections and auto load the connections

            UpdateConnectionsHandles();
        }

        /// <summary>
        /// Generates the road surface and mesh road parts
        /// </summary>
        /// <param name="mesh">MyNodes mesh</param>
        /// <returns></returns>
        public Material[] GenerateRoadwayMesh(Mesh mesh)
        {
            //TODO: add roadway list structure
            mesh.Clear();

            mesh = GenerateSurfaceMesh();
            /*if (mesh != null) TODO: commented out for mesh debuging
            {
                mesh.RecalculateBounds();
                mesh.Optimize();
            }*/

			Material[] materials = new Material[] { surfaceMaterial };

            return materials;
        }

		/// <summary>
		/// This will generate the road surface.
		/// </summary>
		/// <returns></returns>
		public abstract Mesh GenerateSurfaceMesh();

        private void SetConnectionHash()
        {
            // Probably a beter way to do this but this way makes sense
            int count = myNode.ConnectionCount;

            long[] orderedIds = new long[count]; 
            if(myNode.GetConnectionLink(0).Roadway != null)
            {
                orderedIds[0] = myNode.GetConnectionLink(0).Roadway.getId();
            }
            
            Vector3[] anchorNormals = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                Vector3 a = myNode.GetConnectionLink(i).GetEndPointsRelativeToNode(myNode).normalized;
                anchorNormals[i] = a;
            }
            
            for (int i = 1; i < count; i++) //order hashs by dot prod
            {
                Vector3 a1 = anchorNormals[i-1];
                int lowestI = -1;
                float lowestDot = 1;

                for (int j = 0; j < count; j++)
                {
                    if (j != i)
                    {
                        Vector3 a2 = anchorNormals[j];
                        float dot = Vector3.Dot(a1, a2);
                        float sign = Mathf.Sign(Vector3.Dot(Vector3.Cross(a1, a2), Vector3.up));
                        float signedDot = dot * sign;

                        if (signedDot <= lowestDot)
                        {
                            lowestDot = signedDot;
                            lowestI = j;
                        }
                    }
                }

                if(lowestI != -1 && myNode.GetConnectionLink(lowestI).Roadway != null)
                {
                    orderedIds[i] = myNode.GetConnectionLink(lowestI).Roadway.getId();
                }
                
            }

            int x = 0;
            while(x != count - 1) //find the true start 
            {
                long h1 = orderedIds[(count - x) % count];
                long h2 = orderedIds[(count - x - 1) % count];
                if (h1 == h2)
                {
                    x++;
                }
                else
                {
                    x = (count - x) % count;
                    break;
                }
            }

            Debug.Log($"offset {x}");
            
            long finalId = orderedIds[x % count];

            for (int i = 1; i < count; i++)
            {
                int trueI1 = (x + i) % count;
                int trueI2 = (x + i - 1) % count;

                long h1 = orderedIds[trueI1];
                long h2 = orderedIds[trueI2];

                unchecked{
                    if (h1 == h2)
                    {
                        finalId += h2;
                    }
                    else
                    {
                        finalId -= h2;
                    }
                }
            }

            if (connectionId != finalId)
            {
                Debug.Log($"Hash Change {finalId}");
            }

            connectionId = finalId;
        }

        private void UpdateConnectionsHandles()
        {
            

            /*for (int i = 0; i < PartConnections.Count; i++)
            {
                PartConnections[i].UpdateConnectionsHandles();
            }*/
        }

        /// <summary>
        /// returns the (endPoints[], endTransforms[]) of all endpoints going into myNode
        /// </summary>
        /// <returns></returns>
        private (Vector3[] ,Matrix4x4[]) GetRelativeEnding()
        {
            Vector3[] endpoints = new Vector3[MyNode.ConnectionCount];
            Matrix4x4[] transforms = new Matrix4x4[MyNode.ConnectionCount];
            Vector3 worldPos = MyNode.Position;
            for (int i = 0; i < endpoints.Length; i++)
            {
                Segment s = myNode.GetConnectionLink(i);
                endpoints[i] = s.GetEndPointsRelativeToNode(MyNode);
                transforms[i] = s.GetEndpointTransformMatrix(myNode);
            }

            return (endpoints, transforms);
        }

        /*private void defaultFill()
        {
            RoadwayPart part = myNode.GetConnectionLink(0).Roadway.FindPartByType(typeof(CarriagewayMesh));

            List<Tuple<Segment, Vector3, Vector3>> relativePositions = new List<Tuple<Segment, Vector3, Vector3>>();

            for (int i = 0; i < myNode.ConnectionCount; i++)
            {
                Segment s = myNode.GetConnectionLink(i);
                CarriagewayMeshData data = (CarriagewayMeshData)s.Roadway.FindDataByType(typeof(CarriagewayMeshData));
                Vector3 l = Vector3.left;
                Vector3 r = Vector3.right;
                if(data != null)
                {
                    l *= data.width / 2;
                    r *= -data.width / 2;
                }

                Tuple<Segment, Vector3, Vector3> tuple = new Tuple<Segment, Vector3, Vector3>(s, l, r);

                relativePositions.Add(tuple);
            }  

            //NodeFillConnection fill = new NodeFillConnection(relativePositions, part, );

            //PartConnections.Add();
        }*/
    }

    [System.Serializable]
    public class NodeLineConnection
    {
        [SerializeField] Tuple<int, Vector3>[] connections; // connection index, offset relative to end Matrix4x4
        [SerializeField] Vector3[] handles; // handle of curve relative to connection
        [SerializeField] bool autoHandle;
        [SerializeField] [Range(0f, 1f)] float autoHardness; // 0-1 
        [SerializeField] RoadwayPart part;
        [SerializeField] RoadwayData data;

        public NodeLineConnection(Tuple<int, Vector3> connection1, Tuple<int, Vector3> connection2, RoadwayPart part, RoadwayData data)
        {
            connections = new Tuple<int, Vector3>[] {connection1, connection2};
            autoHandle = true;
            this.part = part;
            this.data = data;
        }

        public void Draw(Vector3 nodePosition, Vector3[] endPoints, Matrix4x4[] endMatrix)
        {
            if (connections == null || connections.Length != 2)
            {
                return;
            }

            Vector3 s1 = connectionPoint(connections[0].Item2, endPoints[connections[0].Item1], endMatrix[connections[0].Item1]) + nodePosition;
            Vector3 s2 = connectionPoint(connections[1].Item2, endPoints[connections[1].Item1], endMatrix[connections[1].Item1]) + nodePosition;
            Vector3 h1 = s1 + handles[0] + nodePosition;
            Vector3 h2 = s2 + handles[1] + nodePosition;

            CubicBezierCurve curve = new CubicBezierCurve(s1, s2, h1, h2);

            curve.DrawUnityBezier(Color.blue);
        }

        public void HandleUpdate(Vector3[] endPoints, Matrix4x4[] endMatrix)
        {
            if (autoHandle)
            {
                Vector3 intersection;
                Vector3 s1 = connectionPoint(connections[0].Item2, endPoints[connections[0].Item1], endMatrix[connections[0].Item1]);
                Vector3 s2 = connectionPoint(connections[1].Item2, endPoints[connections[1].Item1], endMatrix[connections[1].Item1]);
                Vector3 dir1 = endMatrix[connections[0].Item1].MultiplyPoint(Vector3.forward);
                Vector3 dir2 = endMatrix[connections[2].Item1].MultiplyPoint(Vector3.forward);
                if(GeometryF.Vector3Intersection(out intersection, s1, dir1, s2, dir2))
                {
                    float dist1 = Vector3.Distance(s1, s2) * autoHardness;
                    float dist2 = Vector3.Distance(s1, s2) * autoHardness;
                    handles[0] = dir1 * dist1;
                    handles[1] = dir2 * dist2;
                }
                else
                {
                    float dist = Vector3.Distance(s1, s2) * autoHardness;
                    handles[0] = dir1 * dist;
                    handles[1] = dir2 * dist;
                }
            }
        }

        private Vector3 connectionPoint(Vector3 offset, Vector3 endPoint, Matrix4x4 endMatrix)
        {
            return endPoint + endMatrix.MultiplyPoint(offset);
        }
    }

    /// <summary>
    /// Fills area
    /// </summary>
    public class NodeFillConnection
    {
        List<Tuple<int, Vector3>> connections;
        RoadwayPart part;
        RoadwayData data;
    }
}