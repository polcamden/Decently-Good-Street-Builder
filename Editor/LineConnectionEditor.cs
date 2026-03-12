using DecentlyGoodStreetBuilder.NodeTypes;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(NodeLineConnection))]
public class LineConnectionEditor : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        SerializedProperty connections = property.FindPropertyRelative("autoHandle");
        SerializedProperty autoHandle = property.FindPropertyRelative("autoHandle");

        container.Add(new PropertyField(autoHandle));
        if (!autoHandle.boolValue)
        {
            container.Add(new PropertyField(property.FindPropertyRelative("handles")));
        }
        else
        {
            container.Add(new PropertyField(property.FindPropertyRelative("autoHardness")));
        }

        return container;
    }
}
