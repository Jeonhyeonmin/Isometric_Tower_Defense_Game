using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillEncyclopedia : SingletonManager<SkillEncyclopedia>
{
	#region ����

	private ItemDatabase[] itemDatabases;

	public ItemDatabase[] ItemDatabases
	{
		get => itemDatabases;
		set => itemDatabases = value;
	}

    [SerializeField] private GameObject encylopediaPanel;

	[SerializeField] private GameObject[] categoryPrefabObject_List;
	[SerializeField] private GameObject categoryPrefab;
	[SerializeField] private Transform categoryContents;

	[SerializeField] private ItemDatabase seletedItemDatabase;

	[SerializeField] private GameObject[] subItemList;

	[SerializeField] private List<Button> listenerButtonList;

	[SerializeField] private TMP_Text baseItemTitle;
	[SerializeField] private TMP_Text baseItem_SubTitle;
	[SerializeField] private Image baseItem_Image;

	#endregion

	private void Awake()
	{
		FindItemDatabase();
	}

	private void FindItemDatabase()
	{
		itemDatabases = Resources.LoadAll<ItemDatabase>("Item/ItemDataBase");
		Debug.Log($"������ ������ ���̽� {itemDatabases.Length}���� �ν� �Ǿ����ϴ�.");
	}   // ������ ������ ���̽��� �ڵ����� �˻����ִ� �ż���

	private void OnEnable()
	{
        Transform gameObject = GameObject.Find("Skill_Encyclopedia").transform;
        Transform[] gameObjects = gameObject.GetComponentsInChildren<Transform>(true);
        encylopediaPanel = gameObjects[1].gameObject;

        StopCoroutine(ActiveObjectEncyclopedia());
		StartCoroutine(ActiveObjectEncyclopedia());
	}

	private IEnumerator ActiveObjectEncyclopedia()
	{
        while (!encylopediaPanel.activeSelf)
        {
			yield return null;
        }

        FindBaseItemObject();
        Category_TitleText();
        UpdateInterfaceItemList();
    }

    private void FindBaseItemObject()
    {
        baseItemTitle = FindChildRecursive(encylopediaPanel.transform, "BaseItem_TItle").GetComponent<TMP_Text>();
        baseItem_SubTitle = FindChildRecursive(encylopediaPanel.transform, "BaseItem_SubTItle").GetComponent<TMP_Text>();
        baseItem_Image = FindChildRecursive(encylopediaPanel.transform, "BaseItem_Image").GetComponent<Image>();
		categoryContents = FindChildRecursive(encylopediaPanel.transform, "Category_Contents").transform;
		Debug.Log(FindChildRecursive(encylopediaPanel.transform, "Category_Contents").transform);
    }

    private void Category_TitleText()
	{
		if (itemDatabases == null)
		{
			Debug.LogError("��� �� ������ ���̽��� �������� �ʾ� ī�װ��� ������ �� �����ϴ�.");
		}

		if (categoryContents.childCount > 0)
		{
			Debug.Log("ī�װ��� �̹� �����Ǿ� �־� �ı� �� ����� �մϴ�.");

			foreach (Transform categoryContent in categoryContents)
			{
				Destroy(categoryContent);
			}
		}

		foreach (ItemDatabase itemDatabase in itemDatabases)
		{
			GameObject categoryObject = Instantiate(categoryPrefab, categoryContents);

			Debug.Log("CategoryObject�� �����߽��ϴ�.");
			TMP_Text categoryTitleText = FindChildRecursive(categoryObject.transform, "Category_TitleText").GetComponent<TMP_Text>();
			categoryTitleText.text = itemDatabase.databaseName;

			categoryObject.GetComponent<Button>().onClick.AddListener(() => OnClickCategory(itemDatabase.databaseName));
			listenerButtonList.Add(categoryObject.GetComponent<Button>());

			categoryObject.GetComponent<Image>().SetNativeSize();
		}
	}	// ������ ���̽��� ��ȸ�ϸ� ī�װ� UI ������Ʈ�� ������ �Ŀ� ī�װ��� �̸��� ������ ���̽��� �̸����� �����Ѵ�.

	private Transform FindChildRecursive(Transform parent, string childName)
	{
		foreach (Transform childObject in parent)
		{
			if (childObject.name == childName)
			{
				return childObject;
			}

			Transform foundChildObject = FindChildRecursive(childObject, childName);

            if (foundChildObject != null)
            {
				return foundChildObject;
            }
        }

		return null;
	}	// �ڽ� ��ü ���� �ڽ� ��ü�� ã�� ���� �ż���

    public void OnClickCategory(string categoryName)
	{
		foreach (ItemDatabase itemDatabase in itemDatabases)
		{
			if (itemDatabase.databaseName == categoryName)
			{
				Debug.Log($"{itemDatabase.databaseName}�� {categoryName}�� ���� ��� True�Դϴ�. \n���� �� ������ ������ ���̽��� {itemDatabase}�� ��ȯ�մϴ�.");
				seletedItemDatabase = itemDatabase;
				UpdateInterfaceItemList();
				break;
			}

			Debug.LogError($"���� ī�װ� �̸� {categoryName}�� ���� ������ ���̽��� �����ϴ�.");
		}
	}	// ī�װ��� AddListener�� �ż���, ī�װ� ���� �̸��� ������ ���̽� �̸��� ������ ���� �� ������ ���̽��� �����Ѵ�.

	private void UpdateInterfaceItemList()
	{
        baseItemTitle.text = string.Empty;
        baseItem_SubTitle.text = string.Empty;
        baseItem_Image.sprite = null;
        baseItem_Image.gameObject.SetActive(false);

        if (seletedItemDatabase == null)
		{
			for (int i = 0; i < subItemList.Length; i++)
			{
				Image skillIconImage = FindChildRecursive(subItemList[i].transform, "Item_Image").GetComponent<Image>();
				skillIconImage.sprite = null;
				skillIconImage.color = new Color(255, 255, 255, 0);

				GameObject lockPanel = FindChildRecursive(subItemList[i].transform, "LockPanel").gameObject;
				lockPanel.SetActive(false);

                subItemList[i].GetComponent<TemporarySaveItem>().item = null;

				baseItemTitle.text = string.Empty;
                baseItem_SubTitle.text = string.Empty;
				baseItem_Image.sprite = null;
				baseItem_Image.gameObject.SetActive(false);
            }

			return;
		}

		if (!categoryContents.gameObject.activeInHierarchy)
			return;

		Item[] items = seletedItemDatabase.items;

		for (int i = 0; i < subItemList.Length; i++)
		{
			if (i < items.Length)
			{
				Image skillIconImage = FindChildRecursive(subItemList[i].transform, "Item_Image").GetComponent<Image>();
				skillIconImage.sprite = items[i].SkillIcon;
				skillIconImage.color = new Color(255, 255, 255, 255);

				if (!items[i].isUnlocking)
				{
                    GameObject lockPanel = FindChildRecursive(subItemList[i].transform, "LockPanel").gameObject;
					subItemList[i].GetComponent<TemporarySaveItem>().item = null;
                    lockPanel.SetActive(true);
                }
				else
				{
                    GameObject lockPanel = FindChildRecursive(subItemList[i].transform, "LockPanel").gameObject;
                    subItemList[i].GetComponent<TemporarySaveItem>().item = items[i];
                    lockPanel.SetActive(false);
                    Debug.Log($"��� ���°� {items[i].isUnlocking} �̹Ƿ� LockPanel�� ������ �ʽ��ϴ�.");
				}
			}
			else
			{
				Image skillIconImage = FindChildRecursive(subItemList[i].transform, "Item_Image").GetComponent<Image>();
				skillIconImage.sprite = null;
				skillIconImage.color = new Color(255, 255, 255, 0);

				GameObject lockPanel = FindChildRecursive(subItemList[i].transform, "LockPanel").gameObject;
				lockPanel.SetActive(false);

                subItemList[i].GetComponent<TemporarySaveItem>().item = null;
            }
		}
	}

	public void OnClickSubItem(int buttonIndex)
	{
		if (seletedItemDatabase == null)
			return;

        Item[] items = seletedItemDatabase.items;

		for (int i = 0; i < items.Length; i++)
		{
			if (i == buttonIndex)
			{
				Debug.Log("�˸��� �ε��� �˻��� �����߽��ϴ�.");

				baseItemTitle.text = items[i].itemName;
				baseItem_SubTitle.text = items[i].description;
				baseItem_Image.sprite = items[i].SkillIcon;
				baseItem_Image.gameObject.SetActive(true);

				baseItem_Image.SetNativeSize();
			}
		}
    }

    private void OnDisable()
    {
        foreach (Button button in listenerButtonList)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
