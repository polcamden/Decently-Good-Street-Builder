using UnityEngine;

namespace DecentlyGoodStreetBuilder.Roadway
{
	[System.Serializable]
	public abstract class RoadwayData : ScriptableObject
	{
		[SerializeField] public float offset = 0;
	}
}