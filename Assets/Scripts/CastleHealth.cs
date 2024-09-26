using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CastleHealth : MonoBehaviour
{
	public float currentHealth;

	[Header("성 속성")]
	[SerializeField] private float maxHealth;

	[Header("적의 공격 우선 순위대로 지정")]
	[SerializeField] private int castleNumber;

	private StageWaypoint stageWaypoint;

	private BoxCollider2D boxCollider2D;

	private Slider sliderHealth;

	private UIManager uIManager;

	private void Awake()
	{
		Setup();

		uIManager = FindAnyObjectByType<UIManager>();
	}

	private void Setup()
	{
		currentHealth = maxHealth;

		stageWaypoint = GameObject.FindAnyObjectByType<StageWaypoint>();

		boxCollider2D = GetComponent<BoxCollider2D>();

		// UI
		sliderHealth = GetComponent<Slider>();
	}

	private void Update()
	{
		FallDetection();
		UpdateHealthUI();
	}

	private void FallDetection()
	{
		if (stageWaypoint.currentEnemyLineNumber >= stageWaypoint.enemyLinesNumber)
		{
			uIManager.OnDefeat();
		}
	}

	private void UpdateHealthUI()
	{
		if (currentHealth <= 0) { return; }
		if (sliderHealth == null) { return; }

		sliderHealth.value = currentHealth / maxHealth;
	}

	private void TakeDamage(float _Damage)
	{
		Mathf.Clamp(currentHealth -= _Damage, 0, maxHealth);

		if (currentHealth <= 0)
		{
			FallenCastle();
		}
	}

	private void FallenCastle()
	{
		GameManager.Instance.currentScore -= 300;
		DisableCastle();

		if (stageWaypoint.currentEnemyLineNumber <= stageWaypoint.enemyLinesNumber)
		{
			stageWaypoint.currentEnemyLineNumber++;

			GameManager.Instance.castleFallAnimationNotify();
			stageWaypoint.NextEnemyPoints();
		}
	}

	private void DisableCastle()
	{
		boxCollider2D.enabled = false;
		sliderHealth.value = 0;
		sliderHealth.enabled = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.CompareTag("Enemy")) { return; }

		TakeDamage(25);

		GameManager.Instance.currentWaveLivingEnemy--;
		PoolingManager.Instance.InsertEnemyQueue(collision.gameObject);
	}
}
