using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyHealthUISetup : MonoBehaviour
{
	[SerializeField] private RectTransform sliderEnemyHP;

	private void Awake()
	{
		sliderEnemyHP = Instantiate(sliderEnemyHP, GameObject.FindGameObjectWithTag("Canvas").transform);
		Setup();
	}

	private void Setup()
	{
		HealthUIFollow temp_HealthUIFollow = sliderEnemyHP.GetComponent<HealthUIFollow>();
		Viewer_EnemyHealthBar temp_ViewerEnemyHealthBar = sliderEnemyHP.GetComponent<Viewer_EnemyHealthBar>();

		temp_HealthUIFollow.FollowHealthUISetup(gameObject, sliderEnemyHP);

		temp_ViewerEnemyHealthBar.m_EnemyPrefab = gameObject;
		temp_ViewerEnemyHealthBar.m_Slider = sliderEnemyHP;
	}

	private void OnEnable()
	{
		if (sliderEnemyHP != null)
			sliderEnemyHP.gameObject.SetActive(true);
	}

	private void OnDisable()
	{
		if (sliderEnemyHP != null)
			sliderEnemyHP.gameObject.SetActive(false);
	}
}
