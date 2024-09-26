using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewerTowerInfo : MonoBehaviour
{
	public static ViewerTowerInfo Instance;

	private GameObject targetObject;

	public TMP_Text infoTitle_text;
	public Image towerSprite;
	public Image towerTypeSprite;
	public TMP_Text towerType_text;
	public GameObject upgradeInfo;
	public TMP_Text towerUpgradeCost_Text;
	public TMP_Text attackPower_Status;
	public TMP_Text attackSpeed_Status;

	public Button demolition_Button;
	public Button Upgrade_Button;

	[Header("TowerType Sprite")]
	[SerializeField] private Sprite shieldIcon;
	[SerializeField] private Sprite swordIcon;

	private void Awake()
	{
		Instance = this;
	}

	public void RefreshInterface(GameObject _TowerObject)
	{
		Tower _Tower = _TowerObject.GetComponent<Tower>();

		if (_Tower != null)
		{
			targetObject = _TowerObject;

			TowerTemplate _TowerTemplate = _Tower.towerTemplate;

			if (_TowerTemplate != null)
			{
				infoTitle_text.text = _TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].name;
				towerSprite.sprite = _TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].sprite;
				towerType_text.text = _TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].towerType.ToString();

				Vector2 nativeSize;
				RectTransform rectTransform;
				switch (_TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].towerType)
				{
					case TowerTemplate.TowerType.공격타워:
						towerType_text.text = _TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].towerType.ToString();
						towerTypeSprite.sprite = swordIcon;
						nativeSize = new Vector2(swordIcon.rect.width, swordIcon.rect.height);
						rectTransform = towerTypeSprite.GetComponent<RectTransform>();
						rectTransform.sizeDelta = nativeSize;
						break;
					case TowerTemplate.TowerType.방어타워:
						towerType_text.text = _TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].towerType.ToString();
						towerTypeSprite.sprite = shieldIcon;
						nativeSize = new Vector2(shieldIcon.rect.width, shieldIcon.rect.height);
						rectTransform = towerTypeSprite.GetComponent<RectTransform>();
						rectTransform.sizeDelta = nativeSize;
						break;
					case TowerTemplate.TowerType.지원타워:
						towerType_text.text = _TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].towerType.ToString();
						break;
				}

				if (_Tower.CurrentLevel + 1 >= _Tower.MaxLevel)
				{
					upgradeInfo.SetActive(false);
					towerUpgradeCost_Text.text = string.Empty;
					Upgrade_Button.interactable = false;
				}
				else
				{
					upgradeInfo.SetActive(true);
					towerUpgradeCost_Text.text = _TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].upgradeCost.ToString("N0");
					Upgrade_Button.interactable = true;
					Upgrade_Button.GetComponentInChildren<TextMeshProUGUI>().text = "업그레이드";
				}

				if (PlayerStatus.Instance.Coin < _TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].upgradeCost)
				{
					upgradeInfo.SetActive(true);
					towerUpgradeCost_Text.text = _TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].upgradeCost.ToString("N0");
					Upgrade_Button.interactable = false;
					Upgrade_Button.GetComponentInChildren<TextMeshProUGUI>().text = "자원 부족";
				}

				attackPower_Status.text = _Tower.Damage.ToString("F2");
				attackSpeed_Status.text = _Tower.Rate.ToString("F2");
			}
		}
	}

	public void TowerDemolition()
	{
		Tower _Tower = targetObject.GetComponent<Tower>();

		if (_Tower != null)
		{
			_Tower.GetPlaceTile.IsTowerBuild = false;
			Destroy(targetObject);
			gameObject.SetActive(false);
		}
	}

	public void TowerUpgrade()
	{
		Tower _Tower = targetObject.GetComponent<Tower>();

		if (_Tower != null)
		{
			TowerTemplate _TowerTemplate = _Tower.towerTemplate;

			if (_Tower.CurrentLevel + 1 >= _Tower.MaxLevel)
			{
				return;
			}

			if (_TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].upgradeCost <= PlayerStatus.Instance.Coin)
			{
				PlayerStatus.Instance.Coin -= _TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].upgradeCost;

				// 타워 업그레이드 (버프 재적용 포함)
				// 레벨 업
				if (_Tower.CurrentLevel + 1 < _Tower.MaxLevel)
				{
					_Tower.CurrentLevel++;
				}

				_Tower.SetupBaseStats();
				_Tower.InitializeRuntimeStats();
				_Tower.UpgradeTower();

				RefreshInterface(targetObject);
				targetObject.GetComponent<SpriteRenderer>().sprite = _TowerTemplate.WeaponPropertie[_Tower.CurrentLevel].sprite;

				switch (_Tower.towerType)
				{
					case TowerType.Archer:
						targetObject.name = $"Archer-Level-{_Tower.CurrentLevel + 1}(Clone)";
						break;
				}

				Debug.Log("결제완료");
			}
		}
	}


	private void OnDisable()
	{
		targetObject = null;
	}
}
