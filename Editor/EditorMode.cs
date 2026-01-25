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

        public override void OnActivated()
        {
            base.OnActivated();

			streetBuilder = (StreetBuilder)target;
			Selection.activeObject = streetBuilder.gameObject;
        }

		public override void OnWillBeDeactivated()
		{
			base.OnWillBeDeactivated();
			streetBuilder = null;
			ToolManager.RestorePreviousPersistentTool();
		}

		public override void OnToolGUI(EditorWindow _) 
		{
            if (streetBuilder == null) DestroyImmediate(this);
			
			HandleUtility.AddDefaultControl(
				GUIUtility.GetControlID(FocusType.Passive)
			);

			if (streetBuilder != null && Selection.activeObject != streetBuilder.gameObject)
			{
				Selection.activeObject = streetBuilder.gameObject;
			}
        }

        public override void PopulateMenu(DropdownMenu menu)
        {
            menu.AppendAction("Create Node", (item) => AddNode());
        }

		public static void AddNode()
		{
            Node node = CreateInstance<Node>();
			node.Init(Vector3.zero, streetBuilder);
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