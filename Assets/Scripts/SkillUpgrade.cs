using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgrade : MonoBehaviour
{
    #region 스킬 변수

    [SerializeField] private ItemDatabase[] itemDatabases;

    #region 강화 베이스 변수

    [SerializeField] private TMP_Text targetItem_Title;
    [SerializeField] private Image targetItem_Image;

    #endregion

    [SerializeField] private GameObject[] skillUpgradePrefabObject_List;
    [SerializeField] private GameObject skillUpgradePrefab;
    [SerializeField] private Transform skillUpgradeContents;

    [SerializeField] private ItemDatabase seletedItemDatabase;
    [SerializeField] private Item seletedItem;

    [SerializeField] private GameObject skillUpgradePanel;

    [SerializeField] private GameObject[] subItemList;
    [SerializeField] private List<Button> listenerButtonList;

    #endregion

    private void Start()
    {
        FindItemDatabase();

        Transform[] gameObjects = gameObject.GetComponentsInChildren<Transform>(true);
        skillUpgradePanel = gameObjects[1].gameObject;

        StopCoroutine(ActiveObjectSkillUpgrade());
        StartCoroutine(ActiveObjectSkillUpgrade());
    }

    private void FindItemDatabase()
    {
        //itemDatabases = Resources.LoadAll<ItemDatabase>("Item/ItemDataBase");
        //Debug.Log($"아이템 데이터 베이스 {itemDatabases.Length}개가 인식 되었습니다.");

        if (SkillEncyclopedia.Instance == null)
            return;

        itemDatabases = SkillEncyclopedia.Instance.ItemDatabases;
    }   // 아이템 데이터 베이스를 자동으로 검색해주는 매서드

    private IEnumerator ActiveObjectSkillUpgrade()
    {
        while (!skillUpgradePanel.activeSelf)
        {
            yield return null;
        }

        FindBaseItemObject();
        Category_TitleText();
        UpdateInterfaceItemList();
    }

    private void FindBaseItemObject()
    {
        targetItem_Title = FindChildRecursive(skillUpgradePanel.transform, "TItle_Text").GetComponent<TMP_Text>();
        targetItem_Image = FindChildRecursive(skillUpgradePanel.transform, "Upgrade_Image").GetComponent<Image>();
    }

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

    private void Category_TitleText()
    {
        if (itemDatabases == null)
        {
            Debug.LogError("등록 된 데이터 베이스가 존재하지 않아 카테고리를 생성할 수 없습니다.");
        }

        if (skillUpgradeContents.childCount > 0)
        {
            Debug.Log("카테고리가 이미 생성되어 있어 파괴 후 재생성 합니다.");

            foreach (Transform categoryContent in skillUpgradeContents)
            {
                Destroy(categoryContent);
            }
        }

        foreach (ItemDatabase itemDatabase in itemDatabases)
        {
            GameObject categoryObject = Instantiate(skillUpgradePrefab, skillUpgradeContents);

            Debug.Log("CategoryObject를 생성했습니다.");
            TMP_Text categoryTitleText = FindChildRecursive(categoryObject.transform, "Category_TitleText").GetComponent<TMP_Text>();
            categoryTitleText.text = itemDatabase.databaseName;

            categoryObject.GetComponent<Button>().onClick.AddListener(() => OnClickCategory(itemDatabase.databaseName));
            listenerButtonList.Add(categoryObject.GetComponent<Button>());
        }
    }	// 데이터 베이스를 순회하며 카테고리 UI 오브젝트를 생성한 후에 카테고리의 이름을 데이터 베이스의 이름으로 변경한다.

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

            Debug.LogError($"현재 카테고리 이름 {categoryName}과 같은 데이터 베이스가 없습니다. {itemDatabase.databaseName}");
        }
    }   // 카테고리의 AddListener용 매서드, 카테고리 지정 이름과 데이터 베이스 이름이 같으면 선택 된 데이터 베이스로 지정한다.

	private void UpdateInterfaceItemList()
	{
		targetItem_Title.text = string.Empty;
		targetItem_Image.sprite = null;
		targetItem_Image.gameObject.SetActive(false);

		for (int i = 0; i < subItemList.Length; i++)
		{
			Image skillIconImage = FindChildRecursive(subItemList[i].transform, "Item_Image").GetComponent<Image>();
			skillIconImage.sprite = null;
			skillIconImage.color = new Color(255, 255, 255, 0);

			subItemList[i].GetComponent<TemporarySaveItem>().item = null;
		}

		if (seletedItemDatabase == null)
		{
			return;
		}

		if (!skillUpgradeContents.gameObject.activeInHierarchy)
			return;

		Item[] items = seletedItemDatabase.items;
		int itemIndex = 0;

		for (int i = 0; i < subItemList.Length; i++)
		{
			if (itemIndex >= items.Length)
				break;

			Image skillIconImage = FindChildRecursive(subItemList[i].transform, "Item_Image").GetComponent<Image>();

			if (items[itemIndex].isUnlocking)
			{
				skillIconImage.sprite = items[itemIndex].SkillIcon;
				skillIconImage.color = new Color(255, 255, 255, 255);

				subItemList[i].GetComponent<TemporarySaveItem>().item = items[itemIndex];
			}
			else
			{
				skillIconImage.sprite = null;
				skillIconImage.color = new Color(255, 255, 255, 0);
				i--;
			}

			// itemIndex는 항상 증가
			itemIndex++;
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

                targetItem_Title.text = items[i].itemName;
                targetItem_Image.sprite = items[i].SkillIcon;
                targetItem_Image.gameObject.SetActive(true);

                targetItem_Image.SetNativeSize();
            }
        }
    }
}
