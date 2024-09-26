using UnityEngine;

public class HealthUIFollow : MonoBehaviour
{
	private Camera cam;
	private GameObject m_EnemyPrefab;
	private RectTransform m_HealthUIRectTransform;

	[SerializeField] private float positionCorrectionValuesUI = 2f;

	private void Awake()
	{
		cam = Camera.main;
	}

	public void FollowHealthUISetup(GameObject enemyPrefab, RectTransform healthUI)
	{
		m_EnemyPrefab = enemyPrefab;
		m_HealthUIRectTransform = healthUI;
	}

	private void Update()
	{
		if (m_EnemyPrefab != null && m_HealthUIRectTransform != null)
		{
			Vector2 screenPosition = cam.WorldToScreenPoint(m_EnemyPrefab.transform.position);
			m_HealthUIRectTransform.position = screenPosition + (Vector2.down * positionCorrectionValuesUI);
		}
	}
}
