using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
	[SerializeField] private float health;
	[SerializeField] private float maxHealth;

	public float Health
	{
		set => health = value;
		get => health;
	}
	public float MaxHealth
	{
		set => maxHealth = value;
		get => maxHealth;
	}


	public void TakeDamage(float damageAmount)
	{
		health -= damageAmount;
		health = Mathf.Clamp(health, 0, maxHealth);
	}

	public void TakeHeal(float healAmount)
	{
		health += healAmount;
		health = Mathf.Clamp(health, 0, maxHealth);
	}

	private void Update()
	{
		OnDie();
	}

	private void OnDie()
	{
		if (health <= 0)
		{
			GameManager.Instance.currentWaveLivingEnemy--;
			GameManager.Instance.currentkillEnemyCount++;
			PlayerStatus.Instance.Coin += GameManager.Instance.GetEnemyTemplate(gameObject.name).rewardCoin;
			PlayerStatus.Instance.IsGetCoin = true;
			GameManager.Instance.currentScore += GameManager.Instance.GetEnemyTemplate(gameObject.name).rewardScore;

			int _ranInt = Random.Range(0, 6);

			switch (_ranInt)
			{
				case 0:
				case 1:
					{
						PlayerStatus.Instance.Crystal += GameManager.Instance.GetEnemyTemplate(gameObject.name).rewardCristal;
						PlayerStatus.Instance.IsGetCrystal = true;
						Debug.Log("Å©¸®½ºÅ» È¹µæ");
					}
					break;
			}
			
			PoolingManager.Instance.InsertEnemyQueue(gameObject);
		}
	}
}
