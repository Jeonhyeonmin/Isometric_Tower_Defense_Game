using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillEncyclopedia : SingletonManager<SkillEncyclopedia>
{
	#region 변수

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
		Debug.Log($"아이템 데이터 베이스 {itemDatabases.Length}개가 인식 되었습니다.");
	}   // 아이템 데이터 베이스를 자동으로 검색해주는 매서드

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
			Debug.LogError("등록 된 데이터 베이스가 존재하지 않아 카테고리를 생성할 수 없습니다.");
		}

		if (categoryContents.childCount > 0)
		{
			Debug.Log("카테고리가 이미 생성되어 있어 파괴 후 재생성 합니다.");

			foreach (Transform categoryContent in categoryContents)
			{
				Destroy(categoryContent);
			}
		}

		foreach (ItemDatabase itemDatabase in itemDatabases)
		{
			GameObject categoryObject = Instantiate(categoryPrefab, categoryContents);

			Debug.Log("CategoryObject를 생성했습니다.");
			TMP_Text categoryTitleText = FindChildRecursive(categoryObject.transform, "Category_TitleText").GetComponent<TMP_Text>();
			categoryTitleText.text = itemDatabase.databaseName;

			categoryObject.GetComponent<Button>().onClick.AddListener(() => OnClickCategory(itemDatabase.databaseName));
			listenerButtonList.Add(categoryObject.GetComponent<Button>());

			categoryObject.GetComponent<Image>().SetNativeSize();
		}
	}	// 데이터 베이스를 순회하며 카테고리 UI 오브젝트를 생성한 후에 카테고리의 이름을 데이터 베이스의 이름으로 변경한다.

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
	}	// 자식 객체 안의 자식 객체를 찾기 위한 매서드

    public void OnClickCategory(string categoryName)
	{
		foreach (ItemDatabase itemDatabase in itemDatabases)
		{
			if (itemDatabase.databaseName == categoryName)
			{
				Debug.Log($"{itemDatabase.databaseName}와 {categoryName}를 비교한 결과 True입니다. \n선택 된 아이템 데이터 베이스를 {itemDatabase}로 변환합니다.");
				seletedItemDatabase = itemDatabase;
				UpdateInterfaceItemList();
				break;
			}

			Debug.LogError($"현재 카테고리 이름 {categoryName}과 같은 데이터 베이스가 없습니다.");
		}
	}	// 카테고리의 AddListener용 매서드, 카테고리 지정 이름과 데이터 베이스 이름이 같으면 선택 된 데이터 베이스로 지정한다.

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
                    Debug.Log($"잠금 상태가 {items[i].isUnlocking} 이므로 LockPanel이 보이지 않습니다.");
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
				Debug.Log("알맞은 인덱스 검색을 성공했습니다.");

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
