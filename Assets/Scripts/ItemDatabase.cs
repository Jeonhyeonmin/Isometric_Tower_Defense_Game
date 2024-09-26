using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Shop/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
	public string databaseName = string.Empty;
	public Item[] items;
}
