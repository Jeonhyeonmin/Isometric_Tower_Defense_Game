using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy Template/Create Enemy Template", order = int.MaxValue)]
public class EnemyTemplate : ScriptableObject
{
	public Sprite sprite;
	public float health;
	public float speed;
	public float damage;
	public int rewardCoin;
	public int rewardScore;
	public int rewardCristal;
}
