using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
    /// <summary>
    /// Given to Nodes with 1 Connections. 
    /// </summary>
    public class EndPoint : NodeType
    {
        public EndPoint(Node myNode) : base(myNode)
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