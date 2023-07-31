using UnityEngine;

// If you add something, you should edit Item_Editor script
[CreateAssetMenu(menuName = "Genel/Item")]
public class Item : ScriptableObject
{
	public int point;
	public Sprite icon;
}