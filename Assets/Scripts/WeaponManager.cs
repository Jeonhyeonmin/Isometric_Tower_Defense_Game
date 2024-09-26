using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    #region ���� ����

    // ===================
    [Header("������ ���̽� ���")] // ������ ���̽� ���
    // ===================

    [SerializeField] private GunDatabase[] gunDatabases;
    [SerializeField] private GunDatabase currentGunDatabase;
    [SerializeField] private GunDatabase pistolGunDatabase;
    [SerializeField] private GunDatabase rifleGunDatabase;
    [SerializeField] private GunDatabase sniperRifleGunDatabase;

    // ===================
    [Space(30)]
    [Header("UI ���")] // UI ���
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
    [Header("�ڼ��� ���� UI ���")]

    [SerializeField] private GameObject readMore_Panel;

    [SerializeField] private TMP_Text readMore_Title_Text;

    [SerializeField] private TMP_Text readMore_Damage_Ability_Text;
    [SerializeField] private TMP_Text readMore_Rate_Ability_Text;
    [SerializeField] private TMP_Text readMore_MagazineSize_Ability_Text;
    [SerializeField] private TMP_Text readMore_AimAccuracy_Ability_Text;
    [SerializeField] private TMP_Text readMore_EffectiveRange_Ability_Text;
    [SerializeField] private TMP_Text readMore_moveSpeed_Ability_Text;

    [Space(30)]
    [Header("���� UI ��ҵ�")]
    [SerializeField] private Sprite scopeSprite;
    [SerializeField] private Sprite barrelSprite;
    [SerializeField] private Sprite magazineSprite;
    [SerializeField] private Sprite gripSprite;
    [SerializeField] private Sprite stockSprite;

    // ===================
    [Space(30)]
    [Header("�ý��� �Ӽ� ���")] // �ý��� �Ӽ� ���
    // ===================

    // (0: ����, 1: ������, 2: �������� ������)
    [SerializeField] private int currentPage = 0;   // ���� ����, ������, �������� ������ ������ �� ��� ����������,
    [SerializeField] private int currentPageIndex = 0;
    [SerializeField] private int currentDatabaseMaxPageIndex = 0;

    [SerializeField] private int gunPartsCount;
    [SerializeField] private int gunActivePartsCount;

    // ===================
    // ��������Ʈ ���
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
    }   // �¾� �ż������ ��Ƶδ� �ż���

    private void SetupDelegate()
    {
        ChangePageDelegate += SettingCurrentPageIndexUI;
    }   // ��������Ʈ�� �۵��� ����ϴ� �ż���

    private void AutomationFindGunDatabase()
    {
        gunDatabases = Resources.LoadAll<GunDatabase>("Gun/GunDataBase");
        Debug.Log($"���� ������ ���̽��� {gunDatabases.Length}���� ã�ҽ��ϴ�.");

        currentGunDatabase = gunDatabases[0];
        currentPage = 0;
    }   // ���� �����ͺ��̽��� �ڵ����� ã���ִ� �ż��� (���� �� ����)

    private void Update()
    {
        brickCoinUpdate();
    }

    public void brickCoinUpdate()
    {
        brickCoin_Text.text = PlayerWalletManager.Instance.TotalBrick.ToString("#,##0");
    }   // �긯 ������ ������ �ǽð����� �����ϱ� ���� �ż���

    public void onClickMovementPages(int _PageIndex)
    {
        if (gunDatabases[_PageIndex].guns.Length <= 0)
        {
            Debug.LogError($"{gunDatabases[_PageIndex].databaseName}�� ���� �������� ��� �� ���Ⱑ �������� �ʽ��ϴ�. ��� : 'return'");
            return;
        }

        currentPage = _PageIndex;   // ���� �������� ��ư���� �޾ƿ� �ε��� ��ȣ�� �����Ѵ�.
        Debug.Log($"{currentPage}������ {_PageIndex}������ �������� �̵��մϴ�.");
        currentGunDatabase = gunDatabases[currentPage];
        Debug.Log($"{currentGunDatabase.name}���� ���� ������ ���̽��� �����߽��ϴ�.");

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

    }   // ����, ������, �������� ������ ��ư Ŭ�� �� ���� �Ǵ� �̺�Ʈ �ż���

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
            gunDescription_Price_Text.text = "<color=white> �ر� ��! </color>";
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
		gunParts_Text.text = gunPartsCount != 0 ? gunActivePartsCount != 0 ? $"{gunActivePartsCount}/{gunPartsCount} | {Mathf.RoundToInt((float)gunActivePartsCount / gunPartsCount * 100)}%" : $"0/{gunPartsCount} | 0%" : "���� ���� �Ұ�";

        foreach (Transform childTransform in gunPartsGroup)
        {
            Destroy(childTransform.gameObject);
        }

        if (currentGunDatabase.guns[currentPageIndex].isUnlocking)
        {
            #region ���� �˻�

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
            gunParts_Text.text = "<color=grey> �����ؾ� �� </color>";
            gunProgressGauge.fillAmount = 0;
        }

        Debug.Log("�ѱ��� UI ��� ������ ���ƽ��ϴ�."); 
    }   // ���� �� �������� UI�� ���Ӱ� �����ϴ� �ż���

    public void OnClickGunReadMore()
    {
        readMore_Panel.SetActive(true);

        readMore_Title_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunName} �ڼ��� ����";
        readMore_Damage_Ability_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunList[currentGunDatabase.guns[currentPageIndex].gunCurrentLevel].damage.ToString("#,##0")}";
        readMore_Rate_Ability_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunList[currentGunDatabase.guns[currentPageIndex].gunCurrentLevel].fireRate.ToString("#,##0")}";
        readMore_MagazineSize_Ability_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunList[currentGunDatabase.guns[currentPageIndex].gunCurrentLevel].magazineSize.ToString("#,##0")}";
        readMore_AimAccuracy_Ability_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunList[currentGunDatabase.guns[currentPageIndex].gunCurrentLevel].aimAccuracy.ToString("#,##0")}";
        readMore_EffectiveRange_Ability_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunList[currentGunDatabase.guns[currentPageIndex].gunCurrentLevel].effectiveRange.ToString("#,##0")}";
        readMore_moveSpeed_Ability_Text.text = $"{currentGunDatabase.guns[currentPageIndex].gunList[currentGunDatabase.guns[currentPageIndex].gunCurrentLevel].moveSpeed.ToString("#,##0")}";

        Debug.Log("���� �ڼ��� ���� UI ��� ������ ���ƽ��ϴ�.");
    }

    /// <summary>
    /// ���� ���⸦ ���� ���� �ε��� �̵��� ����ϴ� �̺�Ʈ �ż���
    /// (����: 0, ������: 1)
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
                    Debug.Log("������ �ε����� '0'���� �����߽��ϴ�.");
					SettingCurrentPageIndexUI();
					Debug.Log("���ο� ������ �ε����� UI�� �ҷ��Խ��ϴ�.");
                    isLock = true;
					return;
                }  
                
                currentPageIndex--;
                break;
            case 1:
                if (currentPageIndex + 1 >= currentDatabaseMaxPageIndex)
                {
                    currentPageIndex = currentDatabaseMaxPageIndex;
                    Debug.Log($"������ �ε����� {currentDatabaseMaxPageIndex}���� �����߽��ϴ�.");
					SettingCurrentPageIndexUI();
					Debug.Log("���ο� ������ �ε����� UI�� �ҷ��Խ��ϴ�.");
                    isLock = true;
					return;
                }
                    
                currentPageIndex++;
                break;
        }

        if (!isLock)
        {
			SettingCurrentPageIndexUI();
			Debug.Log("���ο� ������ �ε����� UI�� �ҷ��Խ��ϴ�.");
		}
    }

    public void OnClickBuyWeapon()
    {
        if (PlayerWalletManager.Instance.TotalBrick < currentGunDatabase.guns[currentPageIndex].price)
        {
            Debug.LogError($"{currentGunDatabase.guns[currentPageIndex].price}���� �긯 ������ �ʿ������� {PlayerWalletManager.Instance.TotalBrick}���� �ʿ��մϴ�.");
            return;
        }

        if (currentGunDatabase.guns[currentPageIndex].isUnlocking)
        {
            Debug.LogError($"{currentGunDatabase.guns[currentPageIndex].isUnlocking}�̹Ƿ� ��ȯ�մϴ�.");
            return;
        }    

        PlayerWalletManager.Instance.TotalBrick -= currentGunDatabase.guns[currentPageIndex].price;

        currentGunDatabase.guns[currentPageIndex].isUnlocking = true;
        Debug.Log($"{currentGunDatabase.guns[currentPageIndex].gunName}�� ���� ���Ⱑ ������� �Ǿ����ϴ�.");
	}
}
