using UnityEngine;

namespace DecentlyGoodStreetBuilder
{
	/// <summary>
	/// Holds shared values between Nodes and Segments. 
	/// </summary>
	public abstract class StreetElement : ScriptableObject
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
			set { position = value; OnPositionChange(); }
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
		}

		/// <summary>
		/// Used to draw to the editor scene. 
		/// </summary>
		/// <param name="isSelected"></param>
		public abstract void Draw(bool isSelected);

		/// <summary>
		/// Called when position is set
		/// </summary>
		public virtual void OnPositionChange() { }

		/// <summary>
		/// makes the mesh Gameobject
		/// </summary>
		public void MakeGameObject()
		{

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

		private void OnDestroy()
		{
			//tell my element group to remove me
		}
	}
}