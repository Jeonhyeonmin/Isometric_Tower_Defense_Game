using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Viewer_EnemyHealthBar : MonoBehaviour
{
	public GameObject m_EnemyPrefab;
	public RectTransform m_Slider;

	private Slider healthSlider;

	private void OnEnable()
	{
		StopCoroutine(TryGetComponentSlider());
		StartCoroutine(TryGetComponentSlider());
	}

	private IEnumerator TryGetComponentSlider()
	{
		while (true)
		{
			if (m_Slider != null)
			{
				yield return null;
				healthSlider = m_Slider.GetComponent<Slider>();

				if (healthSlider != null)
					break;
			}

			else
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
	}

	private void Update()
	{
		EnemyHealth temp_EnemyHealth = m_EnemyPrefab.GetComponent<EnemyHealth>();
		if (temp_EnemyHealth != null)
		{
			float healthRatio = temp_EnemyHealth.Health / temp_EnemyHealth.MaxHealth;
			if (healthSlider != null)
			{
				healthSlider.value = healthRatio;
			}
		}
	}

	private void OnDisable()
	{
		StopCoroutine(TryGetComponentSlider());
	}
}
