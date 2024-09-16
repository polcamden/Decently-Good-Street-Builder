using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.TerrainTools;
using UnityEngine;

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
			
			//draw
			for (int i = 0; i < streetBuilder.Count; i++)
			{
				for (int j = 0; j < streetBuilder[i].Count; j++) {
					streetBuilder[i][j].Draw(false);
				}
			}
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
	
	public class MoveTool : EditorTool
	{
		public override void OnToolGUI(EditorWindow _)
		{

		}
	}
}