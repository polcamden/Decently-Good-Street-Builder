using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using DecentlyGoodStreetBuilder.Roadway;

namespace DecentlyGoodStreetBuilder.Editor
{
    public class AssetLibrary : EditorWindow
    {
        // "Assets/DGSB Prototype Assets"
        public static string path = "Assets/protoAssets";
        
        public static List<Object> assets;

        [MenuItem("Tools/Decently Good Street Builder/Asset Library")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(AssetLibrary));
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Library");
            path = GUILayout.TextField(path);
            if (GUILayout.Button("Refresh"))
            {
                string[] assetFiles = Directory.GetFiles(path);

                assets = new List<Object>();
                foreach (string assetFile in assetFiles)
                {
                    Object asset = AssetDatabase.LoadAssetAtPath(assetFile, typeof(Object));

                    if(asset != null && asset.GetType() == typeof(RoadwayBlueprint))
                        assets.Add(asset);
                }
            }
            GUILayout.EndHorizontal();

            if(assets != null)
                DrawAssetList(assets, 3);
        }

        private void DrawAssetList(List<Object> assets, int width)
        {
            GUILayout.Label($"Count: {assets.Count}");

            GUILayout.BeginHorizontal();

            for (int i = 0; i < assets.Count; i++)
            {
                DrawAsset(assets[i]);


				if (i != 0 && i % width == 0)
                {
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
				}
            }

            GUILayout.EndHorizontal();
        }

        public void DrawAsset(Object asset)
        {
			GUILayout.BeginVertical();
			GUILayout.Label($"{asset.name}");
			if (GUILayout.Button("use"))
			{

			}
			GUILayout.EndVertical();
		}
    }
}