using Unity.VisualScripting;
using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
    [System.Serializable]
    public abstract class NodeType : ScriptableObject
    {
        [SerializeField] private Node myNode;

        public Node MyNode
        {
            get {  return myNode; }
        }

        public virtual void Init(Node myNode)
        {
			this.myNode = myNode;
		}

        public abstract void Draw(string[] args);

        public abstract void HandleUpdate(); 

        public abstract void GenerateMesh();
    }
}