using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DecentlyGoodStreetBuilder.Roadway
{
	[System.Serializable]
	[CreateAssetMenu(menuName = "DG Street Builder/Roadway")]
	public class RoadwayBlueprint : ScriptableObject
	{
		[SerializeField] private List<RoadwayPart> parts;
		[SerializeField] private List<RoadwayData> data;

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
	}
}