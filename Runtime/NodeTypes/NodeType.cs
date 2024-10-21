using Unity.VisualScripting;
using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
    [System.Serializable]
    public abstract class NodeType
    {
        private Node myNode;

        public NodeType(Node myNode)
        {
            this.myNode = myNode;
        }

        public abstract void Draw();

        public abstract void HandleUpdate(); 

        public abstract void GenerateMesh();
    }
}