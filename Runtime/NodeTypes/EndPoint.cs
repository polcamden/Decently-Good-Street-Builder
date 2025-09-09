using Unity.VisualScripting;
using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
    /// <summary>
    /// Given to Nodes with 1 Connections. 
    /// </summary>
    public class EndPoint : NodeType
    {
        public override void Draw()
        {
            
        }

        public override void GenerateMesh()
        {
            
        }

        public override void HandleUpdate()
        {
			if (MyNode.ConnectionCount != 1)
			{
				Debug.LogError("EndPoint is being used on a node that doesn't have 1 connections");
                return;
			}


		}
    }
}