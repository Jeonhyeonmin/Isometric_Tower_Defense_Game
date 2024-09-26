using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventorySystem : MonoBehaviour
{
    // ===============
    [Header("데이터베이스 요소 ---")]
    // 데이터베이스 요소
    // ===============

    [Header("스킬 데이터 베이스 요소")]
    [SerializeField] private ItemDatabase[] skillDatabases;  // 스킬의 데이터 베이스들

    [Space(10)]

    [Header("무기 데이터 베이스 요소")]
    [SerializeField] private GunDatabase[] gunDatabases;    // 무기의 데이터 베이스들

    // ===============
    [Space(30)]
    [Header("UI 요소 ---")]
    // UI 요소
    // ===============

    [Header("인벤토리 상단 요소")]
    [SerializeField] private Button inventoryQuit_Button;

    [SerializeField] private GameObject topInventory_BG_Object;

    [SerializeField] private Image target_Item_Image;
    [SerializeField] private TMP_Text target_Item_Title_Text;
    [SerializeField] private TMP_Text target_ITem_SubTitle_Text;
	[SerializeField] private SubItem target_SubItem;

    [SerializeField] private Button equip_Item_Button;
    [SerializeField] private Button upgrade_Item_Button;
    [SerializeField] private TMP_Text upgrade_ItemPrice_Text;
    [SerializeField] private Button throw_Item_Button;
    [Space(30)]

    [Header("인벤토리 중앙 요소")]
    [SerializeField] private GameObject inventorySubItem_Group;

    [SerializeField] private GameObject inventorySubItemExpansion_Group;
    [SerializeField] private GameObject inventorySubItem_Object;

	[SerializeField] private GameObject[] inventorySubItemExpansion_Button_Group;
	[Space(30)]

    [Header("인벤토리 하단 요소")]
    [SerializeField] private Button sorting_Button;
    [SerializeField] private bool isSkillFirst;

	[SerializeField] private TMP_Text inventoryCount_Text;
	[SerializeField] private int currentInventoryMaximumCount = 15;
    [Space(30)]

    [Header("인벤토리 사이드 요소")]
    [SerializeField] private Button sideAll_Button;
    [SerializeField] private Button sideSkill_Button;
    [SerializeField] private Button sideWeapon_Button;
    [Space(30)]

    [Header("인벤토리 정비 요소")]
    [SerializeField] private GameObject equipManagement_Object;

    [SerializeField] private Image skillEquip_Compartment01_Image;
    [SerializeField] private Image skillEquip_Compartment02_Image;
    [SerializeField] private Image skillEquip_Compartment03_Image;

    [SerializeField] private Image weaponEquip_Compartment01_Image;

    [SerializeField] private Button equipManagement_Quit_Button;

	[SerializeField] private GameObject skillEquip_Manager;
	[SerializeField] private GameObject weaponEquip_Manager;
    [Space(30)]

    // ==================
    [Header("시스템 요소 ---")]
    // 시스템 요소
    // ==================

    public int currentInventoryCategory;

    private void Awake()
    {
        FindAutomationDatabases();
    }

    private void OnEnable()
    {
        SetupInterface();
        SetupButtonAddListener();
    }

	private void LateUpdate()
	{
		InventoryCount();
	}

	private void FindAutomationDatabases()
    {
        #region 스킬 데이터 베이스

        skillDatabases = Resources.LoadAll<ItemDatabase>("Item/ItemDataBase");
        Debug.Log($"{skillDatabases.Length}개의 스킬 데이터 베이스를 찾았습니다.");

        #endregion

        #region 무기 데이터 베이스

        gunDatabases = Resources.LoadAll<GunDatabase>("Gun/GunDataBase");
        Debug.Log($"{gunDatabases.Length}개의 무기 데이터 베이스를 찾았습니다.");

        #endregion
    }

    private void SetupInterface()
    {
        #region 인벤토리 상단 UI

        target_Item_Image.sprite = null;
        target_Item_Title_Text.text = string.Empty;
        target_ITem_SubTitle_Text.text = string.Empty;
        currentInventoryCategory = 0;
		inventoryQuit_Button.transform.localPosition = new Vector3(531.4f, 325f, 0f);

		InventoryExpansion();

		topInventory_BG_Object.SetActive(false);

        #endregion

        #region 인벤토리 중앙

        foreach (Transform childTransform in inventorySubItem_Group.transform)
        {
            Destroy(childTransform.gameObject);
        }
        Debug.Log("남아있던 SubItem 오브젝트를 제거했습니다.");

		for (int i = 0; i < skillDatabases.Length; i++)
		{
			foreach (Item item in skillDatabases[i].items)
			{
				if (item.isUnlocking)
				{
					GameObject _tempSubItem_Object = Instantiate(inventorySubItem_Object, inventorySubItem_Group.transform);

					_tempSubItem_Object.GetComponentsInChildren<Image>()[1].sprite = item.SkillIcon;
					_tempSubItem_Object.GetComponent<SubItem>().itemType = ItemType.Skill;
					_tempSubItem_Object.GetComponent<SubItem>().itemData = item;
					_tempSubItem_Object.GetComponent<SubItem>().itemSprite = item.SkillIcon;
					_tempSubItem_Object.GetComponent<SubItem>().itemTitle = item.itemName;
					_tempSubItem_Object.GetComponent<SubItem>().itemDescription = item.simpleDescription;

					_tempSubItem_Object.GetComponent<Button>().onClick.AddListener(() => OnClickSubItem(_tempSubItem_Object.transform));

					Debug.Log($"{item.itemName}을 SubItem에 등록했습니다.");
				}
			}
		}

		for (int i = 0; i < gunDatabases.Length; i++)
		{
			foreach (Gun gun in gunDatabases[i].guns)
			{
				if (gun.isUnlocking)
				{
					GameObject _tempSubItem_Object = Instantiate(inventorySubItem_Object, inventorySubItem_Group.transform);

					_tempSubItem_Object.GetComponentsInChildren<Image>()[1].sprite = gun.gunIcon;
					_tempSubItem_Object.GetComponent<SubItem>().itemType = ItemType.Weapon;
					_tempSubItem_Object.GetComponent<SubItem>().gunData = gun;
					_tempSubItem_Object.GetComponent<SubItem>().itemSprite = gun.gunIcon;
					_tempSubItem_Object.GetComponent<SubItem>().itemTitle = gun.gunName;
					_tempSubItem_Object.GetComponent<SubItem>().itemDescription = gun.simpleDescription;

					_tempSubItem_Object.GetComponent<Button>().onClick.AddListener(() => OnClickSubItem(_tempSubItem_Object.transform));

					Debug.Log($"{gun.gunName}을 SubItem에 등록했습니다.");
				}
			}
		}

		#endregion
	}

    private void SetupButtonAddListener()
    {
        sorting_Button.onClick.AddListener(OnClickSortingInventory);
    }

	public void onClickChangeCategorySideBar(int _tempCategoryNumber)
    {
        ChangeInventoryCategory(_tempCategoryNumber);
    }

    public void ChangeInventoryCategory(int _categoryNumber)
    {
        if (currentInventoryCategory == _categoryNumber)
        {
            Debug.Log("이미 같은 카테고리입니다.");
            return;
        }

        currentInventoryCategory = _categoryNumber;
		Debug.Log($"현재 변경 될 카테고리 : {_categoryNumber}번 - 변경완료");

		foreach (Transform childTransform in inventorySubItem_Group.transform)
		{
			Destroy(childTransform.gameObject);
		}
		Debug.Log("남아있던 SubItem 오브젝트를 제거했습니다.");

		switch (currentInventoryCategory)
        {
            case 0:
				for (int i = 0; i < skillDatabases.Length; i++)
				{

					foreach (Item item in skillDatabases[i].items)
					{
						if (item.isUnlocking)
						{
							GameObject _tempSubItem_Object = Instantiate(inventorySubItem_Object, inventorySubItem_Group.transform);

							_tempSubItem_Object.GetComponentsInChildren<Image>()[1].sprite = item.SkillIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemType = ItemType.Skill;
							_tempSubItem_Object.GetComponent<SubItem>().itemData = item;
							_tempSubItem_Object.GetComponent<SubItem>().itemSprite = item.SkillIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemTitle = item.itemName;
							_tempSubItem_Object.GetComponent<SubItem>().itemDescription = item.simpleDescription;

							_tempSubItem_Object.GetComponent<Button>().onClick.AddListener(() => OnClickSubItem(_tempSubItem_Object.transform));

							Debug.Log($"{item.itemName}을 SubItem에 등록했습니다.");
						}
					}
				}

				for (int i = 0; i < gunDatabases.Length; i++)
				{
					foreach (Gun gun in gunDatabases[i].guns)
					{
						if (gun.isUnlocking)
						{
							GameObject _tempSubItem_Object = Instantiate(inventorySubItem_Object, inventorySubItem_Group.transform);

							_tempSubItem_Object.GetComponentsInChildren<Image>()[1].sprite = gun.gunIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemType = ItemType.Weapon;
							_tempSubItem_Object.GetComponent<SubItem>().gunData = gun;
							_tempSubItem_Object.GetComponent<SubItem>().itemSprite = gun.gunIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemTitle = gun.gunName;
							_tempSubItem_Object.GetComponent<SubItem>().itemDescription = gun.simpleDescription;

							_tempSubItem_Object.GetComponent<Button>().onClick.AddListener(() => OnClickSubItem(_tempSubItem_Object.transform));

							Debug.Log($"{gun.gunName}을 SubItem에 등록했습니다.");
						}
					}
				}
				break;  // 모두 선택 (스킬과 무기 모두 검색함)
			case 1:
				for (int i = 0; i < skillDatabases.Length; i++)
				{
					foreach (Item item in skillDatabases[i].items)
					{
						if (item.isUnlocking)
						{
							GameObject _tempSubItem_Object = Instantiate(inventorySubItem_Object, inventorySubItem_Group.transform);

							_tempSubItem_Object.GetComponentsInChildren<Image>()[1].sprite = item.SkillIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemType = ItemType.Skill;
							_tempSubItem_Object.GetComponent<SubItem>().itemData = item;
							_tempSubItem_Object.GetComponent<SubItem>().itemSprite = item.SkillIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemTitle = item.itemName;
							_tempSubItem_Object.GetComponent<SubItem>().itemDescription = item.simpleDescription;

							_tempSubItem_Object.GetComponent<Button>().onClick.AddListener(() => OnClickSubItem(_tempSubItem_Object.transform));

							Debug.Log($"{item.itemName}을 SubItem에 등록했습니다.");
						}
					}
				}
				break;  // 스킬 선택 (스킬 검색함)
			case 2:
				for (int i = 0; i < gunDatabases.Length; i++)
				{
					foreach (Gun gun in gunDatabases[i].guns)
					{
						if (gun.isUnlocking)
						{
							GameObject _tempSubItem_Object = Instantiate(inventorySubItem_Object, inventorySubItem_Group.transform);

							_tempSubItem_Object.GetComponentsInChildren<Image>()[1].sprite = gun.gunIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemType = ItemType.Weapon;
							_tempSubItem_Object.GetComponent<SubItem>().gunData = gun;
							_tempSubItem_Object.GetComponent<SubItem>().itemSprite = gun.gunIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemTitle = gun.gunName;
							_tempSubItem_Object.GetComponent<SubItem>().itemDescription = gun.simpleDescription;

							_tempSubItem_Object.GetComponent<Button>().onClick.AddListener(() => OnClickSubItem(_tempSubItem_Object.transform));

							Debug.Log($"{gun.gunName}을 SubItem에 등록했습니다.");
						}
					}
				}
				break;  // 무기 선택 (무기 검색함)
		}
	}

    private void OnClickSortingInventory()
    {
		if (currentInventoryCategory != 0)
		{
			Debug.Log($"{currentInventoryCategory}가 0이 아니기 때문에 정렬을 할 수 없습니다.");
			return;
		}

        isSkillFirst = !isSkillFirst;
        Debug.Log($"현재 정렬상태 스킬 우선 : {isSkillFirst}");

		foreach (Transform childTransform in inventorySubItem_Group.transform)
		{
			Destroy(childTransform.gameObject);
		}
		Debug.Log("남아있던 SubItem 오브젝트를 제거했습니다.");

		switch (isSkillFirst)
		{
			case true:
				for (int i = 0; i < skillDatabases.Length; i++)
				{
					foreach (Item item in skillDatabases[i].items)
					{
						if (item.isUnlocking)
						{
							GameObject _tempSubItem_Object = Instantiate(inventorySubItem_Object, inventorySubItem_Group.transform);

							_tempSubItem_Object.GetComponentsInChildren<Image>()[1].sprite = item.SkillIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemType = ItemType.Skill;
							_tempSubItem_Object.GetComponent<SubItem>().itemData = item;
							_tempSubItem_Object.GetComponent<SubItem>().itemSprite = item.SkillIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemTitle = item.itemName;
							_tempSubItem_Object.GetComponent<SubItem>().itemDescription = item.simpleDescription;

							_tempSubItem_Object.GetComponent<Button>().onClick.AddListener(() => OnClickSubItem(_tempSubItem_Object.transform));

							Debug.Log($"{item.itemName}을 SubItem에 등록했습니다.");
						}
					}
				}
				for (int i = 0; i < gunDatabases.Length; i++)
				{
					foreach (Gun gun in gunDatabases[i].guns)
					{
						if (gun.isUnlocking)
						{
							GameObject _tempSubItem_Object = Instantiate(inventorySubItem_Object, inventorySubItem_Group.transform);

							_tempSubItem_Object.GetComponentsInChildren<Image>()[1].sprite = gun.gunIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemType = ItemType.Weapon;
							_tempSubItem_Object.GetComponent<SubItem>().gunData = gun;
							_tempSubItem_Object.GetComponent<SubItem>().itemSprite = gun.gunIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemTitle = gun.gunName;
							_tempSubItem_Object.GetComponent<SubItem>().itemDescription = gun.simpleDescription;

							_tempSubItem_Object.GetComponent<Button>().onClick.AddListener(() => OnClickSubItem(_tempSubItem_Object.transform));

							Debug.Log($"{gun.gunName}을 SubItem에 등록했습니다.");
						}
					}
				}
				break;
			case false:
				for (int i = 0; i < gunDatabases.Length; i++)
				{
					foreach (Gun gun in gunDatabases[i].guns)
					{
						if (gun.isUnlocking)
						{
							GameObject _tempSubItem_Object = Instantiate(inventorySubItem_Object, inventorySubItem_Group.transform);

							_tempSubItem_Object.GetComponentsInChildren<Image>()[1].sprite = gun.gunIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemType = ItemType.Weapon;
							_tempSubItem_Object.GetComponent<SubItem>().gunData = gun;
							_tempSubItem_Object.GetComponent<SubItem>().itemSprite = gun.gunIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemTitle = gun.gunName;
							_tempSubItem_Object.GetComponent<SubItem>().itemDescription = gun.simpleDescription;

							_tempSubItem_Object.GetComponent<Button>().onClick.AddListener(() => OnClickSubItem(_tempSubItem_Object.transform));

							Debug.Log($"{gun.gunName}을 SubItem에 등록했습니다.");
						}
					}
				}
				for (int i = 0; i < skillDatabases.Length; i++)
				{
					foreach (Item item in skillDatabases[i].items)
					{
						if (item.isUnlocking)
						{
							GameObject _tempSubItem_Object = Instantiate(inventorySubItem_Object, inventorySubItem_Group.transform);

							_tempSubItem_Object.GetComponentsInChildren<Image>()[1].sprite = item.SkillIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemType = ItemType.Skill;
							_tempSubItem_Object.GetComponent<SubItem>().itemData = item;
							_tempSubItem_Object.GetComponent<SubItem>().itemSprite = item.SkillIcon;
							_tempSubItem_Object.GetComponent<SubItem>().itemTitle = item.itemName;
							_tempSubItem_Object.GetComponent<SubItem>().itemDescription = item.simpleDescription;

							_tempSubItem_Object.GetComponent<Button>().onClick.AddListener(() => OnClickSubItem(_tempSubItem_Object.transform));

							Debug.Log($"{item.itemName}을 SubItem에 등록했습니다.");
						}
					}
				}
				break;
		}
	}

	private int CurrentCategoryEverySubItemCount()
	{
		int _tempInt = 0;

		switch (currentInventoryCategory)
		{
			case 0:
				for (int i = 0; i < skillDatabases.Length; i++)
				{
					foreach (Item item in skillDatabases[i].items)
					{
						if (item.isUnlocking)
						{
							_tempInt++;
						}
					}
				}

				for (int i = 0; i < gunDatabases.Length; i++)
				{
					foreach (Gun gun in gunDatabases[i].guns)
					{
						if (gun.isUnlocking)
						{
							_tempInt++;
						}
					}
				}
				break;
			case 1:
				for (int i = 0; i < skillDatabases.Length; i++)
				{
					foreach (Item item in skillDatabases[i].items)
					{
						if (item.isUnlocking)
						{
							_tempInt++;
						}
					}
				}
				break;
			case 2:
				for (int i = 0; i < gunDatabases.Length; i++)
				{
					foreach (Gun gun in gunDatabases[i].guns)
					{
						if (gun.isUnlocking)
						{
							_tempInt++;
						}
					}
				}
				break;
		}

		return _tempInt;
	}	// 현재 검색 중인 카테고리의 모든 아이템 개수를 추적

	private void InventoryCount()
	{
		if (CurrentCategoryEverySubItemCount() > currentInventoryMaximumCount)
		{
			inventoryCount_Text.text = $"<color=red>공간부족</color>";
		}
		else if(CurrentCategoryEverySubItemCount() <= currentInventoryMaximumCount)
		{
			inventoryCount_Text.text = $"<color=white>{CurrentCategoryEverySubItemCount().ToString("D2")} / {currentInventoryMaximumCount.ToString("D2")}</color>";
		}

		if (inventorySubItem_Group.transform.childCount > currentInventoryMaximumCount)
		{
			Destroy(inventorySubItem_Group.transform.GetChild(0).gameObject);
			Debug.LogWarning("인벤토리가 꽉 차 가장 오래 전 갱신 된 인벤토리 아이템이 숨겨졌습니다.");
		}
	}

	private void InventoryExpansion()
	{
		switch (PlayerWalletManager.Instance.InventoryExpansionLevel)
		{
			case 1:
				for (int i = 0; i < inventorySubItemExpansion_Button_Group.Length; i++)
				{
					if (i == 0)
					{
						inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().alpha = 1;
						inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().interactable = true;
						continue;
					}
					Debug.Log(99);
					inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().alpha = 0.3f;
					inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().interactable = false;

					currentInventoryMaximumCount = 25;
				}
				break;
			case 2:
				for (int i = 0; i < inventorySubItemExpansion_Button_Group.Length; i++)
				{
					if (i == 1)
					{
						inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().alpha = 1;
						inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().interactable = true;
						return;
					}

					inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().alpha = 0.3f;
					inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().interactable = false;

					if (i < 1)
					{
						inventorySubItemExpansion_Button_Group[i].SetActive(false);
					}

					currentInventoryMaximumCount = 40;
				}
				break;
			case 3:
				for (int i = 0; i < inventorySubItemExpansion_Button_Group.Length; i++)
				{
					if (i == 2)
					{
						inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().alpha = 1;
						inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().interactable = true;
						return;
					}

					inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().alpha = 0.3f;
					inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().interactable = false;

					if (i < 2)
					{
						inventorySubItemExpansion_Button_Group[i].SetActive(false);
					}

					currentInventoryMaximumCount = 50;
				}
				break;
			case 4:
				for (int i = 0; i < inventorySubItemExpansion_Button_Group.Length; i++)
				{
					if (i == 3)
					{
						inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().alpha = 1;
						inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().interactable = true;
						return;
					}

					inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().alpha = 0.3f;
					inventorySubItemExpansion_Button_Group[i].GetComponent<CanvasGroup>().interactable = false;

					if (i < 3)
					{
						inventorySubItemExpansion_Button_Group[i].SetActive(false);
					}
				}
				break;
			default:
				Debug.LogError("현재 인벤토리 확장 레벨이 이상합니다.");
				PlayerWalletManager.Instance.InventoryExpansionLevel = 1;
				break;
		}
	}

	public void OnClickInventoryExpansionButton(int _tempExpansionPrice)
	{
		int _tempInventoryMaximumCount = 0;

		if (PlayerWalletManager.Instance.TotalCoin >= _tempExpansionPrice)
		{
			PlayerWalletManager.Instance.TotalCoin-= _tempExpansionPrice;
			Debug.Log("확장 결제 완료!");

			switch (PlayerWalletManager.Instance.InventoryExpansionLevel)
			{
				case 1:
					_tempInventoryMaximumCount = 25;
					break;
				case 2:
					_tempInventoryMaximumCount = 40;
					break;
				case 3:
					_tempInventoryMaximumCount = 50;
					break;
			}

			PlayerWalletManager.Instance.InventoryExpansionLevel++;
			currentInventoryMaximumCount = _tempInventoryMaximumCount;

			InventoryExpansion();
		}
	}

	public void OnClickSubItem(Transform tf)
	{
		topInventory_BG_Object.SetActive(true);
		inventoryQuit_Button.transform.localPosition = new Vector3(531.4f, 818f, 0f);
		target_SubItem = tf.GetComponent<SubItem>();
		target_Item_Title_Text.text = tf.GetComponent<SubItem>().itemTitle;
		target_ITem_SubTitle_Text.text = tf.GetComponent<SubItem>().itemDescription;
		target_Item_Image.sprite = tf.GetComponent<SubItem>().itemSprite;

		switch (tf.GetComponent<SubItem>().itemType)
		{
			case ItemType.Skill:
				break;
			case ItemType.Weapon:
				break;
		}
	}

	public void OnClickEquipItem()
	{
		equipManagement_Object.SetActive(true);

		switch (target_SubItem.itemType)
		{
			case ItemType.Skill:
				skillEquip_Manager.SetActive(true);
				weaponEquip_Manager.SetActive(false);
				break;
			case ItemType.Weapon:
				weaponEquip_Manager.SetActive(true);
				skillEquip_Manager.SetActive(false);
				break;
		}

	}

	public void OnClickThrowItem()
	{
		switch (target_SubItem.itemType)
		{
			case ItemType.Skill:
				target_SubItem.itemData.isUnlocking = false;
				Debug.Log("스킬 버리기가 성공했습니다.");
				topInventory_BG_Object.SetActive(false);
				break;
			case ItemType.Weapon:
				target_SubItem.gunData.isUnlocking = false;
				Debug.Log("스킬 버리기가 성공했습니다.");
				topInventory_BG_Object.SetActive(false);
				break;
		}

		inventoryQuit_Button.transform.localPosition = new Vector3(531.4f, 325f, 0f);
	}

	private void OnDisable()
	{
		SetupButtonRemoveListener();
	}

	private void SetupButtonRemoveListener()
	{
		sorting_Button.onClick.RemoveAllListeners();
	}
}
