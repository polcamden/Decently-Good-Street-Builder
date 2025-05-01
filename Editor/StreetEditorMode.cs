using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UIElements;

namespace DecentlyGoodStreetBuilder.Editor
{
	[EditorToolContext("Street Builder Editor Mode", typeof(StreetBuilder))]
	[Icon("Assets/Examples/Icons/TransformIcon.png")]
	public class EditorMode : EditorToolContext
	{
		public static StreetBuilder streetBuilder;

		public static void AddNode()
		{
            Node node = CreateInstance<Node>();
			node.Init(Vector3.zero, streetBuilder);
        }

		public override void OnToolGUI(EditorWindow _) 
		{
            if (streetBuilder == null) DestroyImmediate(this);
			
            //locks the GameObject
            Selection.objects = new UnityEngine.Object[] { ((StreetBuilder)target).gameObject };

			streetBuilder = ((StreetBuilder)target).gameObject.GetComponent<StreetBuilder>();
        }

        public override void PopulateMenu(DropdownMenu menu)
        {
            menu.AppendAction("Create Node", (item) => AddNode());
        }

        protected override Type GetEditorToolType(Tool tool)
		{
			//Tool menu
			switch (tool)
			{
				case Tool.Rect:
					return typeof(SelectTool);
				case Tool.Move:
					return typeof(MoveTool);
				default:
					return null;
			}
		}
	}
	
	public class SelectTool : EditorTool
	{
		public List<ISelectable> selected = new List<ISelectable>();

		public override void OnToolGUI(EditorWindow _)
		{
			if (EditorMode.streetBuilder == null) return;

			DrawElements(null);

			DoToSelected();

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.isMouse)
			{
				bool voidClick = true;

				EditorMode.streetBuilder.ResetElementLoop();
				StreetElement elem = EditorMode.streetBuilder.NextElement();
				while (elem != null)
				{
					ISelectable[] selc = elem.Selected();

                    if (selc != null)
					{
                        if (!Event.current.shift)
                        {
							selected.Clear();
                        }

						//prevent double selection and adds shift deselect
						bool containsAll = true;
						for (int i = 0; i < selc.Length; i++)
						{
							if (!selected.Contains(selc[i]))
							{
								selected.Add(selc[i]);
								containsAll = false;
                            }
						}

						if (containsAll)
						{
							for (int i = 0; i < selc.Length; i++)
							{
								selected.Remove(selc[i]);
							}
                        }

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

		private void AddSelected()
		{

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

        public override void PopulateMenu(DropdownMenu menu)
        {
            menu.AppendSeparator();

			if (selected.Count == 1 && selected[0].GetType() == typeof(Segment))
			{
                menu.AppendAction("Change Roadway", (item) => ChangeRoadway());
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

		/// <summary>
		/// Todo: open a library of roadway materials
		/// </summary>
		public void ChangeRoadway()
		{
			Debug.Log("not implimented");
		}
	}
}