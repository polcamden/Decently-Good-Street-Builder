using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
    [System.Serializable]
    public abstract class NodeType
    {
        private NodeType myNode;

        public abstract void HandleUpdate(); 

        public abstract void GenerateMesh(); 
    }
}