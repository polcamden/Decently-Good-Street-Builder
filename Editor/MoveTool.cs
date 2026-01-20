using DecentlyGoodStreetBuilder.Editor;
using DecentlyGoodStreetBuilder;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
namespace DecentlyGoodStreetBuilder.Editor
{
    public class MoveTool : SelectTool
    {
        Vector3 center = Vector3.zero;

        public override void OnSelection()
        {
            Vector3 c = Vector3.zero;
            int a = 0;

            //find segments and only select nodes find center for move tool
            for (int i = 0; i < selected.Count; i++)
            {
                if (selected[i].GetType() == typeof(Segment))
                {
                    Segment segment = (Segment)selected[i];
                    selected.Add(segment.GetConnection(0));
                    selected.Add(segment.GetConnection(1));
                    selected.RemoveAt(i);
                    i--;
                }
                else
                {
                    c += selected[i].Position;
                    a++;
                }
            }

            center = c / a;
        }

        public override bool DoToSelected()
        {
            /*bool selectedOnlySegment = true;
            for (int i = 0; i < selected.Count; i++)
            {
                if (selected[i].GetType() != typeof(Segment))
                {
                    selectedOnlySegment = false;
                    break;
                }
            }*/

            if (selected.Count > 0 /*&& !selectedOnlySegment*/)
            {
                Vector3 move = Handles.PositionHandle(center, Quaternion.identity) - center;

                foreach (ISelectable selc in selected)
                {
                    selc.Position += move;
                }

                OnSelection();

                if (move != Vector3.zero)
                {
                    return false;
                }
            }

            return true;
        }

        public override void PopulateMenu(DropdownMenu menu)
        {
            menu.AppendSeparator();

            if (selected.Count > 0)
            {
                menu.AppendAction("Destroy", (item) => DestroySelected());
            }

            if(selected.Count == 1)
            {
				menu.AppendAction("Inspect Selected", (item) => InspectSelected());
			}

            base.PopulateMenu(menu);
        }
	}
}