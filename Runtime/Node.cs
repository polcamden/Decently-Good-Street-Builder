using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine;

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


        const float HandleSize = 0.5f;


        public void Init(Vector3 position, StreetBuilder streetBuilder, ElementGroup elementGroup = null)
        {
            base.Init(streetBuilder, elementGroup);

            Position = position;
        }

        public override void Draw(bool isSelected)
        {
            Handles.color = Color.green;
            Handles.SphereHandleCap(0, Position, Quaternion.identity, HandleSize, EventType.Repaint);
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
    }

}