using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
	/// <summary>
	/// Given to Nodes who have 3 or more connections. 
	/// </summary>
	[System.Serializable]
	public class Intersection : NodeType
    {
        [SerializeField] float radius = 6;

        public override void Draw(string[] args)
        {
            
        }

        public override Mesh GenerateSurfaceMesh()
        {
            return null;
        }

        public override void HandleUpdate()
        {
            if (MyNode.ConnectionCount < 3)
            {
                Debug.LogError("Intersection is being used on a node that doesn't have 3 or more connections");
                return;
            }

			for (int i = 0; i < MyNode.ConnectionCount; i++)
			{
                Node n = MyNode.GetConnectionLink(i).GetConnection(0);
                if(n == MyNode)
                {
					n = MyNode.GetConnectionLink(i).GetConnection(1);
				}

                Vector3 endPoint = GeometryF.Normal(MyNode.Position, n.Position) * radius;

                MyNode.GetConnectionLink(i).SetEndPointRelativeToNode(MyNode, endPoint);
			}
		}
    }
}