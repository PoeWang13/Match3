using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
public class Item_Editor : Editor
{
    private Item myItem;
    #region Property
    private SerializedProperty icon, point;
    #endregion

    private void OnEnable()
    {
        myItem = (Item)target;
        point = serializedObject.FindProperty("point");
        icon = serializedObject.FindProperty("icon");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(point);

        if (myItem.icon != null)
        {
            Rect horizontalRect = EditorGUILayout.BeginHorizontal();
            int resimBaseHeight = 20;
            int resimIconAndTextureDistance = 30;
            Vector2Int textureSize = new Vector2Int(90, 60);
            GUILayout.Label("Icon", GUILayout.Height(textureSize.y));

            Rect resimRect = new Rect(horizontalRect.x, horizontalRect.y + ((horizontalRect.height - resimBaseHeight) / 2), horizontalRect.width - textureSize.x - (resimIconAndTextureDistance / 3), resimBaseHeight);
            Rect textureRect = new Rect(horizontalRect.width - textureSize.x + (resimIconAndTextureDistance / 3) * 2, horizontalRect.y + 5, textureSize.x, horizontalRect.height);

            EditorGUI.PropertyField(resimRect, icon, new GUIContent(" "));
            Texture2D texture2D = AssetPreview.GetAssetPreview(myItem.icon);
            GUI.DrawTexture(textureRect, texture2D);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.PropertyField(icon);
        }
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
    }
}