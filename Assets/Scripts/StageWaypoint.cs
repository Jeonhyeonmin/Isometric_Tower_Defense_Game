using System.Linq;
using UnityEngine;

public class StageWaypoint : MonoBehaviour
{
	[SerializeField] private Transform[] enemyLinesGroup;
	[SerializeField] private Transform[] enemyPoints;
	public int enemyLinesNumber;
	public int currentEnemyLineNumber;
	public int currentEnemyLine = 0;

	public Transform[] EnemyPoints => enemyPoints;

	private void Awake()
	{
		GameObject[] _EnemyLines = GameObject.FindGameObjectsWithTag("EnemyLine");
		enemyLinesGroup = new Transform[_EnemyLines.Length];
		enemyLinesNumber = _EnemyLines.Length;

		for (int i = 0; i < enemyLinesNumber; i++)
		{
			string _EnemyLineName = $"Enemy Line ({i + 1})";
			GameObject _EnemyLine = GameObject.Find(_EnemyLineName);

			if (_EnemyLine == null)
				continue;

			enemyLinesGroup[i] = _EnemyLine.transform;
		}

		NextEnemyPoints();
	}

	public void NextEnemyPoints()
	{
		if (currentEnemyLine >= enemyLinesGroup.Length)
		{
			return;
		}

		currentEnemyLine++;
		Transform[] _TemporaryEnemyPoint = enemyLinesGroup[currentEnemyLine - 1].GetComponentsInChildren<Transform>();
		enemyPoints = _TemporaryEnemyPoint.Where(t => t != enemyLinesGroup[currentEnemyLine - 1]).ToArray();
	}
}
