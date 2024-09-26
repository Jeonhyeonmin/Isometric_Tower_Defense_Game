using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WayPointDirection : MonoBehaviour
{
	[SerializeField] private Transform currentEnemyPoint_TF;
	[SerializeField] private int currentEnemyPoint;

	private bool registerEnemy = false;

	private StageWaypoint stageWaypoint;
	private Movement2D movement2D;

	private void Awake()
	{
		GameObject stageWayPointObject = GameObject.Find("Stage_Waypoint");
		stageWaypoint = stageWayPointObject.GetComponent<StageWaypoint>();
		movement2D = GetComponent<Movement2D>();
	}

	private void Start()
	{
		InitializeWaypoints();

		registerEnemy = true;
	}

	private void OnEnable()
	{
		if (registerEnemy)
		{
			InitializeWaypoints();
		}
	}

	private void Update()
	{
		if (stageWaypoint == null || currentEnemyPoint_TF == null)
			return;

		CheckWaypointDistance();
	}

	private void InitializeWaypoints()
	{
		if (stageWaypoint.EnemyPoints.Length == 0)
		{
			Debug.LogError("세팅 된 EnemyPoint가 존재하지 않습니다.");
			return;
		}

		currentEnemyPoint = 0;
		currentEnemyPoint_TF = stageWaypoint.EnemyPoints[0];
		transform.position = currentEnemyPoint_TF.localPosition;

		UpdateMovementDirection();
		MoveToNextWaypoint();
	}

	private void MoveToNextWaypoint()
	{
		if (currentEnemyPoint >= stageWaypoint.EnemyPoints.Length)
			return;

		currentEnemyPoint++;
	}

	private void UpdateMovementDirection()
	{
		if (currentEnemyPoint >= stageWaypoint.EnemyPoints.Length)
			return;

		currentEnemyPoint_TF = stageWaypoint.EnemyPoints[currentEnemyPoint];
		Vector2 direction = (currentEnemyPoint_TF.position - transform.position).normalized;
		movement2D.MoveTo(direction);
	}

	private void CheckWaypointDistance()
	{
		Vector2 position2D = new Vector2(transform.position.x, transform.position.y);
		Vector2 enemyPoint2D = new Vector2(currentEnemyPoint_TF.position.x, currentEnemyPoint_TF.position.y);
		float distance = Vector2.Distance(position2D, enemyPoint2D);

		if (distance <= 0.05f * movement2D.MoveSpeed)
		{
			transform.position = currentEnemyPoint_TF.position;
			UpdateMovementDirection();
			MoveToNextWaypoint();
		}
	}

	private void OnDisable()
	{
		currentEnemyPoint = 0;
		currentEnemyPoint_TF = null;
		transform.position = Vector3.zero;
		movement2D.MoveTo(Vector2.zero);
	}
}
