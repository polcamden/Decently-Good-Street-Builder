using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DecentlyGoodStreetBuilder
{
	/// <summary>
	/// Holds shared values between Nodes and Segments. 
	/// </summary>
	public abstract class StreetElement : ScriptableObject, ISelectable
	{
		[SerializeField] private StreetBuilder myStreetBuilder;
		public StreetBuilder MyStreetBuilder
		{
			get { return myStreetBuilder; }
		}

		[SerializeField] private ElementGroup myElementGroup;
		public StreetBuilder MyElementGroup
		{
			get { return myStreetBuilder; }
		}

		[SerializeField] private Vector3 position;
		public virtual Vector3 Position
		{
			get { return position; }
			set { 
				if(position != value)
				{
                    Undo.RecordObject(this, "Move");
                    Undo.undoRedoEvent += OnPositionUndo;
                    position = value;
                    OnPositionChange();
                }
			}
		}

		[SerializeField] private GameObject gameObject;
		public GameObject GameObject
		{
			get { return gameObject; }
		}

		[SerializeField] private List<int> propsHash;
		[SerializeField] private List<GameObject[]> props;

		/// <summary>
		/// returns the count of propsHash, does not return the number of total props
		/// </summary>
		public int PropGroupCount
		{
			get
			{
				return propsHash.Count;
			}
		}

		public void SetProps(int hashIndex, GameObject[] objects)
		{
			int i = propsHash.IndexOf(hashIndex);

			if (i == -1)
			{
				Debug.LogError("Trying to set props but has doesnt exist");
			}
			else
			{
				props[i] = objects;
			}
		}

		public void AddPropGroup(int hash)
		{
			int i = propsHash.IndexOf(hash);

            if (i == -1)
			{
                propsHash.Add(hash);
				props.Add(new GameObject[0]);
			}
			else
			{
				Debug.LogError("Trying to add prop Group but has already exist in propHashes");
			}
		}

		public void RemovePropGroup(int hash)
		{
			int i = propsHash.IndexOf(hash);

			if(i == -1)
			{
				Debug.LogError("Trying to remove PropGroup but no propGroup of hash exists");
			}
			else
			{
				propsHash.RemoveAt(i);
				props.RemoveAt(i);
			}
		}

        /// <summary>
        /// constructorish
        /// </summary>
        /// <param name="streetBuilder"></param>
        /// <param name="elementGroup">null = default group</param>
        public virtual void Init(StreetBuilder streetBuilder, ElementGroup elementGroup = null)
		{
			this.myStreetBuilder = streetBuilder;

			if(elementGroup == null)
			{
                elementGroup = streetBuilder.DefaultGroup;
            }
            this.myElementGroup = elementGroup;

            elementGroup.AddStreetElement(this);

			MakeGameObject();
        }

		/// <summary>
		/// Used to draw to the editor scene. 
		/// </summary>
		/// <param name="isSelected"></param>
		public abstract void Draw(string[] args);

		/// <summary>
		/// Called to find if the element has been clicked on
		/// </summary>
		/// <returns>returns the elements that should be selected</returns>
		public abstract ISelectable[] Selected();

        /// <summary>
        /// Called when position is set
        /// </summary>
        public virtual void OnPositionChange() {
			if(gameObject == null)
			{
				MakeGameObject();
			}
			else
			{
				gameObject.transform.position = position;
			}
		}

		/// <summary>
		/// makes the mesh Gameobject
		/// </summary>
		public void MakeGameObject()
		{
			if (gameObject == null)
			{
				gameObject = new GameObject(this.GetType().Name + "-" + this.GetHashCode());

				gameObject.AddComponent<MeshFilter>();
				gameObject.AddComponent<MeshRenderer>();
				gameObject.AddComponent<MeshCollider>();
			}
			else
			{
				Debug.LogWarning("StreetElement " + this + " trying to create GameObject while having a gameobject already existing. ");
			}
        }

        public void Move(Vector3 move)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Moves this element to a new group. 
        /// </summary>
        /// <param name="newGroup"></param>
        public void MoveGroups(ElementGroup newGroup)
		{
			myElementGroup.RemoveStreetElement(this);
			newGroup.AddStreetElement(this);
			myElementGroup = newGroup;
		}

		public virtual void OnDestroy()
		{
			myElementGroup.RemoveStreetElement(this);
        }

		public virtual void OnPositionUndo(in UndoRedoInfo info)
		{
			OnPositionChange();
		}
    }
}