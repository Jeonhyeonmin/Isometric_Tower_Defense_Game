using UnityEngine;

public class SaveScoreManager : SingletonManager<SaveScoreManager>
{
    public Sprite oneStar;
	public Sprite twoStar;
	public Sprite threeStar;

    private const string StageScoreKey = "StageScore_";
    private const string StageStarKey = "StageStar_";

    public void SetScore(string stageName, int score)
    {
        if (score > int.MaxValue)
        {
            Debug.LogError("시스템 에러 : 점수 저장 최대치를 넘겼습니다.");
            return;
        }

        if (PlayerPrefs.GetInt(StageScoreKey + score) > score)
		{
			Debug.LogError("이전 점수보다 현재 점수가 더 적습니다.");
			return;
		}

		PlayerPrefs.SetInt(StageScoreKey + stageName, score);
        PlayerPrefs.Save();
    }

    public void SetStars(string stageName, int starCount)
    {
        if (starCount < 0 || starCount > 3)
        {
            Debug.LogError("별 개수는 0과 3 사이여야 합니다.");
            return;
        }

        if (PlayerPrefs.GetInt(StageStarKey + stageName) > starCount)
        {
            Debug.LogError("이전 별 개수보다 현재 별 개수가 더 적습니다.");
            return;
        }

        PlayerPrefs.SetInt(StageStarKey + stageName, starCount);
        PlayerPrefs.Save();
    }

    public int GetScore(string stageName)
    {
        return PlayerPrefs.GetInt(StageScoreKey + stageName, 0);
    }

    public int GetStars(string stageName)
    {
        return PlayerPrefs.GetInt(StageStarKey + stageName, 0);
    }
}
