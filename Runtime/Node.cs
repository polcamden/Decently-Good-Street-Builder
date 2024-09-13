using System.Collections.Generic;
using UnityEngine;

namespace DecentlyGoodStreetBuilder
{
    public class Node : StreetElement
    {
        public List<Node> connections = new List<Node>();
        public List<Segment> connectionLinks = new List<Segment>();


        public override void Draw(bool isSelected)
        {
            
        }

        /// <summary>
        /// should only be called from Segment.Init
        /// </summary>
        /// <param name="node"></param>
        /// <param name="link"></param>
        public void AddConnection(Node node, Segment link)
        {

        }
    }

}