using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
    /// <summary>
    /// Given to Nodes who have 3 or more connections. 
    /// </summary>
    public class Intersection : NodeType
    {
        public Intersection(Node myNode) : base(myNode)
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