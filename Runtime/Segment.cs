using System.Collections.Generic;
using UnityEngine;

namespace DecentlyGoodStreetBuilder
{
    public class Segment : StreetElement
    {
        [SerializeField] private Node[] connection;
        [SerializeField] private Vector3 handle;

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

        public override void Draw(bool isSelected)
        {
            
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