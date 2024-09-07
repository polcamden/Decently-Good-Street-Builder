using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DecentlyGoodStreetBuilder
{
    /// <summary>
    /// Main street builder class, attachs to a empty object. 
    /// </summary>
    public class StreetBuilder : MonoBehaviour
    {
        [SerializeField, HideInInspector] private List<ElementGroup> groups = new List<ElementGroup>();
        public ElementGroup this[int index]
		{
			get { return groups[index]; }
		}
		public int Count
		{
			get { return groups.Count; }
		}

		/// <summary>
		/// Adds a StreetElement to the default ElementGroup. 
		/// </summary>
		public void AddElement(StreetElement streetElement)
        {

        }

		/// <summary>
		/// Creates a new ElementGroup and moves the StreetElements into it. 
		/// </summary>
		/// <param name="streetElements"></param>
		public void MakeElementGroup(StreetElement[] streetElements)
		{

		}
	}
}