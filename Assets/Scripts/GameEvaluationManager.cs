using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameEvaluationManager : MonoBehaviour
{
    private static GameEvaluationManager instance;

    public static GameEvaluationManager Instance
	{
		get => instance ?? (instance = new GameEvaluationManager());
		set => instance = value;
	}

	[SerializeField] private PlayerStatus playerStatus;
	[SerializeField] private EnemySpawner enemySpawner;
	[SerializeField] private List<CastleHealth> castleHealths = new List<CastleHealth>();

	private bool isCastleFall;

	private void Awake()
	{
		instance = this;
		playerStatus = FindAnyObjectByType<PlayerStatus>();
		enemySpawner = FindAnyObjectByType<EnemySpawner>();
		castleHealths = FindObjectsByType<CastleHealth>(FindObjectsSortMode.None).ToList();
	}

	private void Update()
	{

	}

	public int StarEvaluationCheck()
	{
		int _starPoint = 0;

		if (playerStatus == null)
		{
			Debug.LogError("PlayerStatus를 찾을 수 없었습니다.");
			return 0;
		}

		#region OneStar

		if (playerStatus.Coin >= 9999)
		{
			_starPoint++;
		}

		#endregion

		#region TwoStar

		if (enemySpawner.currentEnemyCount <= 0 && GameManager.Instance.currentkillEnemyCount == enemySpawner.maximumEnemyCount)
		{
			_starPoint++;
		}

		#endregion

		#region threeStar

		foreach(CastleHealth castleHealth in castleHealths)
		{
			if (castleHealth.currentHealth <= 0)
			{
				isCastleFall = true;
			}
		}

		if (!isCastleFall)
		{
			_starPoint++;
		}

		return _starPoint;

		#endregion
	}
}
