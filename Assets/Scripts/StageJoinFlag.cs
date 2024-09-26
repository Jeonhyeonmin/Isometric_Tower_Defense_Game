using UnityEngine;
using UnityEngine.SceneManagement;

public class StageJoinFlag : MonoBehaviour
{
	public string stageName;

	public int flagScore;
	public int flagStars;

	private void OnEnable()
	{
		RefreshGameMetrics();
	}

	public void RefreshGameMetrics()
	{
		flagScore = SaveScoreManager.Instance.GetScore(stageName);
		flagStars = SaveScoreManager.Instance.GetStars(stageName);

		RefreshUserInterface();
	}

	private void RefreshUserInterface()
	{
		SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		
		switch (flagStars)
		{
			case 0:
				break;
			case 1:
				spriteRenderers[1].sprite = SaveScoreManager.Instance.oneStar;
				break;
			case 2:
				spriteRenderers[1].sprite = SaveScoreManager.Instance.twoStar;
				break;
			case 3:
				spriteRenderers[1].sprite = SaveScoreManager.Instance.threeStar;
				break;
		}
	}
}
