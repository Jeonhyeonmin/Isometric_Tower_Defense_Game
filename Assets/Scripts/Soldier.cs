using System.Collections;
using UnityEngine;

public class Soldier : MonoBehaviour
{
	[SerializeField] private Animator anim;

	[SerializeField] private Movement2D m_Movement2D;

	public Transform target;

	private Coroutine attackCoroutine;

	private float damage;

	public bool isAttack;

	[SerializeField] private LayerMask enemyLayerMask;

	public float Damage
	{
		set => damage = Mathf.Clamp(damage, 0, float.MaxValue);
		get => damage;
	}

	private void Awake()
	{
		m_Movement2D = GetComponent<Movement2D>();
	}

	void FixedUpdate()
	{
		if (target == null)
		{
			return;
		}

		Vector2 moveDirection = (target.position - transform.position).normalized;
		float distanceToTarget = Vector2.Distance(transform.position, target.position);

		if (moveDirection.x != 0 && !isAttack)
		{
			if (moveDirection.x > 0)
			{
				transform.localScale = new Vector3(-1, 1, 1);
			}
			else
			{
				transform.localScale = new Vector3(1, 1, 1);
			}
		}

		if (distanceToTarget > transform.GetComponent<Movement2D>().MinDistance)
		{
			m_Movement2D.MoveTo(moveDirection);
		}
		else
		{
			m_Movement2D.MoveTo(Vector2.zero);
		}

		if (target != null && m_Movement2D.MoveSpeed == 0)
		{
			anim.SetBool("Idle", true);
			anim.SetBool("Walk", false);
		}
		else
		{
			anim.SetBool("Idle", false);
			anim.SetBool("Walk", true);
		}

		Targetdetection();
	}

	private void Update()
	{
		CoroutineManager();
	}

	private void CoroutineManager()
	{
		if (target == null || !target.gameObject.activeSelf)
		{
			StartCoroutine(TargetLost());
		}
	}

	private IEnumerator TargetLost()
	{
		if (attackCoroutine != null)
		{
			StopCoroutine(attackCoroutine);
			attackCoroutine = null;
		}

		m_Movement2D.MoveSpeed = 0;

		anim.SetBool("Idle", true);
		anim.SetBool("Walk", false);
		anim.speed = 1.0f;
		anim.SetTrigger("Return");

		yield return new WaitForSeconds(1.30f);

		transform.GetComponentInParent<Barrack>().ReturnObject(gameObject);
	}

	private void Targetdetection()
	{
		Collider2D[] collider2D = Physics2D.OverlapCircleAll(transform.position, transform.GetComponent<Movement2D>().MinDistance, enemyLayerMask);

		foreach (Collider2D collider in collider2D)
		{
			if (collider.gameObject == target.gameObject)
			{
				if (attackCoroutine == null)
				{
					attackCoroutine = StartCoroutine(AttackRoutine());
				}
			}
		}
	}

	public void FollowSetup(Transform _EnemyTarget, float _Damage)
	{
		target = _EnemyTarget;
		damage = _Damage;
		m_Movement2D.MoveTo(Vector2.zero);
		transform.position = transform.parent.position;
	}

	private IEnumerator AttackRoutine()
	{
		if (transform.parent.GetComponent<Barrack>() == null)
		{
			Debug.Log("부모 오브젝트에 Barrack 컴포넌트가 존재하지 않습니다.");
		}

		isAttack = true;

		anim.SetTrigger("Attack");

		m_Movement2D.MoveSpeed = 0;

		float attackRate = transform.parent.GetComponent<Barrack>().Rate;
		float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;

		// 애니메이션 속도를 공격 속도에 맞추기
		anim.speed = animationLength / attackRate;

		// 애니메이션이 완료될 때까지 대기
		yield return new WaitForSeconds(attackRate);

		if (target != null)
		{
			EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
			enemyHealth.TakeDamage(Damage);
		}

		anim.SetBool("Idle", true);

		yield return new WaitForSeconds(0.5f);

		anim.speed = 1.0f;
		anim.SetTrigger("Return");

		yield return new WaitForSeconds(1.30f);

		transform.GetComponentInParent<Barrack>().ReturnObject(gameObject);

		isAttack = false;
	}

	private void OnEnable()
	{
		string objectName = transform.name.Replace("(Clone)", "");
		m_Movement2D.MoveTo(Vector2.zero);
		m_Movement2D.MoveSpeed = GameManager.Instance.GetBulletTemplate($"{objectName}").speed;
		anim.ResetTrigger("Attack");
		anim.ResetTrigger("Return");
		anim.SetBool("Idle", true);
		anim.SetBool("Walk", false);
		isAttack = false;
	}

	private void OnDisable()
	{
		target = null;
		damage = 0.0f;
		m_Movement2D.MoveTo(Vector2.zero);
		m_Movement2D.MoveSpeed = 0;
		target = null;
		anim.ResetTrigger("Attack");
		anim.ResetTrigger("Return");
		anim.SetBool("Idle", true);
		anim.SetBool("Walk", false);
		attackCoroutine = null;
		isAttack = false;
		StopAllCoroutines();
	}
}
