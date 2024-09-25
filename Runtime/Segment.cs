using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace DecentlyGoodStreetBuilder
{
    public class Segment : StreetElement
    {
        [SerializeField] private Node[] connection;
        [SerializeField] private Vector3[] endPoints;
        [SerializeField] private Vector3 handle;

        public override Vector3 Position { get => base.Position; set { } }

        const float SELECTION_DISTANCE = 10;

        /// <summary>
        /// Sets Group and links the segment to two nodes
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="streetBuilder"></param>
        /// <param name="elementGroup"></param>
        public void Init(Node node1, Node node2, StreetBuilder streetBuilder, ElementGroup elementGroup = null)
        {
            base.Init(streetBuilder, elementGroup);

            //error check make sure node1 isnt already connected 
            //call add connection in both nodes
            bool prevConFound = false;
            for (int i = 0; i < node1.ConnectionCount; i++) { 
                Node n = node1.GetConnection(i);

                if (n == node2)
                {
                    prevConFound = true;
                }
            }

            if (prevConFound)
            {
                DestroyImmediate(this);
            }
            else
            {
                connection = new Node[2];
                connection[0] = node1; 
                connection[1] = node2;

                endPoints = new Vector3[2];

                node1.AddConnection(node2, this);
                node2.AddConnection(node1, this);
            }
        }

        /// <summary>
        /// Should not be use
        /// </summary>
        /// <param name="streetBuilder"></param>
        /// <param name="elementGroup"></param>
        public override void Init(StreetBuilder streetBuilder, ElementGroup elementGroup = null)
        {
            Debug.LogWarning("do not use base Init for segments");
            DestroyImmediate(this);
        }

        public override void Draw(string[] args)
        {
            Handles.color = Color.black;
            if (args.Contains<string>("selected"))
            {
                Handles.color = Color.red;
            }

            Handles.DrawLine(connection[0].Position, connection[1].Position, 3);
        }

        public override StreetElement[] Selected()
        {
            float cursorDistance = HandleUtility.DistanceToLine(connection[0].Position, connection[1].Position);

            if (SELECTION_DISTANCE > cursorDistance)
            {
                return new StreetElement[] { this, connection[0], connection[1] };
            }

            return null;
        }

        /// <summary>
        /// Called by a Node that has been moved. keeps the position of segment between the connections. 
        /// </summary>
        public void ConnectionNodePositionUpdate()
        {
            base.Position = Vector3.Lerp(connection[0].Position, connection[1].Position, 0.5f);
        }



        private void OnDestroy()
        {
            if (connection[0] != null && connection[1] != null)
            {
                connection[0].RemoveConnection(this);
                connection[1].RemoveConnection(this);
            }
        }
    }
}