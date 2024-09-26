using UnityEngine;

public class Arrow : MonoBehaviour
{
	[SerializeField] private Movement2D m_Movement2D;

	public Transform enemyTarget;

	private float damage;

	private void Awake()
	{
		m_Movement2D = GetComponent<Movement2D>();
	}

	private void OnEnable()
	{
		Invoke("ReturnObjectAfterDelay", 6f);
	}

	private void ReturnObjectAfterDelay()
	{
		transform.GetComponentInParent<Archer>().ReturnObject(gameObject);
	}

	public float Damage
	{
		set => damage = Mathf.Clamp(damage, 0, float.MaxValue);
		get => damage;
	}

	public void Setup(Transform _EnemyTarget, float _Damage)
	{
		enemyTarget = _EnemyTarget;
		damage = _Damage;
	}

	private void Update()
	{
		if (!enemyTarget.gameObject.activeSelf)
		{
			transform.GetComponentInParent<Archer>().ReturnObject(gameObject);
		}
	}

	private void FixedUpdate()
	{
		if (enemyTarget != null)
		{
			Vector2 moveDirection = (enemyTarget.position - transform.position).normalized;
			transform.up = moveDirection;
			m_Movement2D.MoveTo(moveDirection);
		}
		else
		{
			transform.GetComponentInParent<Archer>().ReturnObject(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.CompareTag("Enemy") && !collision.CompareTag("Tower") || collision.transform != enemyTarget)
		{
			return;
		}

		EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
		enemyHealth.TakeDamage(Damage);
		transform.GetComponentInParent<Archer>().ReturnObject(gameObject);
	}

	private void OnDisable()
	{
		enemyTarget = null;
		damage = 0;
		m_Movement2D.MoveTo(Vector2.zero);
	}
}
