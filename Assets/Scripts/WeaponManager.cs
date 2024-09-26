using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    #region 무기 변수

    // ===================
    [Header("데이터 베이스 요소")] // 데이터 베이스 요소
    // ===================

    [SerializeField] private GunDatabase[] gunDatabases;
    [SerializeField] private GunDatabase currentGunDatabase;
    [SerializeField] private GunDatabase pistolGunDatabase;
    [SerializeField] private GunDatabase rifleGunDatabase;
    [SerializeField] private GunDatabase sniperRifleGunDatabase;

    // ===================
    [Space(30)]
    [Header("UI 요소")] // UI 요소
    // ===================

    [SerializeField] private TMP_Text brickCoin_Text;

    [Space(20)]

    [SerializeField] private TMP_Text panelTitle_Text;

    [SerializeField] private TMP_Text gunType_Text;
    [SerializeField] private TMP_Text gunTitle_Text;

    [SerializeField] private Image gunIcon_Image;

    [SerializeField] private TMP_Text gunDescription_Title_Text;
    [SerializeField] private TMP_Text gunDescription_SubTitle_Text;
    [SerializeField] private TMP_Text gunDescription_Price_Text;

    [SerializeField] private Transform menuCircleGrid_TF;
    [SerializeField] private GameObject nullCircleImage;
    [SerializeField] private GameObject seletedCircleImage;

    [SerializeField] private List<Image> selectedWeaponList;
	[SerializeField] private List<Button> selectedWeaponBGList;
    [SerializeField] private Sprite selectedBoxSprite;
    [SerializeField] private Sprite normalBoxSprite;

    [SerializeField] private Image gunPartsGauge;

    [SerializeField] private TMP_Text gunParts_Text;
    [SerializeField] private Image gunProgressGauge;

    [SerializeField] private Transform gunPartsGroup;

    [SerializeField] private GameObject unLockPartButton;
    [SerializeField] private GameObject lockPartButton;

	[Space(30)]
    [Header("자세히 보기 UI 요소")]

    [SerializeField] private GameObject readMore_Panel;

    [SerializeField] private TMP_Text readMore_Title_Text;

    [SerializeField] private TMP_Text readMore_Damage_Ability_Text;
    [SerializeField] private TMP_Text readMore_Rate_Ability_Text;
    [SerializeField] private TMP_Text readMore_MagazineSize_Ability_Text;
    [SerializeField] private TMP_Text readMore_AimAccuracy_Ability_Text;
    [SerializeField] private TMP_Text readMore_EffectiveRange_Ability_Text;
    [SerializeField] private TMP_Text readMore_moveSpeed_Ability_Text;

    [Space(30)]
    [Header("파츠 UI 요소들")]
    [SerializeField] private Sprite scopeSprite;
    [SerializeField] private Sprite barrelSprite;
    [SerializeField] private Sprite magazineSprite;
    [SerializeField] private Sprite gripSprite;
    [SerializeField] private Sprite stockSprite;

    // ===================
    [Space(30)]
    [Header("시스템 속성 요소")] // 시스템 속성 요소
    // ===================

    // (0: 권총, 1: 라이플, 2: 스나이퍼 라이플)
    [SerializeField] private int currentPage = 0;   // 현재 권총, 라이플, 스나이퍼 라이플 페이지 중 어느 페이지인지,
    [SerializeField] private int currentPageIndex = 0;
    [SerializeField] private int currentDatabaseMaxPageIndex = 0;

    [SerializeField] private int gunPartsCount;
    [SerializeField] private int gunActivePartsCount;

    // ===================
    // 델리게이트 요소
    // ===================

    private delegate void ChangePage();
    private ChangePage ChangePageDelegate;

    #endregion

    private void Start()
    {
        Setup(); 
        AutomationFindGunDatabase();
        SettingCurrentPageIndexUI();
    }

    private void Setup()
    {
        SetupDelegate();
    }   // 셋업 매서드들을 모아두는 매서드

    private void SetupDelegate()
    {
        ChangePageDelegate += SettingCurrentPageIndexUI;
    }   // 델리게이트의 작동을 담당하는 매서드

    private void AutomationFindGunDatabase()
    {
        gunDatabases = Resources.LoadAll<GunDatabase>("Gun/GunDataBase");
        Debug.Log($"무기 데이터 베이스를 {gunDatabases.Length}개를 찾았습니다.");

        currentGunDatabase = gunDatabases[0];
        currentPage = 0;
    }   // 무기 데이터베이스를 자동으로 찾아주는 매서드 (수동 시 제외)

    private void Update()
    {
        brickCoinUpdate();
    }

    public void brickCoinUpdate()
    {
        brickCoin_Text.text = PlayerWalletManager.Instance.TotalBrick.ToString("#,##0");
    }   // 브릭 코인의 개수를 실시간으로 연동하기 위한 매서드

    public void onClickMovementPages(int _PageIndex)
    {
        if (gunDatabases[_PageIndex].guns.Length <= 0)
        {
            Debug.LogError($"{gunDatabases[_PageIndex].databaseName}의 상점 페이지에 등록 된 무기가 존재하지 않습니다. 결과 : 'return'");
            return;
        }

        currentPage = _PageIndex;   // 현재 페이지를 버튼에서 받아온 인덱스 번호로 변경한다.
        Debug.Log($"{currentPage}번에서 {_PageIndex}번으로 페이지를 이동합니다.");
        currentGunDatabase = gunDatabases[currentPage];
        Debug.Log($"{currentGunDatabase.name}으로 현재 데이터 베이스를 변경했습니다.");

        currentPageIndex = 0;

        if (ChangePageDelegate != null)
        {
            ChangePageDelegate();
        }

        switch (currentPage)
        {
            case 0:
                selectedWeaponList[0].gameObject.SetActive(true);
                selectedWeaponList[1].gameObject.SetActive(false);
                selectedWeaponList[2].gameObject.SetActive(false);
                selectedWeaponBGList[0].GetComponent<Image>().sprite = selectedBoxSprite;
				selectedWeaponBGList[1].GetComponent<Image>().sprite = normalBoxSprite;
				selectedWeaponBGList[2].GetComponent<Image>().sprite = normalBoxSprite;
				break;
			case 1:
				selectedWeaponList[0].gameObject.SetActive(false);
				selectedWeaponList[1].gameObject.SetActive(true);
				selectedWeaponList[2].gameObject.SetActive(false);
				selectedWeaponBGList[0].GetComponent<Image>().sprite = normalBoxSprite;
				selectedWeaponBGList[1].GetComponent<Image>().sprite = selectedBoxSprite;
				selectedWeaponBGList[2].GetComponent<Image>().sprite = normalBoxSprite;
				break;
            case 2:
				selectedWeaponList[0].gameObject.SetActive(false);
				selectedWeaponList[1].gameObject.SetActive(false);
				selectedWeaponList[2].gameObject.SetActive(true);
				selectedWeaponBGList[0].GetComponent<Image>().sprite = normalBoxSprite;
				selectedWeaponBGList[1].GetComponent<Image>().sprite = normalBoxSprite;
				selectedWeaponBGList[2].GetComponent<Image>().sprite = selectedBoxSprite;
				break;
        }

    }   // 권총, 라이플, 스나이퍼 라이플 버튼 클릭 시 실행 되는 이벤트 매서드

    private void SettingCurrentPageIndexUI()
    {
		currentDatabaseMaxPageIndex = currentGunDatabase.guns.Length;

		panelTitle_Text.text = currentGunDatabase.databaseName;

        gunType_Text.text = currentGunDatabase.gunTypeName;
        gunTitle_Text.text = currentGunDatabase.guns[currentPageIndex].gunName;
        gunIcon_Image.sprite = currentGunDatabase.guns[currentPageIndex].gunIcon;
        gunIcon_Image.SetNativeSize();

        gunDescription_Title_Text.text = currentGunDatabase.guns[currentPageIndex].gunDescriptionTitle;
        gunDescription_SubTitle_Text.text = currentGunDatabase.guns[currentPageIndex].gunDescriptionSubTitle;

        if (currentGunDatabase.guns[currentPageIndex].isUnlocking)
        {
            gunDescription_Price_Text.text = "<color=white> 해금 됨! </color>";
        }
        else
        {
            gunDescription_Price_Text.text = currentGunDatabase.guns[currentPageIndex].price.ToString("#,##0");
        }

        foreach (Transform childObject in menuCircleGrid_TF)
        {
            Destroy(childObject.gameObject);
        }
        
        for (int i = 0; i <  currentGunDatabase.guns.Length; i++)
        {
            if (i == currentPageIndex)
            {
                Instantiate(seletedCircleImage, menuCircleGrid_TF);
            }
            else
            {
                Instantiate(nullCircleImage, menuCircleGrid_TF);
            }
        }

        bool[] parts = new[] { 
        currentGunDatabase.guns[currentPageIndex].haveScope,
        currentGunDatabase.guns[currentPageIndex].haveBarrel,
        currentGunDatabase.guns[currentPageIndex].haveMagazine,
        currentGunDatabase.guns[currentPageIndex].haveGrip,
        currentGunDatabase.guns[currentPageIndex].haveStock};

        gunPartsCount = parts.Count(part => part);

		bool[] activeParts = new[] {
		currentGunDatabase.guns[currentPageIndex].isUnlockScope,
		currentGunDatabase.guns[currentPageIndex].isUnlockBarrel,
		currentGunDatabase.guns[currentPageIndex].isUnlockMagazine,
		currentGunDatabase.guns[currentPageIndex].isUnlockGrip,
		currentGunDatabase.guns[currentPageIndex].isUnlockStock};

        gunActivePartsCount = activeParts.Count(part => part);

		float fillAmount = gunPartsCount != 0 ? gunActivePartsCount != 0 ? (float)gunActivePartsCount / gunPartsCount : 0 : 1;

		gunPartsGauge.fillAmount = fillAmount;
		gunProgressGauge.fillAmount = fillAmount;
		gunParts_Text.text = gunPartsCount != 0 ? gunActivePartsCount != 0 ? $"{gunActivePartsCount}/{gunPartsCount} | {Mathf.RoundToInt((float)gunActivePartsCount / gunPartsCount * 100)}%" : $"0/{gunPartsCount} | 0%" : "파츠 장착 불가";

        foreach (Transform childTransform in gunPartsGroup)
        {
            Destroy(childTransform.gameObject);
        }

        if (currentGunDatabase.guns[currentPageIndex].isUnlocking)
        {
            #region 파츠 검사

            if (currentGunDatabase.guns[currentPageIndex].haveScope)
            {
                if (currentGunDatabase.guns[currentPageIndex].isUnlockScope)
                {
                    GameObject _unLockPartButton = Instantiate(unLockPartButton, gunPartsGroup);
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].sprite = scopeSprite;
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].SetNativeSize();
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].transform.localScale = new Vector3(0.46966f, 0.46966f, 0.46966f);

                }
                else
                {
                    GameObject _lockPartButton = Instantiate(lockPartButton, gunPartsGroup);
                    _lockPartButton.GetComponentsInChildren<Image>()[1].sprite = scopeSprite;
                    _lockPartButton.GetComponentsInChildren<Image>()[1].SetNativeSize();
                    _lockPartButton.GetComponentsInChildren<Image>()[1].transform.localScale = new Vector3(0.46966f, 0.46966f, 0.46966f);
                    _lockPartButton.GetComponentsInChildren<Image>()[1].color = new Color32(139, 139, 138, 255);
                    Debug.Log(53536363);
                }
            }

            if (currentGunDatabase.guns[currentPageIndex].haveBarrel)
            {
                if (currentGunDatabase.guns[currentPageIndex].isUnlockBarrel)
                {
                    GameObject _unLockPartButton = Instantiate(unLockPartButton, gunPartsGroup);
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].sprite = barrelSprite;
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].SetNativeSize();
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                }
                else
                {
                    GameObject _lockPartButton = Instantiate(lockPartButton, gunPartsGroup);
                    _lockPartButton.GetComponentsInChildren<Image>()[1].sprite = barrelSprite;
                    _lockPartButton.GetComponentsInChildren<Image>()[1].SetNativeSize();
                    _lockPartButton.GetComponentsInChildren<Image>()[1].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                    _lockPartButton.GetComponentsInChildren<Image>()[1].color = new Color32(139, 139, 138, 255);
                }
            }

            if (currentGunDatabase.guns[currentPageIndex].haveMagazine)
            {
                if (currentGunDatabase.guns[currentPageIndex].isUnlockMagazine)
                {
                    GameObject _unLockPartButton = Instantiate(unLockPartButton, gunPartsGroup);
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].sprite = magazineSprite;
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].SetNativeSize();
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
                }
                else
                {
                    GameObject _lockPartButton = Instantiate(lockPartButton, gunPartsGroup);
                    _lockPartButton.GetComponentsInChildren<Image>()[1].sprite = magazineSprite;
                    _lockPartButton.GetComponentsInChildren<Image>()[1].SetNativeSize();
                    _lockPartButton.GetComponentsInChildren<Image>()[1].transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
                    _lockPartButton.GetComponentsInChildren<Image>()[1].color = new Color32(139, 139, 138, 255);
                }
            }

            if (currentGunDatabase.guns[currentPageIndex].haveGrip)
            {
                if (currentGunDatabase.guns[currentPageIndex].isUnlockGrip)
                {
                    GameObject _unLockPartButton = Instantiate(unLockPartButton, gunPartsGroup);
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].sprite = gripSprite;
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].SetNativeSize();
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].transform.localScale = new Vector3(0.41f, 0.41f, 0.41f);
                }
                else
                {
                    GameObject _lockPartButton = Instantiate(lockPartButton, gunPartsGroup);
                    _lockPartButton.GetComponentsInChildren<Image>()[1].sprite = gripSprite;
                    _lockPartButton.GetComponentsInChildren<Image>()[1].SetNativeSize();
                    _lockPartButton.GetComponentsInChildren<Image>()[1].transform.localScale = new Vector3(0.41f, 0.41f, 0.41f);
                    _lockPartButton.GetComponentsInChildren<Image>()[1].color = new Color32(139, 139, 138, 255);
                }
            }

            if (currentGunDatabase.guns[currentPageIndex].haveStock)
            {
                if (currentGunDatabase.guns[currentPageIndex].isUnlockStock)
                {
                    GameObject _unLockPartButton = Instantiate(unLockPartButton, gunPartsGroup);
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].sprite = stockSprite;
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].SetNativeSize();
                    _unLockPartButton.GetComponentsInChildren<Image>()[1].transform.localScale = new Vector3(0.54f, 0.54f, 0.54f);
                }
                else
                {
                    GameObject _lockPartButton = Instantiate(lockPartButton, gunPartsGroup);
                    _lockPartButton.GetComponentsInChildren<Image>()[1].sprite = stockSprite;
                    _lockPartButton.GetComponentsInChildren<Image>()[1].SetNativeSize();
                    _lockPartButton.GetComponentsInChildren<Image>()[1].transform.localScale = new Vector3(0.54f, 0.54f, 0.54f);
                    _lockPartButton.GetComponentsInChildren<Image>()[1].color = new Color32(139, 139, 138, 255);
                }
            }

            #endregion
        }
        else
        {
            gunPartsGauge.fillAmount = 0;
            gunParts_Text.text = "<color=grey> 구매해야 함 </color>";
            gunProgressGauge.fillAmount = 0;
        }

        Debug.Log("총기의 UI 요소 변경을 마쳤습니다."); 
    }   // 변경 될 페이지의 UI를 새롭게 구축하는 매서드

    public void OnClickGunReadMore()
    {
        readMore_Panel.SetActive(true);

        readMore_Title_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunName} 자세히 보기";
        readMore_Damage_Ability_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunList[currentGunDatabase.guns[currentPageIndex].gunCurrentLevel].damage.ToString("#,##0")}";
        readMore_Rate_Ability_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunList[currentGunDatabase.guns[currentPageIndex].gunCurrentLevel].fireRate.ToString("#,##0")}";
        readMore_MagazineSize_Ability_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunList[currentGunDatabase.guns[currentPageIndex].gunCurrentLevel].magazineSize.ToString("#,##0")}";
        readMore_AimAccuracy_Ability_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunList[currentGunDatabase.guns[currentPageIndex].gunCurrentLevel].aimAccuracy.ToString("#,##0")}";
        readMore_EffectiveRange_Ability_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunList[currentGunDatabase.guns[currentPageIndex].gunCurrentLevel].effectiveRange.ToString("#,##0")}";
        readMore_moveSpeed_Ability_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunList[currentGunDatabase.guns[currentPageIndex].gunCurrentLevel].moveSpeed.ToString("#,##0")}";

        Debug.Log("무기 자세히 보기 UI 요소 변경을 마쳤습니다.");
    }

    /// <summary>
    /// 다음 무기를 보기 위해 인덱스 이동을 담당하는 이벤트 매서드
    /// (왼쪽: 0, 오른쪽: 1)
    /// </summary>
    /// <param name="_Direction"></param>
    public void OnClickNextIndex(int _Direction)
    {
        bool isLock = false;

        switch (_Direction)
        {
            case 0:
                if (currentPageIndex - 1 <= 0)
                {
                    currentPageIndex = 0;
                    Debug.Log("페이지 인덱스를 '0'으로 조정했습니다.");
					SettingCurrentPageIndexUI();
					Debug.Log("새로운 페이지 인덱스의 UI를 불러왔습니다.");
                    isLock = true;
					return;
                }  
                
                currentPageIndex--;
                break;
            case 1:
                if (currentPageIndex + 1 >= currentDatabaseMaxPageIndex)
                {
                    currentPageIndex = currentDatabaseMaxPageIndex;
                    Debug.Log($"페이지 인덱스를 {currentDatabaseMaxPageIndex}으로 조정했습니다.");
					SettingCurrentPageIndexUI();
					Debug.Log("새로운 페이지 인덱스의 UI를 불러왔습니다.");
                    isLock = true;
					return;
                }
                    
                currentPageIndex++;
                break;
        }

        if (!isLock)
        {
			SettingCurrentPageIndexUI();
			Debug.Log("새로운 페이지 인덱스의 UI를 불러왔습니다.");
		}
    }

    public void OnClickBuyWeapon()
    {
        if (PlayerWalletManager.Instance.TotalBrick < currentGunDatabase.guns[currentPageIndex].price)
        {
            Debug.LogError($"{currentGunDatabase.guns[currentPageIndex].price}개의 브릭 코인이 필요하지만 {PlayerWalletManager.Instance.TotalBrick}개가 필요합니다.");
            return;
        }

        if (currentGunDatabase.guns[currentPageIndex].isUnlocking)
        {
            Debug.LogError($"{currentGunDatabase.guns[currentPageIndex].isUnlocking}이므로 반환합니다.");
            return;
        }    

        PlayerWalletManager.Instance.TotalBrick -= currentGunDatabase.guns[currentPageIndex].price;

        currentGunDatabase.guns[currentPageIndex].isUnlocking = true;
        Debug.Log($"{currentGunDatabase.guns[currentPageIndex].gunName}인 현재 무기가 잠금해제 되었습니다.");
	}
}
