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
				ElementGroup group = null;

                if (groups.Count == 0 || groups[0].name != "Default")
                {
                    group = new ElementGroup();
                    group.name = "Default";
                    groups.Insert(0, group);
				}
				else
				{
                    group = groups[0];
                }

				return group;
            }
		}

		private int groupIndex = 0;
		private int elementIndex = 0;

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
	
		public void ResetElementLoop()
		{
            groupIndex = 0;
            elementIndex = 0;
        }

        /// <summary>
        /// Returns the next Element of the hierarchy. Call ResetElementLoop() before iterating on NextElement(). 
        /// </summary>
        /// <returns>Next Element of the hierarchy will return null at the end</returns>
        public StreetElement NextElement()
		{
			StreetElement element = null;

            if (groups.Count > groupIndex)
			{
                if (groups[groupIndex].Count > elementIndex)
				{
					element = groups[groupIndex][elementIndex];
                    elementIndex++;
                }
				else
				{
					elementIndex = 0;
					groupIndex++;
                }
			}
			else
			{
				ResetElementLoop();
            }

			return element;
		}
	}
}