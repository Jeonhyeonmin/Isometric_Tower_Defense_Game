using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
	public static PoolingManager Instance;

	public GameObject enemyPrefab = null;
	public GameObject healthPrefabs = null;

	public Queue<GameObject> enemyQueue = new Queue<GameObject>();

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		for (int i = 0; i < 1000; i++)
		{
			GameObject temp_EnemyObject = Instantiate(enemyPrefab, Vector2.zero, Quaternion.identity);

			enemyQueue.Enqueue(temp_EnemyObject);
			temp_EnemyObject.transform.parent = transform;
			temp_EnemyObject.transform.position = Vector2.zero;
			temp_EnemyObject.SetActive(false);
		}
	}

	public void InsertEnemyQueue(GameObject prefab_Object)
	{
		enemyQueue.Enqueue(prefab_Object);
		prefab_Object.SetActive(false);
	}

	public GameObject GetEnemyQueue()
	{
		if (enemyQueue.Count == 0)
		{
			GameObject temp_EnemyObject = Instantiate(enemyPrefab, Vector2.zero, Quaternion.identity);
			temp_EnemyObject.transform.parent = transform;
			temp_EnemyObject.transform.position = Vector3.zero;
			InsertEnemyQueue(temp_EnemyObject);
		}

		GameObject temp_object = enemyQueue.Dequeue();
		temp_object.SetActive(true);
		return temp_object;
	}
}
