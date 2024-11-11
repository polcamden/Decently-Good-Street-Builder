using DecentlyGoodStreetBuilder.Roadway;
using UnityEngine;

namespace DecentlyGoodStreetBuilder.Roadway
{
    [CreateAssetMenu(menuName = "DG Street Builder/Mesh Array")]
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
			Vector3[] verticies = new Vector3[4];
			int[] triangles = new int[6];
			

			verticies[0] = Vector3.zero;
			verticies[1] = Vector3.forward;
			verticies[2] = Vector3.right;
			verticies[3] = Vector3.forward + Vector3.right;

			triangles[0] = 0;
			triangles[1] = 2;
			triangles[2] = 1;
			triangles[3] = 3;
			triangles[4] = 2;
			triangles[5] = 1;

            Debug.Log("make");

            Mesh mesh = new Mesh();
			mesh.vertices = verticies;
			mesh.triangles = triangles;
			return mesh;
        }
    }
}