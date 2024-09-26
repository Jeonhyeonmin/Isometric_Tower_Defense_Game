using TMPro;
using UnityEngine;

public class ViewerTowerConstruction : MonoBehaviour
{
	[Header("Archer Variables Group")]
	[SerializeField] private TowerTemplate towerArcherTemplate;
	[SerializeField] private int towerArcherBuildCost;
	[SerializeField] private TMP_Text towerArcherBuildCost_Text;
	[SerializeField] private GameObject towerArcher;

	[Space(30)]

	[Header("Wizard Variables Group")]
	[SerializeField] private TowerTemplate towerWizardTemplate;
	[SerializeField] private int towerWizardBuildCost;
	[SerializeField] private TMP_Text towerWizardBuildCost_Text;
	[SerializeField] private GameObject towerWizard;

	[Header("Barrack Variables Group")]
	[SerializeField] private TowerTemplate towerBarrackTemplate;
	[SerializeField] private int towerBarrackBuildCost;
	[SerializeField] private TMP_Text towerBarrackBuildCost_Text;
	[SerializeField] private GameObject towerBarrack;

	private PlaceTile placeTile;

	public PlaceTile PlaceTile
	{
		get => placeTile; set => placeTile = value;
	}

	private void Awake()
	{
		towerArcherBuildCost_Text.text = $"${towerArcherBuildCost.ToString()}";
		towerWizardBuildCost_Text.text = $"${towerWizardBuildCost.ToString()}";
		towerBarrackBuildCost_Text.text = $"${towerBarrackBuildCost.ToString()}";
	}

	public void OnBuildTowerArcher()
	{
		if (PlayerStatus.Instance.Coin <= towerArcherBuildCost)
		{
			Debug.Log("돈이 부족합니다.");
			return;
		}

		if (placeTile.IsTowerBuild)
		{
			Debug.Log("타워가 이미 설치되어 있습니다.");
			return;
		}

		Vector3 spawnPosition = placeTile.transform.position - new Vector3(0, 0.1f, 0);
		GameObject temp_Tower = Instantiate(towerArcher, spawnPosition, Quaternion.identity);
		temp_Tower.GetComponent<Tower>().GetPlaceTile = placeTile;

		if (placeTile != null)
		{
			placeTile.IsTowerBuild = true;
			placeTile.PlaceTowerTile();
		}

		PlayerStatus.Instance.Coin -= towerArcherBuildCost;
	}

	public void OnBuildTowerWizard()
	{
		if (PlayerStatus.Instance.Coin <= towerWizardBuildCost)
		{
			Debug.Log("돈이 부족합니다.");
			return;
		}

		if (placeTile.IsTowerBuild)
		{
			Debug.Log("타워가 이미 설치되어 있습니다.");
			return;
		}

		Vector3 spawnPosition = placeTile.transform.position - new Vector3(0, 0.1f, 0);
		GameObject temp_Tower = Instantiate(towerWizard, spawnPosition, Quaternion.identity);
		temp_Tower.GetComponent<Tower>().GetPlaceTile = placeTile;

		if (placeTile != null)
		{
			placeTile.IsTowerBuild = true;
			placeTile.PlaceTowerTile();
		}

		PlayerStatus.Instance.Coin -= towerWizardBuildCost;
	}

	public void OnBuildTowerBarrack()
	{
		if (PlayerStatus.Instance.Coin <= towerBarrackBuildCost)
		{
			Debug.Log("돈이 부족합니다.");
			return;
		}

		if (placeTile.IsTowerBuild)
		{
			Debug.Log("타워가 이미 설치되어 있습니다.");
			return;
		}

		Vector3 spawnPosition = placeTile.transform.position - new Vector3(0, 0.1f, 0);
		GameObject temp_Tower = Instantiate(towerBarrack, spawnPosition, Quaternion.identity);
		temp_Tower.GetComponent<Tower>().GetPlaceTile = placeTile;

		if (placeTile != null)
		{
			placeTile.IsTowerBuild = true;
			placeTile.PlaceTowerTile();
		}

		PlayerStatus.Instance.Coin -= towerBarrackBuildCost;
	}
}
