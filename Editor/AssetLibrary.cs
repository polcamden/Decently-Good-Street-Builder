using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using DecentlyGoodStreetBuilder.Roadway;
using UnityEditor.PackageManager.UI;
using static Codice.Client.BaseCommands.Import.Commit;

namespace DecentlyGoodStreetBuilder.Editor
{
    public class AssetLibrary : EditorWindow
    {
        private List<Object> assets;

        private List<string> assetPaths;

        private bool showSettings = false;

        [MenuItem("Tools/Decently Good Street Builder/Asset Library")]
        public static void ShowWindow()
        {
			AssetLibrary window = GetWindow<AssetLibrary>("Asset Library", true);
			window.Show();
		}

		private void OnEnable()
		{
			int size = EditorPrefs.GetInt("size", 0);
			assetPaths = new List<string>();
			for (int i = 0; i < size; i++)
			{
				assetPaths.Add(EditorPrefs.GetString($"path{i}"));
			}
		}

		private void OnDisable()
		{
            EditorPrefs.SetInt("size", assetPaths.Count);
            for (int i = 0; i < assetPaths.Count; i++)
            {
                EditorPrefs.SetString($"path{i}", assetPaths[i]);
            }
		}

		private void OnGUI()
        {
            //Header
            GUILayout.BeginHorizontal();
			GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
			labelStyle.fontSize = 16;
			labelStyle.fontStyle = FontStyle.Bold;
			GUILayout.Label("Library", labelStyle);
            if (GUILayout.Button("Settings", GUILayout.Width(84)))
            {
                showSettings = !showSettings;
			}
            if (GUILayout.Button("Refresh", GUILayout.Width(84)))
            {
                LoadAssets();
			}
            GUILayout.EndHorizontal();
            DrawDivider();

            //settings
			if (showSettings)
            {
                DisplaySettings();
			}

            //Asset List
            if(assets != null)
                DrawAssetList(assets, 3);
        }

        private void LoadAssets()
        {
			assets = new List<Object>();
			foreach (string assetFile in assetPaths)
			{
				Object asset = AssetDatabase.LoadAssetAtPath(assetFile, typeof(Object));

				if (asset != null && asset.GetType() == typeof(RoadwayBlueprint))
					assets.Add(asset);
			}
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

        private void DrawAsset(Object asset)
        {
            if(asset == null)
            {
                return;
            }

			GUILayout.BeginVertical();
			GUILayout.Label($"{asset.name}");
            if (SelectTool.selected.Count != 0 && GUILayout.Button("Apply"))
            {
                foreach (ISelectable select in SelectTool.selected)
                {
                    if(select.GetType() == typeof(Segment))
                    {
                        Segment segment = (Segment)select;
                        segment.Roadway = (RoadwayBlueprint)asset;
                    }
                }
            }
			GUILayout.EndVertical();
		}
    
        private void DisplaySettings()
        {
            GUILayout.Label("Asset Paths");

            for (int i = 0; i < assetPaths.Count; i++)
            {
                GUILayout.BeginHorizontal();
				assetPaths[i] = EditorGUILayout.TextField(assetPaths[i]);
				if (GUILayout.Button("-", GUILayout.Width(16)))
				{
                    assetPaths.RemoveAt(i);
                    break;
				}
                GUILayout.EndHorizontal();
			}

            if (GUILayout.Button("+"))
            {
                assetPaths.Add("");
            }
            DrawDivider();
		}

        private void DrawDivider()
        {
			EditorGUILayout.Space(4);
			Rect rect = EditorGUILayout.GetControlRect(false, 1);
			EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1)); // Dark grey line
            EditorGUILayout.Space(4);
		}
	}
}