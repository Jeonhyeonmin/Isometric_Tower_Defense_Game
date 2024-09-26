using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private Movement2D m_Movement2D;

	public Transform target;

	private float damage;
	private float support;

	private bool supportMode;

	private void Awake()
	{
		m_Movement2D = GetComponent<Movement2D>();
	}

	private void OnEnable()
	{
		if (transform.parent != null)
		{
			Invoke("ReturnObjectAfterDelay", 6f);
		}
	}

	private void ReturnObjectAfterDelay()
	{
		transform.GetComponentInParent<Wizard>().ReturnObject(gameObject);
	}

	public float Damage
	{
		set => damage = Mathf.Clamp(damage, 0, float.MaxValue);
		get => damage;
	}

	public float Support
	{
		set => support = Mathf.Clamp(support, 0, float.MaxValue);
		get => support;
	}

	public void AttackSetup(Transform _EnemyTarget, float _Damage)
	{
		target = _EnemyTarget;
		damage = _Damage;
	}

	public void SupportSetup(Transform _TowerTarget, float _Support)
	{
		target = _TowerTarget;
		support = _Support;
		supportMode = true;
	}

	private void Update()
	{
		if (target == null)
			return;

		if (!target.gameObject.activeSelf)
		{
			if (transform.parent != null && transform.parent.GetComponent<Barrack>() == null)
			{
				transform.GetComponentInParent<Wizard>().ReturnObject(gameObject);
			}
		}
	}

	private void FixedUpdate()
	{
		if (target != null)
		{
			Vector2 moveDirection = (target.position - transform.position).normalized;
			transform.up = moveDirection;
			m_Movement2D.MoveTo(moveDirection);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Tower") && collision.transform == target)
		{
			supportMode = true;
		}

		if (!supportMode)
		{
			if (collision.transform == target)
			{
				EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
				enemyHealth.TakeDamage(Damage);

				transform.GetComponentInParent<Wizard>().ReturnObject(gameObject);
			}
		}
		else
		{
			if (collision.transform == target)
			{
				Tower _Tower = collision.GetComponent<Tower>();

				SupportBuffTower supportBuffTower = transform.parent.GetComponentInParent<SupportBuffTower>();
				supportBuffTower.AddTower(_Tower, Support);

				if (transform.parent != null)
				{
					transform.GetComponentInParent<Wizard>().ReturnObject(gameObject);
				}
			}
		}
	}

	private void OnDisable()
	{
		target = null;
		damage = 0;
		supportMode = false;
		m_Movement2D.MoveTo(Vector2.zero);
	}
}