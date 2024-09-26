using TMPro;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
	public static PlayerStatus Instance;

	[SerializeField] private CastleHealth[] castleHealths;
	[SerializeField] private int castleCount;
	[SerializeField] private int currentCastleCount;
	[SerializeField] private float currentCastleHealth;

	[SerializeField] private int coin;
	[SerializeField] private int crystal;

	[SerializeField] private bool isGetCoin;
	[SerializeField] private bool isGetCrystal;

	[SerializeField] private int maxWaveCount;
	[SerializeField] private int currentWaveCount;

	[SerializeField] private TMP_Text castleHealthText;
	[SerializeField] private TMP_Text castleIndexText;

	[SerializeField] private TMP_Text coinText;

	[SerializeField] private TMP_Text waveCountText;

	private void Awake()
	{
		Instance = this;
	}

	public int Coin
	{
		get => coin;
		set => coin = value;
	}

	public int Crystal
	{
		get => crystal;
		set => crystal = value;
	}

	public bool IsGetCoin
	{
		get => isGetCoin;
		set => isGetCoin = value;
	}

	public bool IsGetCrystal
	{
		get => isGetCrystal;
		set => isGetCrystal = value;
	}

	private void LateUpdate()
	{
		if (castleHealthText != null & castleIndexText != null & coinText != null && waveCountText != null)
		{
			RefreshData();
			RefreshInterface();
		}
	}

	private void RefreshData()
	{
		castleCount = castleHealths.Length;

		StageWaypoint stageWaypoint = FindAnyObjectByType<StageWaypoint>();

		currentCastleCount = stageWaypoint.currentEnemyLine;

		currentCastleHealth = castleHealths[stageWaypoint.currentEnemyLine - 1].currentHealth;

		switch (GameManager.Instance.modeType)
		{
			case GameManager.ModeType.isEasyMode:
				EasyMode enemySpawner = FindAnyObjectByType<EasyMode>();
				maxWaveCount = enemySpawner.maximumWave;
				currentWaveCount = enemySpawner.currentWaveNumber;
				break;
			case GameManager.ModeType.isNormalMode:
				//EasyMode enemySpawner = FindAnyObjectByType<EasyMode>();
				//maxWaveCount = enemySpawner.stageTemplate.easyWaves.Count;
				break;
			case GameManager.ModeType.isHardMode:
				//EasyMode enemySpawner = FindAnyObjectByType<EasyMode>();
				//maxWaveCount = enemySpawner.stageTemplate.easyWaves.Count;
				break;
		}
	}

	private void RefreshInterface()
	{
		CoinRefresh();
		CastleRefresh();
		WaveRefresh();
	}

	private void CoinRefresh()
	{
		coinText.text = coin.ToString();

		if (int.TryParse(coinText.text, out int coinValue))
		{
			if (coinValue > 9999)
			{
				coinText.text = "9999+";
			}
			else
			{
				coinText.text = Mathf.Clamp(coinValue, 0, 9999).ToString();
			}
		}
	}

	private void CastleRefresh()
	{
		castleHealthText.text = currentCastleHealth.ToString();
		castleIndexText.text = GetKoreanOrdinal(currentCastleCount) + " 성";
	}

	private string GetKoreanOrdinal(int number)
	{
		switch (number)
		{
			case 1: return "첫 번째";
			case 2: return "두 번째";
			case 3: return "세 번째";
			case 4: return "네 번째";
			case 5: return "다섯 번째";
			case 6: return "여섯 번째";
			case 7: return "일곱 번째";
			case 8: return "여덟 번째";
			case 9: return "아홉 번째";
			case 10: return "열 번째";
			default: return number + " 번째";
		}
	}

	private void WaveRefresh()
	{
		waveCountText.text = $"{currentWaveCount} / {maxWaveCount}";
	}
}
