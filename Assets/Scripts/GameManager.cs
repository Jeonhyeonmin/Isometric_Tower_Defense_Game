using NUnit.Framework;
using NUnit.Framework.Internal.Commands;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : SingletonManager<GameManager>
{
	public enum ModeType
	{
		isEasyMode,
		isNormalMode,
		isHardMode,
	}

	public ModeType modeType;

	[SerializeField] private GameObject spawnerManager;
	[SerializeField] private EnemySpawner enemySpawner;

	public int currentkillEnemyCount;
	public int currentWaveLivingEnemy;
	public int currentScore;

	private int stageFlagCount;
	public string seletedJoinFlagName;

	private int currentChapter;
	public int CurrentChapter
	{
		get => currentChapter;
		set => currentChapter = value;
	}
	public int StageFlagCount
	{
		get => stageFlagCount;
		set => stageFlagCount = value;
	}

	private Button nextWaveButton;

	public StageJoinFlag seletedJoinFlag;
	[SerializeField] private StageWaypoint stageWaypoint;

	[SerializeField] private GameObject castleStatusNotify;
	[SerializeField] private TMP_Text castleStatus_Title_Text;

	[SerializeField] private UIManager uIManager;

	public SpriteRenderer flagStar;

	public GameObject poolingManager;

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void Setup()
	{
		uIManager = FindAnyObjectByType<UIManager>();

		if (spawnerManager != null)
		{
			SettingGameMode();
		}

		StageFlag();
		FindStageWayPoint();
		CastleStatus();

		nextWaveButton = GameObject.Find("Play_Button")?.GetComponent<Button>();
		
		if (nextWaveButton != null)
		{
			nextWaveButton.onClick.AddListener(OnReady);
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Setup();
	}

	private void CastleStatus()
	{
		castleStatusNotify = GameObject.Find("CastleStatusNotify");
		castleStatus_Title_Text = GameObject.Find("CastleStatus_Title_Text")?.GetComponent<TMP_Text>();
	}

	private void StageFlag()
	{
		StageJoinFlag[] stageJoinFlags = FindObjectsByType<StageJoinFlag>(FindObjectsSortMode.None);
		
		foreach (StageJoinFlag stageJoinFlag in stageJoinFlags)
		{
			if (!string.IsNullOrEmpty(stageJoinFlag.stageName))
			{
				stageFlagCount++;
			}
		}

		if (!string.IsNullOrEmpty(seletedJoinFlagName))
		{
			foreach (StageJoinFlag stageJoinFlag in stageJoinFlags)
			{
				if (stageJoinFlag.stageName == seletedJoinFlagName)
				{
					seletedJoinFlag = stageJoinFlag;
					SpriteRenderer[] spriteRenderers = seletedJoinFlag.GetComponentsInChildren<SpriteRenderer>();
					flagStar = spriteRenderers[1];
					break;
				}
			}
		}
		
	}

	private void FindStageWayPoint()
	{
		spawnerManager = FindAnyObjectByType<EnemySpawner>()?.gameObject;
		enemySpawner = FindAnyObjectByType<EnemySpawner>();
		poolingManager = FindAnyObjectByType<PoolingManager>()?.gameObject;

		if (spawnerManager != null)
		{
			stageWaypoint = FindAnyObjectByType<StageWaypoint>();
		}
	}

	private void SettingGameMode()
	{
		switch (modeType)
		{
			case ModeType.isEasyMode:
				enemySpawner = spawnerManager.GetComponent<EasyMode>();
				break;
			case ModeType.isNormalMode:
				break;
			case ModeType.isHardMode:
				break;
		}

		currentWaveLivingEnemy = 0;
	}

	public EnemyTemplate GetEnemyTemplate(string _EnemyTemplateType) // string의 값은 Resource/Enemy Template에 있는 파일들의 이름
	{
		EnemyTemplate enemyTemplate = Resources.Load<EnemyTemplate>($"Enemy Template/{_EnemyTemplateType}");

		if (enemyTemplate == null)
		{
			Debug.LogError($"{_EnemyTemplateType}의 템플릿이 존재하지 않습니다!");
			return null;
		}

		Debug.Log("가져왔습니다 : " + enemyTemplate.name);
		return enemyTemplate;
	}

	public BulletTemplate GetBulletTemplate(string _BulletTemplateType) // string의 값은 Resource/Bullet Template에 있는 파일들의 이름
	{
		BulletTemplate bulletTemplate = Resources.Load<BulletTemplate>($"Bullet Template/{_BulletTemplateType}");

		if (bulletTemplate == null)
		{
			Debug.LogError($"{_BulletTemplateType}의 템플릿이 존재하지 않습니다!");
			return null;
		}

		return bulletTemplate;
	}

	public void castleFallAnimationNotify()
	{
		string castleMessage = stageWaypoint.currentEnemyLine switch
		{
			1 => "첫 번째 성이 무너졌습니다.",
			2 => "두 번째 성이 무너졌습니다.",
			3 => "세 번째 성이 무너졌습니다.",
			4 => "네 번째 성이 무너졌습니다.",
			5 => "다섯 번째 성이 무너졌습니다.",
			_ => "알 수 없는 성이 무너졌습니다.",
		};

		castleStatus_Title_Text.text = castleMessage;
		castleStatusNotify.GetComponent<Animator>().SetTrigger("Notify");
	}

	public void OnReady()
	{
		StartCoroutine(enemySpawner.StartWave());
		Debug.Log("Spawner OnReady");
	}

	private void OnDisable()
	{
		if (nextWaveButton != null)
		{
			nextWaveButton.onClick.RemoveListener(OnReady);
		}

		currentkillEnemyCount = 0;
		currentWaveLivingEnemy = 0;
		stageFlagCount = 0;
		currentScore = 0;
		castleStatusNotify = null;
		castleStatus_Title_Text = null;
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
}
