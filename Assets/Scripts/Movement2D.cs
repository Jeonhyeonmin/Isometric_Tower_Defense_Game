using UnityEngine;

public enum ObjectType
{
	Enemy,
	Bullet
}

[RequireComponent(typeof(Collider2D))]
public class Movement2D : MonoBehaviour
{
	public ObjectType objectType;

	[Header("Movement Properties")]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float minDistance;

	private Vector2 moveDirection = Vector2.zero;

	public float MoveSpeed
	{
		set => moveSpeed = Mathf.Max(0, value);
		get => moveSpeed;
	}

	public float MinDistance
	{
		set => minDistance = Mathf.Min(0, float.MaxValue);
		get => minDistance;
	}

	private void FixedUpdate()
	{
		ProcessMovement();
	}

	public void MoveTo(Vector2 _Direction)
	{
		moveDirection = _Direction;
	}

	private void ProcessMovement()
	{
		transform.position += (Vector3)moveDirection * moveSpeed * Time.fixedDeltaTime;
	}

	private void OnEnable()
	{
		if (objectType == ObjectType.Bullet)
		{
			string _tempName = transform.name;
			_tempName = _tempName.Replace("(Clone)", "").Trim();
			moveSpeed = Resources.Load<BulletTemplate>($"Bullet Template/{_tempName}").speed;
		}
	}

	private void OnDisable()
	{
		moveSpeed = 0;
		minDistance = 0;
		moveDirection = Vector2.zero;
	}
}
