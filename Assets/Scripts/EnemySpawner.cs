using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class EnemySpawner : MonoBehaviour
{
	public StageTemplate stageTemplate;

	public int currentWaveNumber = 0;
	public int maximumWave = 0;

	public int currentEnemyCount;
	public int currentWaveEnemyCount;
	public int maximumEnemyCount;

	public bool isWave;

	[SerializeField] private UIManager uiManager;


	private void Awake()
	{
		AutomaticScriptable();
		NextSetup();
		uiManager = FindAnyObjectByType<UIManager>();

		switch (GameManager.Instance.modeType)
		{
			case GameManager.ModeType.isEasyMode:
				maximumWave = stageTemplate.easyWaves.Count;
				break;
			case GameManager.ModeType.isNormalMode:
				maximumWave = stageTemplate.easyWaves.Count;
				break;
			case GameManager.ModeType.isHardMode:
				maximumWave = stageTemplate.easyWaves.Count;
				break;
		}

		Debug.Log("Spawner Awake 실행");
	}

	private void AutomaticScriptable()
	{
		string currentSceneName = SceneManager.GetActiveScene().name;
		StageTemplate[] stageTemplates = Resources.LoadAll<StageTemplate>("Stage Template");

		foreach (StageTemplate stageTemplate in stageTemplates)
		{
			if (stageTemplate.StageDeveloperName == currentSceneName)
			{
				this.stageTemplate = stageTemplate;
			}
		}
	}

	public virtual void NextSetup()
	{
		switch (GameManager.Instance.modeType)
		{
			case GameManager.ModeType.isEasyMode:
				maximumWave = stageTemplate.easyWaves.Count;
				currentWaveEnemyCount = stageTemplate.easyWaves[currentWaveNumber].enemyCount;
				break;

			case GameManager.ModeType.isNormalMode:
				maximumWave = stageTemplate.normalWaves.Count;
				currentWaveEnemyCount = stageTemplate.normalWaves[currentWaveNumber].enemyCount;
				break;

			case GameManager.ModeType.isHardMode:
				maximumWave = stageTemplate.hardWaves.Count;
				currentWaveEnemyCount = stageTemplate.hardWaves[currentWaveNumber].enemyCount;
				break;
		}

		Debug.Log("Spawner NextSetup 실행");
		maximumEnemyCount += currentEnemyCount;
	}


	public IEnumerator StartWave()
	{
		currentEnemyCount = 0;

		Debug.Log(currentWaveEnemyCount);

		while (currentEnemyCount < currentWaveEnemyCount)
		{
			++currentEnemyCount;
			Debug.Log("게임 매니저 부름");
			// 적 오브젝트를 풀링 매니저에서 가져옵니다.
			GameObject temp_EnemyObject = PoolingManager.Instance.GetEnemyQueue();
			if (temp_EnemyObject != null)
			{
				GameManager.Instance.currentWaveLivingEnemy++;
				isWave = true;

				// 게임 모드에 따라 적 프리팹을 설정합니다.
				GameObject enemyPrefab = null;
				switch (GameManager.Instance.modeType)
				{
					case GameManager.ModeType.isEasyMode:
						enemyPrefab = stageTemplate.easyWaves[currentWaveNumber].enemyPrefab[Random.Range(0, stageTemplate.easyWaves[currentWaveNumber].enemyPrefab.Length)];
						break;
					case GameManager.ModeType.isNormalMode:
						enemyPrefab = stageTemplate.normalWaves[currentWaveNumber].enemyPrefab[Random.Range(0, stageTemplate.normalWaves[currentWaveNumber].enemyPrefab.Length)];
						break;
					case GameManager.ModeType.isHardMode:
						enemyPrefab = stageTemplate.hardWaves[currentWaveNumber].enemyPrefab[Random.Range(0, stageTemplate.hardWaves[currentWaveNumber].enemyPrefab.Length)];
						break;
				}

				if (enemyPrefab != null)
				{
					// 기존 오브젝트의 메쉬와 텍스처를 새 프리팹으로 교체합니다.
					var enemyRenderer = temp_EnemyObject.GetComponent<SpriteRenderer>();
					var prefabRenderer = enemyPrefab.GetComponent<SpriteRenderer>();
					if (enemyRenderer != null && prefabRenderer != null)
					{
						enemyRenderer.sprite = prefabRenderer.sprite;

					}

					// 적 템플릿을 로드합니다.
					string objectName = enemyPrefab.transform.name;
					temp_EnemyObject.name = objectName;
					EnemyTemplate enemyTemplate = Resources.Load<EnemyTemplate>($"Enemy Template/{objectName}");

					// 적의 체력을 설정합니다.
					var enemyHealth = temp_EnemyObject.GetComponent<EnemyHealth>();
					if (enemyHealth != null)
					{
						enemyHealth.MaxHealth = enemyTemplate.health;
						enemyHealth.Health = enemyTemplate.health;
					}

					temp_EnemyObject.GetComponent<Movement2D>().MoveSpeed = enemyTemplate.speed;
					// 기존 오브젝트의 위치를 초기화합니다.
					temp_EnemyObject.transform.position = Vector2.zero;
					temp_EnemyObject.SetActive(true);
				}

				// 적 스폰 속도에 따라 대기합니다.
				float temp_timer = 0;
				switch (GameManager.Instance.modeType)
				{
					case GameManager.ModeType.isEasyMode:
						temp_timer = Random.Range(stageTemplate.easyWaves[currentWaveNumber].enemySpawnRate / 1.5f, stageTemplate.easyWaves[currentWaveNumber].enemySpawnRate);
						break;
					case GameManager.ModeType.isNormalMode:
						temp_timer = Random.Range(stageTemplate.normalWaves[currentWaveNumber].enemySpawnRate / 1.5f, stageTemplate.normalWaves[currentWaveNumber].enemySpawnRate);
						break;
					case GameManager.ModeType.isHardMode:
						temp_timer = Random.Range(stageTemplate.hardWaves[currentWaveNumber].enemySpawnRate / 1.5f, stageTemplate.hardWaves[currentWaveNumber].enemySpawnRate);
						break;
				}
				yield return new WaitForSeconds(temp_timer);
			}
		}

		// 적들이 모두 처치될 때까지 기다립니다.
		while (GameManager.Instance.currentWaveLivingEnemy > 0)
		{
			yield return null;
		}

		GameManager.Instance.currentScore += 185;
		PlayerStatus.Instance.Coin += 1000;
		isWave = false;
		currentWaveNumber++;
		NextWaveSetting();
	}



	public virtual void NextWaveSetting()
	{
		if (currentWaveNumber >= maximumWave)
		{
			uiManager.OnVictory();
			Debug.Log("웨이브 넘음");
			return;
		}

		GameManager.Instance.currentScore += 300;
		uiManager.gameReadyGroup.SetActive(true);
		Debug.Log($"현재 웨이브 : {currentWaveNumber} 최대 웨이브 : {maximumWave}");
		NextSetup();
	}
}
