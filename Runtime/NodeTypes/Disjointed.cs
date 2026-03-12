using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
    /// <summary>
    /// Given to Nodes with 0 connections. 
    /// </summary>
    public class Disjointed : NodeType
    {
        public override void Draw(string[] args)
        {
            base.Draw(args);
        }

        public override Mesh GenerateSurfaceMesh()
        {
            return null;
        }

        public override void HandleUpdate()
        {
            base.HandleUpdate();
            
			if (MyNode.ConnectionCount == 2)
			{
				Debug.LogError("Continuous is being used on a node that doesn't have 2 connections");
				return;
			}
		}
    }
}