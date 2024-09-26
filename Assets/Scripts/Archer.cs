using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Tower
{
	[SerializeField] private GameObject bulletPrefab;

	[SerializeField] private Animator anim;
	[SerializeField] private Transform centerAxisBow;
	[SerializeField] private Transform arrowFirePos;

	private GameObject selectedTarget = null;

	[SerializeField] private int bulletPoolSize = 10;
	private Queue<GameObject> bulletObjectPool;

	private bool currentLockTarget;

	private Coroutine attackCoroutine;

	public LayerMask layerMask;

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
			return clone;
		}
	}

	public void ReturnObject(GameObject obj)
	{
		obj.SetActive(false);
		bulletObjectPool.Enqueue(obj);
	}


	private void Update()
	{
		SearchTarget();
		RotateToTarget();
	}

	public override void SearchTarget()
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Range, layerMask);

		if (colliders.Length > 0)
		{
			if (!currentLockTarget)
			{
				selectedTarget = colliders[0].gameObject;
				currentLockTarget = true;
				if (attackCoroutine == null)
				{
					attackCoroutine = StartCoroutine(AttackRoutine());
				}
			}
			else
			{
				if (!ShouldLockTarget(colliders))
				{
					selectedTarget = null;
					currentLockTarget = false;

					if (attackCoroutine != null)
					{
						StopCoroutine(attackCoroutine);
						attackCoroutine = null;
					}
				}
			}
		}
		else
		{
			if (currentLockTarget)
			{
				selectedTarget = null;
				currentLockTarget = false;

				if (attackCoroutine != null)
				{
					StopCoroutine(attackCoroutine);
					anim.SetBool("Idle", true);
					attackCoroutine = null;
				}
			}
		}
	}

	public bool ShouldLockTarget(Collider2D[] colliders)
	{
		foreach (Collider2D collider in colliders)
		{
			if (selectedTarget == collider.gameObject)
			{
				return true;
			}
		}
		return false;
	}


	private void RotateToTarget()
	{
		if (selectedTarget == null)
			return;

		float dx = selectedTarget.transform.position.x - transform.position.x;
		float dy = selectedTarget.transform.position.y - transform.position.y;

		float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
		centerAxisBow.rotation = Quaternion.Euler(0, 0, degree + 180f);
	}


	private IEnumerator AttackRoutine()
	{
		bool _RegisteringAnim = false;

		while (currentLockTarget && selectedTarget != null)
		{
			if (selectedTarget == null)
			{
				anim.SetBool("Idle", true);
				currentLockTarget = false;
				attackCoroutine = null;
				yield break;
			}

			float attackRate = Rate;
			float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;

			// 애니메이션 속도를 공격 속도에 맞추기
			anim.speed = animationLength / attackRate;

			if (!_RegisteringAnim)
			{
				yield return new WaitForSeconds(TowerResponseTime);
			}

			_RegisteringAnim = true;

			anim.SetBool("Idle", false);

			// 애니메이션이 완료될 때까지 대기
			yield return new WaitForSeconds(attackRate);

			SpawnArcherArrow();

			anim.speed = 1.0f;
			anim.SetBool("Idle", true);

			if (selectedTarget == null)
			{
				anim.SetBool("Idle", true);
				currentLockTarget = false;
				attackCoroutine = null;
				yield break;
			}

			// 다음 공격 준비
			yield return new WaitForSeconds(0.1f); // 약간의 지연 추가
		}
	}


	public void SpawnArcherArrow()
	{
		if (selectedTarget == null)
		{
			Debug.Log("타겟 못찾음");
			return;
		}

		GameObject arrow = GetBulletObject();
		arrow.transform.position = arrowFirePos.position;
		Arrow arrowScript = arrow.GetComponent<Arrow>();
		arrowScript.Setup(selectedTarget.transform, Damage);
	}
}
