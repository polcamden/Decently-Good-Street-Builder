using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.GPUSort;

namespace DecentlyGoodStreetBuilder.Editor
{
	[EditorToolContext("Street Builder Editor Mode", typeof(StreetBuilder))]
	[Icon("Assets/Examples/Icons/TransformIcon.png")]
	public class EditorMode : EditorToolContext
	{
		public static StreetBuilder streetBuilder;

		public override void OnActivated()
		{
			//Debug.Log("asd");
		}

		public override void OnWillBeDeactivated()
		{
			//Debug.Log("asd");
		}

		public static void AddNode()
		{
            Node node = CreateInstance<Node>();
			node.Init(Vector3.zero, streetBuilder);
        }

		public override void OnToolGUI(EditorWindow _) 
		{ 
			//locks the GameObject
			Selection.objects = new UnityEngine.Object[] { ((StreetBuilder)target).gameObject };

			streetBuilder = ((StreetBuilder)target).gameObject.GetComponent<StreetBuilder>();

			//tool tips
            if (Event.current.alt)
            {
                GenericMenu menu = new GenericMenu();

                // Add menu items (name and callback method)
                menu.AddItem(new GUIContent("Custom Action 1"), false, () => AddNode());

                // Show the context menu
                menu.ShowAsContext();

                // Mark the event as used, so it doesn't propagate
                Event.current.Use();
            }

			/*streetBuilder.ResetElementLoop();
			StreetElement elem = streetBuilder.NextElement();
			while (elem != null) {
                elem.Draw(false);

                elem = streetBuilder.NextElement();
            }*/
        }

		protected override Type GetEditorToolType(Tool tool)
		{
			//Tool menu
			switch (tool)
			{
				case Tool.Move:
					return typeof(MoveTool);
				default:
					return null;
			}
		}
	}

	public class EditorModeMenuItems
	{
		
		static void CreateNode()
		{

		}
	}
	
	public abstract class SelectTool : EditorTool
	{
		public List<StreetElement> selected = new List<StreetElement>();

		public override void OnToolGUI(EditorWindow _)
		{
			DrawElements(null);

			DoToSelected();

            if ( Event.current.type == EventType.MouseDown)
			{
				bool voidClick = true;

				EditorMode.streetBuilder.ResetElementLoop();
				StreetElement elem = EditorMode.streetBuilder.NextElement();
				while (elem != null)
				{
					if (elem.Selected())
					{
                        if (!Event.current.shift)
                        {
							selected.Clear();
                        }

                        selected.Add(elem);
						OnSelection();
                        voidClick = false;

						break;
					}

					elem = EditorMode.streetBuilder.NextElement();
				}

				if (voidClick)
				{
					selected.Clear();
					OnSelection();
                }
			}
        }

		/// <summary>
		/// Draws all elements given a string of args
		/// </summary>
		/// <param name="args"></param>
		private void DrawElements(string[] args)
		{
			if(args == null)
			{
				args = new string[0];
			}

			//given to elements that are selected
			string[] selectedArgs = new string[args.Length + 1];
			for (int i = 0; i < args.Length; i++)
			{
				selectedArgs[i] = args[i];
			}
			selectedArgs[args.Length] = "selected";

            EditorMode.streetBuilder.ResetElementLoop();
            StreetElement elem = EditorMode.streetBuilder.NextElement();
            while (elem != null)
            {
                elem.Draw(selected.Contains(elem) ? selectedArgs : args);

                elem = EditorMode.streetBuilder.NextElement();
            }
        }

		/// <summary>
		/// Called when a new selection has been made
		/// </summary>
		public virtual void OnSelection() { }

        /// <summary>
        /// Called every Tool update after selections have been managed
        /// </summary>
        /// <returns>Returns if the select should happen; false - no select check</returns>
        public virtual bool DoToSelected() { return false; }
	}

	public class MoveTool : SelectTool
	{
		Vector3 center = Vector3.zero;

        public override void OnSelection()
        {
			Vector3 c = Vector3.zero;

			for (int i = 0; i < selected.Count; i++)
			{
				c += selected[i].Position;
			}

			center = c / selected.Count;
        }

        public override bool DoToSelected()
        {
			if (selected.Count > 0)
			{
				Vector3 move = Handles.PositionHandle(center, Quaternion.identity) - center;

				foreach (StreetElement elem in selected)
				{
					elem.Position += move;
				}

				OnSelection();

				if (move != Vector3.zero)
				{
					return false;
				}
			}

			return true;
        }
    }
}