using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWalletManager : SingletonManager<PlayerWalletManager>
{
    public Sprite profile;
    public string profilebase64;
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

	private void Awake()
	{
        totalCoin = PlayerPrefs.GetInt("PlayerWallet_Coin");
        totalCrystal = PlayerPrefs.GetInt("PlayerWallet_Crystal");
        inventoryExpansionLevel = PlayerPrefs.GetInt("PlayerInventoryExpansion_Level");
        profilebase64 = PlayerPrefs.GetString("PlayerProfileBase64");
        nickname = PlayerPrefs.GetString("PlayerNickname");

        string base64 = profilebase64;
        byte[] imageBytes = Convert.FromBase64String(base64);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);
        Debug.Log(base64);

        profile = SpriteFromTexture2D(texture);

        FindItemDatabase();
	}

    private Sprite SpriteFromTexture2D(Texture2D texture)
    {
        // Texture2D에서 Sprite를 생성합니다.
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
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

    private void OnDisable()
    {
        SavePlayerWallet();
    }

    private void SavePlayerWallet()
    {
        PlayerPrefs.SetInt("PlayerWallet_Coin", totalCoin);
		PlayerPrefs.SetInt("PlayerWallet_Crystal", totalCrystal);
        PlayerPrefs.SetInt("PlayerInventoryExpansion_Level", inventoryExpansionLevel);
        PlayerPrefs.SetString("PlayerProfileBase64", profilebase64);
        PlayerPrefs.SetString("PlayerNickname", nickname);
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
