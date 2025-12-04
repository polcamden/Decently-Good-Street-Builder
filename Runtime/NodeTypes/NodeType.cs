using Unity.VisualScripting;
using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
    [System.Serializable]
    public abstract class NodeType : ScriptableObject
    {
        [SerializeField] private Node myNode;
        [SerializeField] private Material surfaceMaterial;

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

        /// <summary>
        /// Generates the road surface and mesh road parts
        /// </summary>
        /// <param name="mesh">MyNodes mesh</param>
        /// <returns></returns>
        public (Mesh, Material[]) GenerateRoadwayMesh(Mesh mesh)
        {
            //TODO: add roadway list structure
            mesh.Clear();

            mesh = GenerateSurfaceMesh();
            if (mesh != null)
            {
                mesh.RecalculateBounds();
                mesh.Optimize();
            }

			Material[] materials = new Material[] { surfaceMaterial };

            return (mesh, materials);
        }

		/// <summary>
		/// This will generate the road surface.
		/// </summary>
		/// <returns></returns>
		public abstract Mesh GenerateSurfaceMesh();
    }
}