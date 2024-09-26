using UnityEngine;

public enum ItemType
{
    Skill,
    Weapon,
}

public class SubItem : MonoBehaviour
{
    public ItemType itemType;
    public Item itemData;
    public Gun gunData;

    public Sprite itemSprite;
    public string itemTitle;
    public string itemDescription;
}
