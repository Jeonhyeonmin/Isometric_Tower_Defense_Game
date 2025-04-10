using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using BackEnd.BackndLitJson;
using LitJson;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelChartData
{
    public int level;
    public int maxExperience;
    public int rewardGold;
}


public class PlayerWalletManager : SingletonManager<PlayerWalletManager>
{
    public Sprite profile;
    public string profilebase64;
    public string nickname;

    [SerializeField] private int totalCoin;
    [SerializeField] private int totalCrystal;
    [SerializeField] private int totalBrick;

    [SerializeField] private int inventoryExpansionLevel = 1;
    [SerializeField] private int playerLevel = 1;
    [SerializeField] private float playerExp;

    private readonly string LEVEL_CHART = "174725";

    public List<LevelChartData> levelChart;

    public PlayerWalletManager()
    {
        levelChart = new List<LevelChartData>();
    }

    public void LoadLevelChart()
    {
        Backend.Chart.GetChartContents(LEVEL_CHART, callback =>
        {
            if (callback.IsSuccess())
            {
                // JSON 데이터 파싱
                try
                {
                    LitJson.JsonData json = callback.FlattenRows();

                    if (json.Count <= 0)
                    {
                        Debug.LogWarning("No data found in the chart.");
                    }
                    else
                    {
                        for (int i = 0; i < json.Count; ++i)
                        {
                            LevelChartData newChart = new LevelChartData();
                            newChart.level = int.Parse(json[i]["level"].ToString());
                            newChart.maxExperience = int.Parse(json[i]["maxExperience"].ToString());
                            newChart.rewardGold = int.Parse(json[i]["rewardGold"].ToString());

                            levelChart.Add(newChart);

                            Debug.Log($"Level : {newChart.level}, Max Exp : {newChart.maxExperience}" +
                                $"Reward Gold : {newChart.rewardGold}"); 
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing JSON data: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("Failed to load level chart: " + callback.GetErrorCode());
            }
        });
    }

    private readonly int increaseExperience = 250;

    public void Process()
    {
        int currentLevel = playerLevel;

        playerExp += increaseExperience;

        if (playerExp >= levelChart[playerLevel-1].maxExperience &&
            levelChart.Count > playerLevel)
        {
            // 레벨업 보상 지급
            totalCoin += levelChart[playerLevel - 1].rewardGold;
            playerExp = 0;
            playerLevel++;
            Debug.Log($"레벨업! 현재 레벨 : {playerLevel}");
        }

        PlayerPrefs.Save();
    }


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

    public int PlayerLevel
    {
        get => playerLevel;
        set => playerLevel = Mathf.Clamp(value, 1, 100);    // 레벨 수정 필요 (서버 연동)
    }

    public float PlayerExp
    {
        get => playerExp;
        set => playerExp = Mathf.Clamp(value, 0, 100);    // 경험치 수정 필요 (서버 연동)
    }

	public void VirtualAwake()
	{
        totalCoin = PlayerPrefs.GetInt("PlayerWallet_Coin");
        totalCrystal = PlayerPrefs.GetInt("PlayerWallet_Crystal");
        inventoryExpansionLevel = PlayerPrefs.GetInt("PlayerInventoryExpansion_Level");
        profilebase64 = PlayerPrefs.GetString("PlayerProfileBase64");
        nickname = PlayerPrefs.GetString("PlayerNickname");
        playerLevel = PlayerPrefs.GetInt("PlayerLevel");
        playerExp = PlayerPrefs.GetFloat("PlayerExp");

        if (profile != null)
        {
            string base64 = profilebase64;
            byte[] imageBytes = Convert.FromBase64String(base64);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            Debug.Log(base64);

            profile = SpriteFromTexture2D(texture);
        }
        

        FindItemDatabase();
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            playerExp += 50;
            Process();
        }
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
        PlayerPrefs.SetInt("PlayerLevel", playerLevel);
        PlayerPrefs.SetFloat("PlayerExp", playerExp);
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
