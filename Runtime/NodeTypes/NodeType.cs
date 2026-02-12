using System;
using Unity.VisualScripting;
using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
    [System.Serializable]
    public abstract class NodeType : ScriptableObject
    {
        [SerializeField] private Node myNode;
        [SerializeField] private Material surfaceMaterial;

        [SerializeField] private long connectionId;
        //[SerializeField] private Tuple<>

        public Node MyNode
        {
            get {  return myNode; }
        }

        public virtual void Init(Node myNode)
        {
			this.myNode = myNode;
		}

        public abstract void Draw(string[] args);

        public virtual void HandleUpdate()
        {
            if(myNode.ConnectionCount != 0)
                SetConnectionHash();
            //TODO: search saved connections and auto load the connections
        }

        /// <summary>
        /// Generates the road surface and mesh road parts
        /// </summary>
        /// <param name="mesh">MyNodes mesh</param>
        /// <returns></returns>
        public Material[] GenerateRoadwayMesh(Mesh mesh)
        {
            //TODO: add roadway list structure
            mesh.Clear();

            mesh = GenerateSurfaceMesh();
            /*if (mesh != null) TODO: commented out for mesh debuging
            {
                mesh.RecalculateBounds();
                mesh.Optimize();
            }*/

			Material[] materials = new Material[] { surfaceMaterial };

            return materials;
        }

		/// <summary>
		/// This will generate the road surface.
		/// </summary>
		/// <returns></returns>
		public abstract Mesh GenerateSurfaceMesh();

        private void SetConnectionHash()
        {
            // Probably a beter way to do this but this way makes sense
            int count = myNode.ConnectionCount;

            long[] orderedIds = new long[count]; 
            if(myNode.GetConnectionLink(0).Roadway != null)
            {
                orderedIds[0] = myNode.GetConnectionLink(0).Roadway.getId();
            }
            
            Vector3[] anchorNormals = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                Vector3 a = myNode.GetConnectionLink(i).GetEndPointsRelativeToNode(myNode).normalized;
                anchorNormals[i] = a;
            }
            
            for (int i = 1; i < count; i++) //order hashs by dot prod
            {
                Vector3 a1 = anchorNormals[i-1];
                int lowestI = -1;
                float lowestDot = 1;

                for (int j = 0; j < count; j++)
                {
                    if (j != i)
                    {
                        Vector3 a2 = anchorNormals[j];
                        float dot = Vector3.Dot(a1, a2);
                        float sign = Mathf.Sign(Vector3.Dot(Vector3.Cross(a1, a2), Vector3.up));
                        float signedDot = dot * sign;

                        if (signedDot <= lowestDot)
                        {
                            lowestDot = signedDot;
                            lowestI = j;
                        }
                    }
                }

                if(lowestI != -1 && myNode.GetConnectionLink(lowestI).Roadway != null)
                {
                    orderedIds[i] = myNode.GetConnectionLink(lowestI).Roadway.getId();
                }
                
            }

            int x = 0;
            while(x != count - 1) //find the true start 
            {
                long h1 = orderedIds[(count - x) % count];
                long h2 = orderedIds[(count - x - 1) % count];
                if (h1 == h2)
                {
                    x++;
                }
                else
                {
                    x = (count - x) % count;
                    break;
                }
            }

            Debug.Log($"offset {x}");
            
            long finalId = orderedIds[x % count];

            for (int i = 1; i < count; i++)
            {
                int trueI1 = (x + i) % count;
                int trueI2 = (x + i - 1) % count;

                long h1 = orderedIds[trueI1];
                long h2 = orderedIds[trueI2];

                unchecked{
                    if (h1 == h2)
                    {
                        finalId += h2;
                    }
                    else
                    {
                        finalId -= h2;
                    }
                }
            }

            if (connectionId != finalId)
            {
                Debug.Log($"Hash Change {finalId}");
            }

            connectionId = finalId;
        }
    }
}