using System;
using System.Collections.Generic;
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

		public ElementGroup DefaultGroup
		{
			get
			{
                ElementGroup group = groups[0];

                if (group == null)
                {
                    group = new ElementGroup();
                    group.name = "Default";
                    groups.Insert(0, group);
                }

				return group;
            }
		}

		/// <summary>
		/// Creates a new ElementGroup and moves the StreetElements into it. 
		/// </summary>
		/// <param name="streetElements"></param>
		public void MakeElementGroup(string name, StreetElement[] streetElements = null)
		{
			ElementGroup group = ScriptableObject.CreateInstance<ElementGroup>();
			group.name = name;

			groups.Add(group);

			if (streetElements != null)
			{
				for (int i = 0; i < streetElements.Length; i++)
				{
					streetElements[i].MoveGroups(group);
				}
			}
		}

	}
}