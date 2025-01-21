using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace DecentlyGoodStreetBuilder.Roadway
{
	[CreateAssetMenu(menuName = "DG Street Builder/Roadway")]
	public class RoadwayBlueprint : ScriptableObject
	{
		[SerializeField] private List<RoadwayPart> parts = new List<RoadwayPart>();
		[SerializeField] private List<RoadwayData> data = new List<RoadwayData>();

		public int Count
		{
			get { return parts.Count; }
		}

		public RoadwayPart this[int i]
		{
			get { return parts[i]; }
		}

		/// <summary>
		/// Adds RoadwayPart and Data to the asset. 
		/// </summary>
		/// <param name="part"></param>
		/// <param name="offset"></param>
		public void AddPart(RoadwayPart part, float offset = 0)
		{
			parts.Add(part);
			RoadwayData d = part.CreateDataObject();

			AssetDatabase.AddObjectToAsset(d, this);
			AssetDatabase.SaveAssets();

			data.Add(d);
		}

		/// <summary>
		/// Removes RoadwayPart and Data from the asset. 
		/// </summary>
		/// <param name="i"></param>
		public void RemovePart(int i)
		{
			parts.RemoveAt(i);

			AssetDatabase.RemoveObjectFromAsset(data[i]);
			AssetDatabase.SaveAssets();

			data.RemoveAt(i);
		}

		/// <summary>
		/// Returns RoadwayPart at index
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public RoadwayPart GetPart(int i)
		{
			return parts[i];
		}

		/// <summary>
		/// Returns RoadwayData at index
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public RoadwayData GetData(int i)
		{
			return data[i];
		}
	
		/*public void UpdateSegmentsRoadway(Segment segment, Mesh mesh, Dictionary<RoadwayData, List<GameObject>> dataToGameobjects)
		{
			bool[] exists = new bool[dataToGameobjects.Count];
			int existsIndex = 0;

			for (int i = 0; i < Count; i++)
            {
				if (GetPart(i).GetType().GetInterface(nameof(IRoadwayObjects)) != null)
				{
					RoadwayData data = GetData(i);

					if (dataToGameobjects.ContainsKey(data))
					{
						exists[existsIndex] = true;
						
					}
					else
					{
						exists[existsIndex] = false;
					}

                    existsIndex++;
				}
			}
		}*/


		public (Mesh, Material[]) GenerateRoadwayMesh(Segment segment, Mesh mesh)
		{
			CubicBezierCurve baseCurve = segment.ToBezierCurve();

			List<CombineInstance> submeshs = new List<CombineInstance>();

			List<Material> materials = new List<Material>();

            for (int i = 0; i < Count; i++)
            {
                if(GetPart(i).GetType().GetInterface(nameof(IRoadwayMesh)) != null) //does part have IRoadwayMesh
				{
					Mesh subMesh = ((IRoadwayMesh)GetPart(i)).GenerateMesh(baseCurve, GetData(i));
					
                    CombineInstance combine = new CombineInstance();
                    combine.mesh = subMesh;
					combine.transform = segment.GameObject.transform.localToWorldMatrix;
					submeshs.Add(combine);

					materials.Add(((IRoadwayMesh)GetPart(i)).Material);
                }
            }

            mesh.Clear();

			mesh.CombineMeshes(submeshs.ToArray(), false, false, false);

			mesh.RecalculateBounds();
			mesh.Optimize();

            return (mesh, materials.ToArray());
        }
		
		public void UpdateGameObjects(Segment segment, Dictionary<RoadwayData, List<GameObject>> gameObjects)
		{
			for (int i = 0; i < Count; i++)
			{
				RoadwayData data = GetData(i);


			}

			//make sure 

            /*for (int i = 0; i < Count; i++)
            {
                if (GetPart(i).GetType().GetInterface(nameof(IRoadwayObjects)) != null) //does part have IRoadwayMesh
                {
                    //search for existing items in dictionary
					//if not found 
                }
            }*/
        }
	}
}