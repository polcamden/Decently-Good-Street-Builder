using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;

[CreateAssetMenu(menuName = "Decently Good Street Builder/Editor Setting", order = 100)]
public class EditorSettings : ScriptableObject
{
    [Header("Asset Library")]
    [SerializeField] public List<string> assetPaths;
    [Header("Tools")]
    [SerializeField] public bool autoUpdate;

	private const string settingsPath = "Assets/Decently-Good-Street-Builder/Editor/EditorSettings.asset";
	private static EditorSettings currentSettings;
	public static EditorSettings Settings
	{
		get
		{
			if (currentSettings == null)
			{
				currentSettings = AssetDatabase.LoadAssetAtPath<EditorSettings>(settingsPath);
				if (currentSettings == null)
				{
					Debug.LogError("Editor Settings not found. Goto EditorSettings and change string settingsPath. ");
				}
			}
			return currentSettings;
		}
	}

}
