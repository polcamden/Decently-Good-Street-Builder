using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using DecentlyGoodStreetBuilder.NodeTypes;

namespace DecentlyGoodStreetBuilder
{
    public class Node : StreetElement
    {
        public int ConnectionCount
        {
            get { return connections.Count; }
        }
        [SerializeField] private List<Node> connections = new List<Node>();
        public Node GetConnection(int i)
        {
            return connections[i];
        }

        [SerializeField] private List<Segment> connectionLinks = new List<Segment>();
        public Segment GetConnectionLink(int i)
        {
            return connectionLinks[i];
        }

        [SerializeField] private NodeType nodeType;
        public NodeType NodeType
        {
            get { return nodeType; }
        }

        const float HANDLE_SIZE = 0.5f;
        const float SELECTION_DISTANCE = 2f;


        public void Init(Vector3 position, StreetBuilder streetBuilder, ElementGroup elementGroup = null)
        {
            base.Init(streetBuilder, elementGroup);

            Position = position;
        }

        public override void Draw(string[] args)
        {
            Handles.color = Color.green;

            if (args.Contains<string>("selected"))
            {
                Handles.color = Color.red;
            }
            
            Handles.SphereHandleCap(0, Position, Quaternion.identity, HANDLE_SIZE, EventType.Repaint);
        }

        public override StreetElement[] Selected()
        {
            float cursorDistance = HandleUtility.DistanceToCircle(Position, HANDLE_SIZE);

            if (SELECTION_DISTANCE > cursorDistance)
            {
                return new StreetElement[] { this };
            }

            return null;
        }

        public override void OnPositionChange()
        {
            foreach (var segment in connectionLinks) { 
                segment.ConnectionNodePositionUpdate();
            }
        }

        /// <summary>
        /// Should only be called from Segment.Init
        /// </summary>
        /// <param name="node"></param>
        /// <param name="link"></param>
        public void AddConnection(Node node, Segment link)
        {
            connections.Add(node);
            connectionLinks.Add(link);


        }
    
        /// <summary>
        /// Should only be called form segment.OnDestroy
        /// </summary>
        /// <param name="link"></param>
        public void RemoveConnection(Segment link)
        {
            int i = connectionLinks.IndexOf(link);

            connections.RemoveAt(i);
            connectionLinks.RemoveAt(i);
        }

        /// <summary>
        /// Checks the connection count and sets new NodeType 
        /// </summary>
        private void CheckForNodeTypeChange()
        {
            if (ConnectionCount == 0)
            {
                //node
            }
            else if (ConnectionCount == 1) { 
                //End
            }
            else if(ConnectionCount == 2)
            {
                //curve or vector
            }
            else if(ConnectionCount >= 3)
            {
                //intersection
            }
        }
    }

}