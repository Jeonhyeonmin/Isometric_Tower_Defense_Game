using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgrade : MonoBehaviour
{
    #region ��ų ����

    [SerializeField] private ItemDatabase[] itemDatabases;

    #region ��ȭ ���̽� ����

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
        //Debug.Log($"������ ������ ���̽� {itemDatabases.Length}���� �ν� �Ǿ����ϴ�.");

        if (SkillEncyclopedia.Instance == null)
            return;

        itemDatabases = SkillEncyclopedia.Instance.ItemDatabases;
    }   // ������ ������ ���̽��� �ڵ����� �˻����ִ� �ż���

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
    }	// �ڽ� ��ü ���� �ڽ� ��ü�� ã�� ���� �ż���

    private void Category_TitleText()
    {
        if (itemDatabases == null)
        {
            Debug.LogError("��� �� ������ ���̽��� �������� �ʾ� ī�װ��� ������ �� �����ϴ�.");
        }

        if (skillUpgradeContents.childCount > 0)
        {
            Debug.Log("ī�װ��� �̹� �����Ǿ� �־� �ı� �� ����� �մϴ�.");

            foreach (Transform categoryContent in skillUpgradeContents)
            {
                Destroy(categoryContent);
            }
        }

        foreach (ItemDatabase itemDatabase in itemDatabases)
        {
            GameObject categoryObject = Instantiate(skillUpgradePrefab, skillUpgradeContents);

            Debug.Log("CategoryObject�� �����߽��ϴ�.");
            TMP_Text categoryTitleText = FindChildRecursive(categoryObject.transform, "Category_TitleText").GetComponent<TMP_Text>();
            categoryTitleText.text = itemDatabase.databaseName;

            categoryObject.GetComponent<Button>().onClick.AddListener(() => OnClickCategory(itemDatabase.databaseName));
            listenerButtonList.Add(categoryObject.GetComponent<Button>());
        }
    }	// ������ ���̽��� ��ȸ�ϸ� ī�װ� UI ������Ʈ�� ������ �Ŀ� ī�װ��� �̸��� ������ ���̽��� �̸����� �����Ѵ�.

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

            Debug.LogError($"���� ī�װ� �̸� {categoryName}�� ���� ������ ���̽��� �����ϴ�. {itemDatabase.databaseName}");
        }
    }   // ī�װ��� AddListener�� �ż���, ī�װ� ���� �̸��� ������ ���̽� �̸��� ������ ���� �� ������ ���̽��� �����Ѵ�.

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

			// itemIndex�� �׻� ����
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
                Debug.Log("�˸��� �ε��� �˻��� �����߽��ϴ�.");

                targetItem_Title.text = items[i].itemName;
                targetItem_Image.sprite = items[i].SkillIcon;
                targetItem_Image.gameObject.SetActive(true);

                targetItem_Image.SetNativeSize();
            }
        }
    }
}
