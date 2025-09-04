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
}