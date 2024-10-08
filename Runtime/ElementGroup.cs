using System.Collections.Generic;
using UnityEngine;

namespace DecentlyGoodStreetBuilder
{
	//
	/// <summary>
	/// Groups together multiple street elements, used to group interchanges or roundabouts. 
	/// </summary>
	[System.Serializable]
	public class ElementGroup : ScriptableObject
	{
        [SerializeField] private List<StreetElement> streetElements = new List<StreetElement>();
		public StreetElement this[int index]
		{
			get { return streetElements[index]; }
		}
		public int Count
		{
			get { return streetElements.Count; }
		}

		/// <summary>
		/// Should only be called from StreetElement.MoveGroups() and StreetElement.Init()
		/// </summary>
		public void AddStreetElement(StreetElement streetElement)
		{
			if (streetElements.Contains(streetElement))
			{
				Debug.LogError("StreetElement already in group");
			}
			else
			{
				if(streetElement)
				streetElements.Add(streetElement);
			}
		}

		/// <summary>
		/// Should only be called from StreetElement.MoveGroups()
		/// </summary>
		public void RemoveStreetElement(StreetElement streetElement) 
		{
			if (streetElements.Contains(streetElement))
			{
				streetElements.Remove(streetElement);
			}
			else
			{
				Debug.LogError("StreetElement does not exist in group");
			}
		}

		private void OnDestroy()
		{
			for (int i = Count-1; i > 0; i++)
			{
				Destroy(streetElements[i]);
			}
		}
	}
}