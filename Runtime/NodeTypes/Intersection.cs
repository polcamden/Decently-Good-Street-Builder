using UnityEngine;

namespace DecentlyGoodStreetBuilder.NodeTypes
{
	/// <summary>
	/// Given to Nodes who have 3 or more connections. 
	/// </summary>
	[System.Serializable]
	public class Intersection : NodeType
    {
        int radius = 10;

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
			for (int i = 0; i < MyNode.ConnectionCount; i++)
			{
                Node n = MyNode.GetConnectionLink(i).GetConnection(0);
                if(n == MyNode)
                {
					MyNode.GetConnectionLink(i).GetConnection(1);
				}

                Vector3 endPoint = GeometryF.Normal(MyNode.Position, n.Position) * radius;
                Debug.Log(endPoint);
                MyNode.GetConnectionLink(i).SetEndPointRelativeToNode(MyNode, new Vector3(10, 2, 6));
			}
		}
    }
}