using UnityEngine;



public enum CurrencyType
{
    Coin,
    Crystal,
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Shop/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public int price;
    public CurrencyType currencyType;
    public Sprite SkillIcon;
    public string description;
    public string simpleDescription;
    public bool isUnlocking;
}
