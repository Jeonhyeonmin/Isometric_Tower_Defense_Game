using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum Difficulty
{
	None = 0,
	easy = 1,
	normal = 2,
	hard = 4,
}

[CreateAssetMenu(fileName = "Stage", menuName = "Stage Template/Create Stage Template", order = int.MaxValue)]
public class StageTemplate : ScriptableObject
{
	[Header("난이도 설정")]
	public Difficulty difficulty;

	[Header("스테이지 속성")]
	public string stageUIName;

	[Header("개발자 속성")]
	public string StageDeveloperName;

	[Space(30)]

	public List<Wave> easyWaves = new List<Wave>();
	public List<Wave> normalWaves = new List<Wave>();
	public List<Wave> hardWaves = new List<Wave>();

	[System.Serializable]
	public struct Wave
	{
		public GameObject[] enemyPrefab;
		public int enemyCount;
		public float enemySpawnRate;
	}
}
