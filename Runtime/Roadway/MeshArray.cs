using DecentlyGoodStreetBuilder.Roadway;
using UnityEngine;

namespace DecentlyGoodStreetBuilder.Roadway
{
    public class MeshArray : RoadwayPart, IRoadwayMesh
    {
		[SerializeField] public bool collidable;
		[SerializeField] public Material material;

		[SerializeField] public Vector2[] verticies;
        [SerializeField] public bool[] sharpVerticies;
		[SerializeField] public float[] uvs;
        
		public bool Collidable
		{
			get{ return collidable; }
		}

		public Material Material
		{
			get{ return material; }
		}

        public Mesh GenerateMesh(Segment segment, RoadwayData data)
        {
            throw new System.NotImplementedException();
        }
    }
}