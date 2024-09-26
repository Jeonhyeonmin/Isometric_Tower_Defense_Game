using UnityEngine;

public class EnemySprite : MonoBehaviour
{
	private void OnEnable()
	{
		string _objectName = transform.name.Replace("(Clone)", "");
		EnemyTemplate enemyTemplate = Resources.Load<EnemyTemplate>($"Enemy Template/{_objectName}");

		transform.GetComponent<SpriteRenderer>().sprite = enemyTemplate.sprite;
	}

	private void OnDisable()
	{
		transform.GetComponent<SpriteRenderer>().sprite = null;
	}
}
