using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using DecentlyGoodStreetBuilder.Roadway;
using UnityEditor.PackageManager.UI;

namespace DecentlyGoodStreetBuilder.Editor
{
    public class AssetLibrary : EditorWindow
    {
        public static List<Object> assets;

        //will be changed
        public Segment segment;

        [MenuItem("Tools/Decently Good Street Builder/Asset Library")]
        public static void ShowWindow(Segment segment)
        {
			AssetLibrary window = GetWindow<AssetLibrary>("Asset Library", true);
			window.Show();
            window.segment = segment;
		}

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Library");
            //path = GUILayout.TextField(path);
            if (GUILayout.Button("Refresh"))
            {
                string[] assetFiles = Directory.GetFiles(EditorSettings.Settings.assetPaths[0]);

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
            if(asset == null)
            {
                return;
            }

			GUILayout.BeginVertical();
			GUILayout.Label($"{asset.name}");
			if (segment != null && asset.GetType() == typeof(RoadwayBlueprint) && GUILayout.Button("apply to segment"))
			{
                segment.Roadway = (RoadwayBlueprint)asset;
			}
			GUILayout.EndVertical();
		}
    }
}