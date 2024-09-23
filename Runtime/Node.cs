using System.Collections.Generic;
using System.Linq;
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

        public override bool Selected()
        {
            float cursorDistance = HandleUtility.DistanceToCircle(Position, HANDLE_SIZE);

            if (SELECTION_DISTANCE > cursorDistance)
            {
                return true;
            }

            return false;
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