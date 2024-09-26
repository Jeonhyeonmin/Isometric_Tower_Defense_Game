using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Tower
{
	[SerializeField] private GameObject bulletPrefab;

	[SerializeField] private Animator anim;
	[SerializeField] private Transform centerAxisStick;
	[SerializeField] private Transform bulletFirePos;

	private List<GameObject> selectedTargets;

	[SerializeField] private int bulletPoolSize = 10;
	private Queue<GameObject> bulletObjectPool;

	private bool currentLockTarget;
	private bool currentSupportMode;

	[SerializeField] private LayerMask enemyLayerMask;
	[SerializeField] private LayerMask towerLayerMask;

	private Coroutine compoundCoroutine;

	private void Awake()
	{
		Setup();
	}

	public override void Setup()
	{
		base.Setup();

		bulletObjectPool = new Queue<GameObject>();

		for (int i = 0; i < bulletPoolSize; i++)
		{
			GameObject clone = Instantiate(bulletPrefab);
			clone.transform.parent = transform;
			clone.SetActive(false);
			bulletObjectPool.Enqueue(clone);
		}

		anim = GetComponent<Animator>();
		anim.SetBool("Idle", true);

		CurrentLevel = 0;
	}

	private GameObject GetBulletObject()
	{
		if (bulletObjectPool.Count > 0)
		{
			GameObject clone = bulletObjectPool.Dequeue();
			clone.SetActive(true);
			return clone;
		}
		else
		{
			GameObject clone = Instantiate(bulletPrefab);
			clone.transform.parent = transform;
			return clone;
		}
	}

	private void Update()
	{
		SearchTarget();
	}

	public void ReturnObject(GameObject obj)
	{
		obj.SetActive(false);
		bulletObjectPool.Enqueue(obj);
	}

	public override void SearchTarget()
	{
		Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(transform.position, Range, enemyLayerMask);
		Collider2D[] towerColliders = Physics2D.OverlapCircleAll(transform.position, Range, towerLayerMask);

		List<Collider2D> filteredEnemyColliders = new List<Collider2D>();
		List<Collider2D> filteredTowerColliders = new List<Collider2D>();

		foreach (Collider2D collider in enemyColliders)
		{
			if (collider.gameObject != gameObject)
			{
				filteredEnemyColliders.Add(collider);
			}
		}

		foreach (Collider2D collider in towerColliders)
		{
			if (collider.gameObject != gameObject)
			{
				filteredTowerColliders.Add(collider);
			}
		}

		if (filteredEnemyColliders.Count > 0 && filteredTowerColliders.Count <= 0)
		{
			if (!currentLockTarget)
			{
				selectedTargets = new List<GameObject> { filteredEnemyColliders[0].gameObject };
				currentLockTarget = true;

				if (compoundCoroutine == null)
				{
					compoundCoroutine = StartCoroutine(AttackRoutine());
				}
			}
			else
			{
				if (!ShouldLockTarget(filteredEnemyColliders.ToArray()))
				{
					selectedTargets = null;
					currentLockTarget = false;

					if (compoundCoroutine != null)
					{
						StopCoroutine(compoundCoroutine);
						compoundCoroutine = null;
					}
				}

				if (!ShouldLockTarget(filteredTowerColliders.ToArray()) && currentSupportMode)
				{
					selectedTargets = null;
					currentLockTarget = false;
					currentSupportMode = false;

					if (compoundCoroutine != null)
					{
						StopCoroutine(compoundCoroutine);
						compoundCoroutine = null;
					}
				}
			}
		}
		else if (filteredEnemyColliders.Count <= 0 && filteredTowerColliders.Count > 0)
		{
			if (!currentLockTarget)
			{
				selectedTargets = new List<GameObject>();

				foreach (Collider2D collider in filteredTowerColliders)
				{
					selectedTargets.Add(collider.gameObject);
				}

				currentLockTarget = true;
				currentSupportMode = true;

				if (compoundCoroutine == null)
				{
					for (int i = 0; i < selectedTargets.Count; i++)
					{
						if (selectedTargets[i].layer == 6)
							return;
						compoundCoroutine = StartCoroutine(SupportRoutine());
					}

				}
				else
				{
					StopCoroutine(compoundCoroutine);
					compoundCoroutine = null;
				}
			}
			else
			{
				UpdateSupportTargets(filteredTowerColliders.ToArray());
			}
		}
		else if (filteredEnemyColliders.Count > 0 && filteredTowerColliders.Count > 0)
		{
			if (!currentLockTarget)
			{
				if (compoundCoroutine != null)
				{
					StopCoroutine(compoundCoroutine);
					compoundCoroutine = null;
				}

				selectedTargets = new List<GameObject>();
				currentLockTarget = true;
				currentSupportMode = true;

				foreach (Collider2D collider in filteredTowerColliders)
				{
					selectedTargets.Add(collider.gameObject);
				}
				if (compoundCoroutine == null)
				{
					for (int i = 0; i < selectedTargets.Count; i++)
					{
						if (selectedTargets[i].layer == 6)
							return;
						compoundCoroutine = StartCoroutine(SupportRoutine());
					}
				}
				else
				{
					StopCoroutine(compoundCoroutine);
					compoundCoroutine = null;

					for (int i = 0; i < selectedTargets.Count; i++)
					{
						if (selectedTargets[i].layer == 6)
							return;
						compoundCoroutine = StartCoroutine(SupportRoutine());
					}
				}
			}
			else
			{
				UpdateSupportTargets(filteredTowerColliders.ToArray());
			}
		}
		else if (filteredEnemyColliders.Count <= 0 && filteredTowerColliders.Count <= 0)
		{
			if (currentLockTarget)
			{
				selectedTargets = null;
				currentLockTarget = false;

				if (compoundCoroutine != null)
				{
					StopCoroutine(compoundCoroutine);
					anim.SetBool("Idle", true);
					compoundCoroutine = null;
				}
			}
		}
	}

	private void UpdateSupportTargets(Collider2D[] filteredTowerColliders)
	{
		List<GameObject> newTargets = new List<GameObject>();

		foreach (Collider2D collider in filteredTowerColliders)
		{
			if (!selectedTargets.Contains(collider.gameObject))
			{
				newTargets.Add(collider.gameObject);
			}
		}

		currentSupportMode = true;

		if (newTargets.Count > 0)
		{
			selectedTargets.AddRange(newTargets);
		}

		int _RemoveLayer = 6;
		selectedTargets.RemoveAll(target => target.layer == _RemoveLayer);
	}

	public bool ShouldLockTarget(Collider2D[] colliders)
	{
		foreach (Collider2D collider in colliders)
		{
			if (selectedTargets.Contains(collider.gameObject))
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator AttackRoutine()
	{
		bool _RegisteringAnim = false;

		while (currentLockTarget && selectedTargets != null)
		{
			if (selectedTargets == null)
			{
				anim.SetBool("Idle", true);
				currentLockTarget = false;
				compoundCoroutine = null;
				yield break;
			}

			float attackRate = TowerTemplate.WeaponPropertie[CurrentLevel].rate;
			float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;

			anim.speed = animationLength / attackRate;

			if (!_RegisteringAnim)
			{
				yield return new WaitForSeconds(TowerResponseTime);
			}

			_RegisteringAnim = true;

			anim.SetBool("Idle", false);

			// 애니메이션이 완료될 때까지 대기
			yield return new WaitForSeconds(attackRate);

			SpawnAttackWizardBullet();

			anim.speed = 1.0f;
			anim.SetBool("Idle", true);

			if (selectedTargets == null)
			{
				anim.SetBool("Idle", true);
				currentLockTarget = false;
				compoundCoroutine = null;
				yield break;
			}

			// 다음 공격 준비
			yield return new WaitForSeconds(0.1f); // 약간의 지연 추가
		}
	}

	private IEnumerator SupportRoutine()
	{
		bool _RegisteringAnim = false;

		while (currentLockTarget && selectedTargets != null)
		{
			if (selectedTargets == null)
			{
				anim.SetBool("Idle", true);
				currentLockTarget = false;
				compoundCoroutine = null;
				yield break;
			}

			float attackRate = Rate;
			float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;

			anim.speed = animationLength / attackRate;

			if (!_RegisteringAnim)
			{
				yield return new WaitForSeconds(TowerResponseTime);
			}

			_RegisteringAnim = true;

			anim.SetBool("Idle", false);

			// 애니메이션이 완료될 때까지 대기
			yield return new WaitForSeconds(attackRate);

			foreach (var target in selectedTargets)
			{
				if (target != null)
				{
					SpawnSupportWizardBullet(target.transform);
				}
			}

			anim.speed = 1.0f;
			anim.SetBool("Idle", true);

			if (selectedTargets == null)
			{
				anim.SetBool("Idle", true);
				currentLockTarget = false;
				compoundCoroutine = null;
				yield break;
			}

			// 다음 지원 준비
			yield return new WaitForSeconds(0.1f); // 약간의 지연 추가
		}
	}

	public void SpawnAttackWizardBullet()
	{
		if (selectedTargets == null || selectedTargets.Count == 0)
		{
			Debug.Log("타겟 못찾음");
			return;
		}

		foreach (var target in selectedTargets)
		{
			GameObject bullet = GetBulletObject();
			bullet.transform.position = bulletFirePos.position;
			Bullet bulletScript = bullet.GetComponent<Bullet>();
			bulletScript.AttackSetup(target.transform, TowerTemplate.WeaponPropertie[CurrentLevel].damage);
		}
	}

	public void SpawnSupportWizardBullet(Transform target)
	{
		if (target == null)
		{
			Debug.Log("타겟 못찾음");
			return;
		}

		GameObject bullet = GetBulletObject();
		bullet.transform.position = bulletFirePos.position;
		Bullet bulletScript = bullet.GetComponent<Bullet>();
		bulletScript.SupportSetup(target, Support);
	}
}
