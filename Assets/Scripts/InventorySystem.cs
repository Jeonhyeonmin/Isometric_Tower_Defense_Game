using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventorySystem : MonoBehaviour
{
    // ===============
    [Header("�����ͺ��̽� ��� ---")]
    // �����ͺ��̽� ���
    // ===============

    [Header("��ų ������ ���̽� ���")]
    [SerializeField] private ItemDatabase[] skillDatabases;  // ��ų�� ������ ���̽���

    [Space(10)]

    [Header("���� ������ ���̽� ���")]
    [SerializeField] private GunDatabase[] gunDatabases;    // ������ ������ ���̽���

    // ===============
    [Space(30)]
    [Header("UI ��� ---")]
    // UI ���
    // ===============

    [Header("�κ��丮 ��� ���")]
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

    [Header("�κ��丮 �߾� ���")]
    [SerializeField] private GameObject inventorySubItem_Group;

    [SerializeField] private GameObject inventorySubItemExpansion_Group;
    [SerializeField] private GameObject inventorySubItem_Object;

	[SerializeField] private GameObject[] inventorySubItemExpansion_Button_Group;
	[Space(30)]

    [Header("�κ��丮 �ϴ� ���")]
    [SerializeField] private Button sorting_Button;
    [SerializeField] private bool isSkillFirst;

	[SerializeField] private TMP_Text inventoryCount_Text;
	[SerializeField] private int currentInventoryMaximumCount = 15;
    [Space(30)]

    [Header("�κ��丮 ���̵� ���")]
    [SerializeField] private Button sideAll_Button;
    [SerializeField] private Button sideSkill_Button;
    [SerializeField] private Button sideWeapon_Button;
    [Space(30)]

    [Header("�κ��丮 ���� ���")]
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
    [Header("�ý��� ��� ---")]
    // �ý��� ���
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
        #region ��ų ������ ���̽�

        skillDatabases = Resources.LoadAll<ItemDatabase>("Item/ItemDataBase");
        Debug.Log($"{skillDatabases.Length}���� ��ų ������ ���̽��� ã�ҽ��ϴ�.");

        #endregion

        #region ���� ������ ���̽�

        gunDatabases = Resources.LoadAll<GunDatabase>("Gun/GunDataBase");
        Debug.Log($"{gunDatabases.Length}���� ���� ������ ���̽��� ã�ҽ��ϴ�.");

        #endregion
    }

    private void SetupInterface()
    {
        #region �κ��丮 ��� UI

        target_Item_Image.sprite = null;
        target_Item_Title_Text.text = string.Empty;
        target_ITem_SubTitle_Text.text = string.Empty;
        currentInventoryCategory = 0;
		inventoryQuit_Button.transform.localPosition = new Vector3(531.4f, 325f, 0f);

		InventoryExpansion();

		topInventory_BG_Object.SetActive(false);

        #endregion

        #region �κ��丮 �߾�

        foreach (Transform childTransform in inventorySubItem_Group.transform)
        {
            Destroy(childTransform.gameObject);
        }
        Debug.Log("�����ִ� SubItem ������Ʈ�� �����߽��ϴ�.");

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

					Debug.Log($"{item.itemName}�� SubItem�� ����߽��ϴ�.");
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

					Debug.Log($"{gun.gunName}�� SubItem�� ����߽��ϴ�.");
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
            Debug.Log("�̹� ���� ī�װ��Դϴ�.");
            return;
        }

        currentInventoryCategory = _categoryNumber;
		Debug.Log($"���� ���� �� ī�װ� : {_categoryNumber}�� - ����Ϸ�");

		foreach (Transform childTransform in inventorySubItem_Group.transform)
		{
			Destroy(childTransform.gameObject);
		}
		Debug.Log("�����ִ� SubItem ������Ʈ�� �����߽��ϴ�.");

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

							Debug.Log($"{item.itemName}�� SubItem�� ����߽��ϴ�.");
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

							Debug.Log($"{gun.gunName}�� SubItem�� ����߽��ϴ�.");
						}
					}
				}
				break;  // ��� ���� (��ų�� ���� ��� �˻���)
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

							Debug.Log($"{item.itemName}�� SubItem�� ����߽��ϴ�.");
						}
					}
				}
				break;  // ��ų ���� (��ų �˻���)
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

							Debug.Log($"{gun.gunName}�� SubItem�� ����߽��ϴ�.");
						}
					}
				}
				break;  // ���� ���� (���� �˻���)
		}
	}

    private void OnClickSortingInventory()
    {
		if (currentInventoryCategory != 0)
		{
			Debug.Log($"{currentInventoryCategory}�� 0�� �ƴϱ� ������ ������ �� �� �����ϴ�.");
			return;
		}

        isSkillFirst = !isSkillFirst;
        Debug.Log($"���� ���Ļ��� ��ų �켱 : {isSkillFirst}");

		foreach (Transform childTransform in inventorySubItem_Group.transform)
		{
			Destroy(childTransform.gameObject);
		}
		Debug.Log("�����ִ� SubItem ������Ʈ�� �����߽��ϴ�.");

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

							Debug.Log($"{item.itemName}�� SubItem�� ����߽��ϴ�.");
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

							Debug.Log($"{gun.gunName}�� SubItem�� ����߽��ϴ�.");
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

							Debug.Log($"{gun.gunName}�� SubItem�� ����߽��ϴ�.");
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

							Debug.Log($"{item.itemName}�� SubItem�� ����߽��ϴ�.");
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
	}	// ���� �˻� ���� ī�װ��� ��� ������ ������ ����

	private void InventoryCount()
	{
		if (CurrentCategoryEverySubItemCount() > currentInventoryMaximumCount)
		{
			inventoryCount_Text.text = $"<color=red>��������</color>";
		}
		else if(CurrentCategoryEverySubItemCount() <= currentInventoryMaximumCount)
		{
			inventoryCount_Text.text = $"<color=white>{CurrentCategoryEverySubItemCount().ToString("D2")} / {currentInventoryMaximumCount.ToString("D2")}</color>";
		}

		if (inventorySubItem_Group.transform.childCount > currentInventoryMaximumCount)
		{
			Destroy(inventorySubItem_Group.transform.GetChild(0).gameObject);
			Debug.LogWarning("�κ��丮�� �� �� ���� ���� �� ���� �� �κ��丮 �������� ���������ϴ�.");
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
				Debug.LogError("���� �κ��丮 Ȯ�� ������ �̻��մϴ�.");
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
			Debug.Log("Ȯ�� ���� �Ϸ�!");

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
				Debug.Log("��ų �����Ⱑ �����߽��ϴ�.");
				topInventory_BG_Object.SetActive(false);
				break;
			case ItemType.Weapon:
				target_SubItem.gunData.isUnlocking = false;
				Debug.Log("��ų �����Ⱑ �����߽��ϴ�.");
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
