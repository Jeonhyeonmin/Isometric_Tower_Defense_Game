using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Bullet Template/Create Bullet Template", order = int.MaxValue)]
public class BulletTemplate : ScriptableObject
{
	public float speed;
	public float minDistance;
}
