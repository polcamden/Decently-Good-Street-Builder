using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
    /// <summary>
    /// Given to Nodes with 0 connections. 
    /// </summary>
    public class Disjointed : NodeType
    {
        public Disjointed(Node myNode) : base(myNode)
        {

        }

        public override void Draw()
        {
            throw new System.NotImplementedException();
        }

        public override void GenerateMesh()
        {
            throw new System.NotImplementedException();
        }

        public override void HandleUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}