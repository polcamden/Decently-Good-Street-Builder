using DecentlyGoodStreetBuilder.Roadway;
using UnityEditor;
using UnityEngine;

namespace PolsStreetBuilder.RoadwayParts
{
    [CustomEditor(typeof(RoadwayBlueprint))]
    public class RoadwayEditor : Editor
    {
        private RoadwayBlueprint roadway;

        private void OnEnable()
        {
            roadway = (RoadwayBlueprint)target;
        }

        public override void OnInspectorGUI()
        {
            if (roadway == null)
                return;

            for (int i = 0; i < roadway.Count; i++)
            {
                RoadwayPart par = roadway.GetPart(i);
                RoadwayData data = roadway.GetData(i);

                if (par == null) //roadwayPart has been deleted
                {
                    roadway.RemovePart(i);
                    break;
                }

                var boldtext = new GUIStyle(GUI.skin.label);
                boldtext.fontStyle = FontStyle.Bold;

                GUILayout.BeginVertical("box");

                //header
                EditorGUILayout.LabelField(par.name, boldtext);

                //body
                if (data != null)
                {
                    CreateEditor(data).OnInspectorGUI();
                }
                if (GUILayout.Button("Remove"))
                {
                    roadway.RemovePart(i);
                    break; //prevents out of index on this loop
                }

                GUILayout.EndVertical();
                GUILayout.Space(6);
            }

            RoadwayPart part = (RoadwayPart)EditorGUILayout.ObjectField("Add Part",null, typeof(RoadwayPart), false);

            if (part != null)
            {
                roadway.AddPart(part);
            }

        }
    }
}