using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using DecentlyGoodStreetBuilder.NodeTypes;

namespace DecentlyGoodStreetBuilder
{
    public class Node : StreetElement
    {
        public override Vector3 Position { 
            get => base.Position;
            set => base.Position = value;
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

        public int ConnectionCount
        {
            get { return connections.Count; }
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
            CheckForNodeTypeChange();
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

        public override ISelectable[] Selected()
        {
            float cursorDistance = HandleUtility.DistanceToCircle(Position, HANDLE_SIZE);

            if (SELECTION_DISTANCE > cursorDistance)
            {
                return new ISelectable[] { this };
            }

            return null;
        }

        public override void OnPositionChange()
        {
            base.OnPositionChange();

            foreach (var segment in connectionLinks) { 
                segment.OnPositionChange();
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

            CheckForNodeTypeChange();
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

            CheckForNodeTypeChange();
        }

        /// <summary>
        /// Checks the connection count and sets new NodeType 
        /// </summary>
        private void CheckForNodeTypeChange()
        {
            if (ConnectionCount == 0 && (nodeType == null || nodeType.GetType() != typeof(Disjointed)))
            {
                nodeType = new Disjointed(this);
            }
            else if (ConnectionCount == 1 && (nodeType == null || nodeType.GetType() != typeof(EndPoint))) 
            {
                nodeType = new EndPoint(this);
            }
            else if(ConnectionCount == 2 && (nodeType == null || nodeType.GetType() != typeof(Continuous)))
            {
                nodeType = new Continuous(this);
            }
            else if(ConnectionCount >= 3 && (nodeType == null || nodeType.GetType() != typeof(Intersection)))
            {
                nodeType = new Intersection(this);
            }
        }

        public override void OnDestroy()
        {
            for (int i = 0; i < ConnectionCount; i++)
            {
                Debug.Log(i + " destroy");
                DestroyImmediate(connectionLinks[i]);
            }

            base.OnDestroy();
        }
    }
}