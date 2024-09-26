using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : Tower
{
	[SerializeField] private GameObject soldierPrefab;
	[SerializeField] private Transform soldierFirePos;

	private List<GameObject> selectedTargets;

	[SerializeField] private int soldierPoolSize = 30;
	private Queue<GameObject> soldierObjectPool;

	private bool currentLockTarget;

	private Coroutine attackCoroutine;

	public LayerMask enemyLayerMask;

	public override void SearchTarget()
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Range, enemyLayerMask);

		if (colliders.Length > 0)
		{
			if (!currentLockTarget)
			{
				selectedTargets = new List<GameObject>();

				foreach (Collider2D collider in colliders)
				{
					selectedTargets.Add(collider.gameObject);
				}

				currentLockTarget = true;

				if (attackCoroutine == null)
				{
					attackCoroutine = StartCoroutine(AttackRoutine());
				}
			}
			else
			{
				UpdateTargets(colliders);

				if (!ShouldLockTarget(colliders))
				{
					selectedTargets = null;
					currentLockTarget = false;

					if (attackCoroutine != null)
					{
						StopCoroutine(attackCoroutine);
						attackCoroutine = null;
					}
				}

				if (attackCoroutine == null)
				{
					attackCoroutine = StartCoroutine(AttackRoutine());
				}
			}
		}
		else
		{
			if (currentLockTarget)
			{
				selectedTargets = null;
				currentLockTarget = false;

				if (attackCoroutine != null)
				{
					StopCoroutine(attackCoroutine);
					attackCoroutine = null;
				}
			}
		}
	}

	private void UpdateTargets(Collider2D[] enemyColliders)
	{
		// 임시 리스트를 사용하여 삭제할 타겟을 추적합니다.
		List<GameObject> targetsToRemove = new List<GameObject>();

		// 현재 타겟 리스트를 순회하며 범위 밖에 있는 타겟을 찾습니다.
		foreach (GameObject target in selectedTargets)
		{
			bool isStillInRange = false;

			foreach (Collider2D collider in enemyColliders)
			{
				if (collider.gameObject == target)
				{
					isStillInRange = true;
					break;
				}
			}

			if (!isStillInRange)
			{
				targetsToRemove.Add(target);
			}
		}

		// 범위를 벗어난 타겟을 제거합니다.
		foreach (GameObject targetToRemove in targetsToRemove)
		{
			selectedTargets.Remove(targetToRemove);
		}

		// 새롭게 범위 안에 들어온 적을 추가합니다.
		foreach (Collider2D collider in enemyColliders)
		{
			if (!selectedTargets.Contains(collider.gameObject))
			{
				selectedTargets.Add(collider.gameObject);
			}
		}
	}


	private void Awake()
	{
		Setup();
	}

	public override void Setup()
	{
		base.Setup();

		soldierObjectPool = new Queue<GameObject>();

		for (int i = 0; i < soldierPoolSize; i++)
		{
			GameObject clone = Instantiate(soldierPrefab);
			clone.transform.parent = transform;
			clone.SetActive(false);
			soldierObjectPool.Enqueue(clone);
		}

		CurrentLevel = 0;
	}

	private GameObject GetSoldierObject()
	{
		if (soldierObjectPool.Count > 0)
		{
			GameObject clone = soldierObjectPool.Dequeue();
			clone.SetActive(true);
			return clone;
		}
		else if (soldierObjectPool.Count <= 0 && transform.childCount < AvailableSoldiers)
		{
			GameObject clone = Instantiate(soldierPrefab);
			clone.transform.parent = transform;
			return clone;
		}

		return null;
	}

	public void ReturnObject(GameObject obj)
	{
		obj.SetActive(false);
		soldierObjectPool.Enqueue(obj);
	}

	private void Update()
	{
		SearchTarget();
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

		while (currentLockTarget && selectedTargets != null && selectedTargets.Count > 0)
		{
			if (selectedTargets == null || selectedTargets.Count == 0)
			{
				currentLockTarget = false;
				attackCoroutine = null;
				yield break;
			}

			if (!_RegisteringAnim)
			{
				yield return new WaitForSeconds(TowerResponseTime);
			}

			_RegisteringAnim = true;

			// 활성화된 자식 오브젝트의 개수를 계산
			int activeChildrenCount = 0;
			foreach (Transform child in transform)
			{
				if (child.gameObject.activeSelf)
				{
					activeChildrenCount++;
				}
			}

			// 활성화된 자식 오브젝트의 개수가 AvailableSoldiers를 초과하면 리턴
			if (activeChildrenCount >= AvailableSoldiers)
			{
				StopCoroutine(attackCoroutine);
				attackCoroutine = null;
				yield break;
			}

			// 무작위로 타겟 선택
			int randomIndex = Random.Range(0, selectedTargets.Count);
			GameObject randomTarget = selectedTargets[randomIndex];

			if (randomTarget != null)
			{
				SpawnAttackBarrackSoldier(randomTarget.transform);
				yield return new WaitForSeconds(Rate);
			}

			if (selectedTargets == null || selectedTargets.Count == 0)
			{
				currentLockTarget = false;
				attackCoroutine = null;
				yield break;
			}

			// 다음 지원 준비
			yield return new WaitForSeconds(0.1f); // 약간의 지연 추가
		}
	}

	public void SpawnAttackBarrackSoldier(Transform target)
	{
		if (target == null)
		{
			Debug.Log("타겟을 찾을 수 없습니다.");
			return;
		}

		GameObject solider = GetSoldierObject();

		if (solider == null)
		{
			Debug.Log("사용 가능한 병사가 없습니다.");
			return;
		}

		solider.transform.position = soldierFirePos.position;
		Soldier soldierScript = solider.GetComponent<Soldier>();
		soldierScript.FollowSetup(target, Damage);
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, Range);
	}
}
