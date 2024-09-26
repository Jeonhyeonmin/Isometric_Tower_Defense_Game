using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWalletManager : SingletonManager<PlayerWalletManager>
{
    public Sprite profile;
    public string nickname;

    [SerializeField] private int totalCoin;
    [SerializeField] private int totalCrystal;
    [SerializeField] private int totalBrick;

    [SerializeField] private int inventoryExpansionLevel = 1;

	private ItemDatabase[] itemDatabases;

	public int TotalCoin
    {
        get => totalCoin; set => totalCoin = Mathf.Clamp(value, 0, int.MaxValue);
    }

    public int TotalCrystal
    {
        get => totalCrystal; set => totalCrystal = Mathf.Clamp(value, 0, int.MaxValue);
    }

    public int TotalBrick
    {
        get => totalBrick;
        set => totalBrick = Mathf.Clamp(value, 0, int.MaxValue);
    }

    public int InventoryExpansionLevel
    {
        get => inventoryExpansionLevel;
        set => inventoryExpansionLevel = Mathf.Clamp(value, 1, 5);
    }

    public ItemDatabase[] ItemDatabases
    {
        get => itemDatabases; set => itemDatabases = value;
    }

	private void OnEnable()
	{
        totalCoin = PlayerPrefs.GetInt("PlayerWallet_Coin");
        totalCrystal = PlayerPrefs.GetInt("PlayerWallet_Crystal");
        inventoryExpansionLevel = PlayerPrefs.GetInt("PlayerInventoryExpansion_Level");

		FindItemDatabase();
	}

	private void FindItemDatabase()
	{
		itemDatabases = Resources.LoadAll<ItemDatabase>("Item/ItemDataBase");
		Debug.Log($"아이템 데이터 베이스 {itemDatabases.Length}개가 인식 되었습니다.");
	}   // 아이템 데이터 베이스를 자동으로 검색해주는 매서드

    public void UnLockingItem(Item item)
    {
        if (item == null)
            return;

        item.isUnlocking = true;
    }

	private void Start()
	{
        InvokeRepeating("SavePlayerWallet", 0.1f, 3.0f);
	}

    private void SavePlayerWallet()
    {
        PlayerPrefs.SetInt("PlayerWallet_Coin", totalCoin);
		PlayerPrefs.SetInt("PlayerWallet_Crystal", totalCrystal);
        PlayerPrefs.SetInt("PlayerInventoryExpansion_Level", inventoryExpansionLevel);
        PlayerPrefs.Save();
	}

    public int GetTotalCoin()
    {
        return totalCoin;
	}

    public int GetTotalCrystal()
    {
        return totalCrystal;
	}
}
