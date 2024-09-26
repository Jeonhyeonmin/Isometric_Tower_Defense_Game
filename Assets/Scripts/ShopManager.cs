using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour
{
	#region 상점 변수

	[SerializeField] private ItemDatabase freeitemDatabase;
	[SerializeField] private ItemDatabase buyitemDatabase;
	[SerializeField] private List<Item> availablefreeItems = new List<Item>();
	[SerializeField] private List<Item> availablebuyItems = new List<Item>();
	[SerializeField] private int freeItemsToDisplay = 2;
    [SerializeField] private int buyitemsToDisplay = 4;
    [SerializeField] private GameObject itemUIPrefab;
    [SerializeField] private Transform itemFreeUIParent;
    [SerializeField] private Transform itemBuyUIParent;
    [SerializeField] private GameObject shopPanel;

	[SerializeField] private string firstTimeOpenShopKey = "FirstTimeOpenShopKey";

	[SerializeField] private Sprite coinSprite;
	[SerializeField] private Sprite crystalSprite;

	private const string FreeItemKey = "AvailableFreeItems";
	private const string BuyItemKey = "AvailableBuyItems";

	private DateTime endTime;

	#endregion

	private void Start()
	{ 
		SettingsItemDisplayCount(); // 아이템 데이터베이스의 아이템의 개수가 "itemsToDisplay"보다 적으면 자동으로 "itemsToDisplay"의 수를 조절합니다.
		NotifyItemCount();  // 개발자에게 데이터 베이스의 존재 유무, 활성화 된 아이템의 개수를 알려줍니다.	
	}

	public void LoadItemData()
	{
		string endTimeString = PlayerPrefs.GetString("ShopEndTime", null);

		if (!string.IsNullOrEmpty(endTimeString))
		{
			endTime = DateTime.Parse(endTimeString);
		}
		else
		{
			endTime = DateTime.Now;
			Debug.Log("'endTime'은 현재로 임시 초기화 되었습니다.");
		}

		TimeSpan remainingTime = endTime - DateTime.Now;

		if (remainingTime.TotalSeconds <= 0)
		{
			Debug.Log($"remainingTime : {remainingTime.TotalSeconds}이므로 이전 저장 된 상점 아이템이 초기화 되었습니다.");
			return;
		}
		else
		{
			Debug.Log($"{remainingTime.TotalSeconds.ToString("F2")}초가 남았으므로 상점 초기화가 이루어지지 않고 이전 상점 아이템이 로딩됩니다");
		}

		// availablefreeItems 불러오기
		if (PlayerPrefs.HasKey(FreeItemKey))
		{
			string freeItemsJson = PlayerPrefs.GetString(FreeItemKey);
			ItemListWrapper freeItemsWrapper = JsonUtility.FromJson<ItemListWrapper>(freeItemsJson);
			availablefreeItems = freeItemsWrapper.items;
		}

		// availablebuyItems 불러오기
		if (PlayerPrefs.HasKey(BuyItemKey))
		{
			string buyItemsJson = PlayerPrefs.GetString(BuyItemKey);
			ItemListWrapper buyItemsWrapper = JsonUtility.FromJson<ItemListWrapper>(buyItemsJson);
			availablebuyItems = buyItemsWrapper.items;
		}

		if (PlayerPrefs.HasKey(FreeItemKey) || PlayerPrefs.HasKey(BuyItemKey))
		{
			if (availablefreeItems.Count != 0 || availablebuyItems.Count != 0)
			{
				Debug.Log("아이템 UI 생성 시도 중 입니다.");
				InstantiateItemUI();
			}
			else
			{
				Debug.LogError($"availablefreeItems : {availablefreeItems.Count}개 존재하고 availablebuyItems : {availablebuyItems.Count}개 존재하므로 UI를 생성할 수 없습니다.");
			}
		}
	}	  // 이전 게임 종료 시 저장 되어 있던 상점 아이템을 불러와 UI 반영

	private void SettingsItemDisplayCount()
    {
		#region 무료 아이템

		if (freeitemDatabase != null)
		{
			if (freeitemDatabase.items.Length >= freeItemsToDisplay)
			{
				Debug.Log($"무료 아이템의 최종 등록 개수는 {freeItemsToDisplay}개 입니다.");
			}
			else
			{
				freeItemsToDisplay = freeitemDatabase.items.Length;
				Debug.LogError($"무료 아이템 데이터 베이스에 등록 된 아이템이 적어 'itemToDisplay' 변수가 자동으로 조절되었습니다. \n최종 등록 개수는 {freeItemsToDisplay}개 입니다.");
			}
		}

		#endregion

		#region 구매 아이템

		if (buyitemDatabase != null)
		{
			if (buyitemDatabase.items.Length >= buyitemsToDisplay)
			{
				Debug.Log($"구매 아이템의 최종 등록 개수는 {buyitemsToDisplay}개 입니다.");
			}
			else
			{
				buyitemsToDisplay = buyitemDatabase.items.Length;
				Debug.LogError($"구매 아이템 데이터 베이스에 등록 된 아이템이 적어 'itemToDisplay' 변수가 자동으로 조절되었습니다. \n최종 등록 개수는 {buyitemsToDisplay}개 입니다.");
			}
		}
		
		#endregion
	}   // 아이템 데이터 베이스에 등록되어 있는 아이템이 기본 DisplayCount 변수보다 작을 시, DisplayCount 변수를 조절함 

	private void NotifyItemCount()
	{
#if UNITY_EDITOR
		#region 무료 아이템

		if (freeitemDatabase != null)
		{
			Debug.Log($"{freeitemDatabase}가 존재하며, {freeitemDatabase.items.Length}개가 무료 아이템 데이터베이스에 존재합니다.");
		}
		else
		{
			Debug.LogError($"'freeitemDatabase'가 존재하지 않습니다.");
		}

		#endregion

		#region 구매 아이템

		if (buyitemDatabase != null)
		{
			Debug.Log($"{buyitemDatabase}가 존재하며, {buyitemDatabase.items.Length}개가 구매 아이템 데이터베이스에 존재합니다.");
		}
		else
		{
			Debug.LogError($"'buyitemDatabase'가 존재하지 않습니다.");
		}

		#endregion
#endif
	}	  // 아이템 데이터 베이스의 상제 정보를 게임 시작 초기에 알려줌

	public void FirstTimeOpenShopUI()
	{
		if (PlayerPrefs.HasKey(firstTimeOpenShopKey))
		{
			Debug.LogError("이미 상점을 방문했기 때문에 'FirstTimeOpenShopUI' 매서드는 작동하지 않습니다.");
			return;
		}

		PlayerPrefs.SetString(firstTimeOpenShopKey, "Visit");
		PlayerPrefs.Save();

		#region 무료 아이템 상점 설정

		if (freeitemDatabase != null)
		{
			List<Item> _ItemsCurrentSelectIndex = new List<Item>();

			int _ItemCurrentRandomIndex = 0;
			int _ItemCurrentIndex = 0;

			bool _ItemCurrentInserted;

			for (int i = 0; i < freeItemsToDisplay; i++)
			{
				GameObject temp_FreeItemObject = Instantiate(itemUIPrefab, itemFreeUIParent);

				TMP_Text temp_FreeItemObject_Title = FindChildRecursive(temp_FreeItemObject.transform, "Item_Title").GetComponent<TMP_Text>();
				Image temp_FreeItemObject_ItemImage = FindChildRecursive(temp_FreeItemObject.transform, "Item_Image").GetComponent<Image>();

				TMP_Text temp_FreeItemObject_Price = FindChildRecursive(temp_FreeItemObject.transform, "Buy_PriceText").GetComponent<TMP_Text>();
				Image temp_FreeItemObject_CurrencyType = FindChildRecursive(temp_FreeItemObject.transform, "Buy_Coin_Image").GetComponent<Image>();

				Button temp_FreeItemObject_Button = FindChildRecursive(temp_FreeItemObject.transform, "Buy_Button").GetComponent<Button>();

#if UNITY_EDITOR
				#region 개발자 디버그 코드

				if (temp_FreeItemObject_Title != null)
				{
					Debug.Log("'temp_FreeItemObject_Title'를 찾았습니다.");
				}
				else
				{
					Debug.Log("'temp_FreeItemObject_Title'를 찾을 수 없습니다.");
				}

				if (temp_FreeItemObject_ItemImage != null)
				{
					Debug.Log("'temp_FreeItemObject_ItemImage'를 찾았습니다.");
				}
				else
				{
					Debug.Log("'temp_FreeItemObject_ItemImage'를 찾을 수 없습니다.");
				}

				if (temp_FreeItemObject_Price != null)
				{
					Debug.Log("'temp_FreeItemObject_Price'를 찾았습니다.");
				}
				else
				{
					Debug.Log("'temp_FreeItemObject_Price'를 찾을 수 없습니다.");
				}

				#endregion
#endif

				if (freeitemDatabase.items.Length == freeItemsToDisplay)    // 무료 아이템 베이스의 등록되어 있는 아이템의 개수가 'freeItemsToDisplay'와 같다면
				{
					temp_FreeItemObject_Title.text = freeitemDatabase.items[_ItemCurrentIndex].itemName;
					temp_FreeItemObject_ItemImage.sprite = freeitemDatabase.items[_ItemCurrentIndex].SkillIcon;
					temp_FreeItemObject_ItemImage.SetNativeSize();

					temp_FreeItemObject_Price.text = freeitemDatabase.items[_ItemCurrentIndex].price.ToString("#,##0");

					temp_FreeItemObject_Button.onClick.AddListener(() => OnClickBuy(freeitemDatabase.items[_ItemCurrentIndex].itemName, temp_FreeItemObject.transform, freeitemDatabase.items[_ItemCurrentIndex], int.Parse(temp_FreeItemObject_Price.text)));

					switch (freeitemDatabase.items[_ItemCurrentIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_FreeItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_FreeItemObject_CurrencyType'의 화폐는 코인입니다.");
							break;
						case CurrencyType.Crystal:
							temp_FreeItemObject_CurrencyType.sprite = crystalSprite;
							temp_FreeItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_FreeItemObject_CurrencyType'의 화폐는 크리스탈입니다.");
							break;
					}

                    if (freeitemDatabase.items[_ItemCurrentIndex].isUnlocking)
                    {
                        GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
                        alreadyBuy_Panel.SetActive(true);
                    }

                    temp_FreeItemObject_CurrencyType.SetNativeSize();

					availablefreeItems.Add(freeitemDatabase.items[_ItemCurrentIndex]);

					_ItemCurrentIndex++;
				}
				else
				{
					while (availablefreeItems.Count < freeItemsToDisplay)	// 선택 될 아이템의 RandomIndex를 검증하고, 마지막으로 선택한다.
					{
						_ItemCurrentRandomIndex = Random.Range(0, freeitemDatabase.items.Length);
						_ItemCurrentInserted = false;
						
						if (_ItemsCurrentSelectIndex != null)
						{
							foreach (Item availablefreeItem in availablefreeItems)
							{
								if (availablefreeItem.itemName == freeitemDatabase.items[_ItemCurrentRandomIndex].itemName)
								{
									Debug.Log($"이미 삽입 되어 있는 같은 아이템이 있어 {freeitemDatabase.items[_ItemCurrentRandomIndex].itemName}은(는) 등록되지 않습니다.");
									_ItemCurrentInserted = true;
									break;
								}
							}
						}

						if (_ItemCurrentInserted)
						{
							Debug.Log($"아이템 삽입 감지 : {_ItemCurrentInserted}이므로 반복문을 넘겼습니다.");
							continue;
						}

						Debug.Log($"아이템 삽입 감지 : {_ItemCurrentInserted}이므로 반복문 넘기지 않았습니다.");

						availablefreeItems.Add(freeitemDatabase.items[_ItemCurrentRandomIndex]);
						break;
					}

					temp_FreeItemObject_Title.text = freeitemDatabase.items[_ItemCurrentRandomIndex].itemName;
					temp_FreeItemObject_ItemImage.sprite = freeitemDatabase.items[_ItemCurrentRandomIndex].SkillIcon;
					temp_FreeItemObject_ItemImage.SetNativeSize();

					temp_FreeItemObject_Price.text = freeitemDatabase.items[_ItemCurrentRandomIndex].price.ToString("#,##0");

					temp_FreeItemObject_Button.onClick.AddListener(() => OnClickBuy(freeitemDatabase.items[_ItemCurrentRandomIndex].itemName, temp_FreeItemObject.transform, freeitemDatabase.items[_ItemCurrentRandomIndex], int.Parse(temp_FreeItemObject_Price.text)));

					switch (freeitemDatabase.items[_ItemCurrentRandomIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_FreeItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_FreeItemObject_CurrencyType'의 화폐는 코인입니다.");
							break;
						case CurrencyType.Crystal:
							temp_FreeItemObject_CurrencyType.sprite = crystalSprite;
							temp_FreeItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_FreeItemObject_CurrencyType'의 화폐는 크리스탈입니다.");
							break;
					}

                    if (freeitemDatabase.items[_ItemCurrentRandomIndex].isUnlocking)
                    {
                        GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
                        alreadyBuy_Panel.SetActive(true);
                    }

                    temp_FreeItemObject_CurrencyType.SetNativeSize();

					_ItemsCurrentSelectIndex.Add(freeitemDatabase.items[_ItemCurrentRandomIndex]);
				}
			}
		}

		#endregion

		#region 구매 아이템 상점 설정

		if (buyitemDatabase != null)
		{
			List<Item> _ItemsCurrentSelectIndex = new List<Item>();

			int _ItemCurrentRandomIndex = 0;
			int _ItemCurrentIndex = 0;

			bool _ItemCurrentInserted;

			for (int i = 0; i < buyitemsToDisplay; i++)
			{
				GameObject temp_BuyItemObject = Instantiate(itemUIPrefab, itemBuyUIParent);

				TMP_Text temp_BuyItemObject_Title = FindChildRecursive(temp_BuyItemObject.transform, "Item_Title").GetComponent<TMP_Text>();
				Image temp_BuyItemObject_ItemImage = FindChildRecursive(temp_BuyItemObject.transform, "Item_Image").GetComponent<Image>();

				TMP_Text temp_BuyItemObject_Price = FindChildRecursive(temp_BuyItemObject.transform, "Buy_PriceText").GetComponent<TMP_Text>();
				Image temp_BuyItemObject_CurrencyType = FindChildRecursive(temp_BuyItemObject.transform, "Buy_Coin_Image").GetComponent<Image>();

				Button temp_FreeItemObject_Button = FindChildRecursive(temp_BuyItemObject.transform, "Buy_Button").GetComponent<Button>();

#if UNITY_EDITOR
				#region 개발자 디버그 코드

				if (temp_BuyItemObject_Title != null)
				{
					Debug.Log("'temp_BuyItemObject_Title'를 찾았습니다.");
				}
				else
				{
					Debug.Log("'temp_BuyItemObject_Title'를 찾을 수 없습니다.");
				}

				if (temp_BuyItemObject_ItemImage != null)
				{
					Debug.Log("'temp_BuyItemObject_ItemImage'를 찾았습니다.");
				}
				else
				{
					Debug.Log("'temp_BuyItemObject_ItemImage'를 찾을 수 없습니다.");
				}

				if (temp_BuyItemObject_Price != null)
				{
					Debug.Log("'temp_BuyItemObject_Price'를 찾았습니다.");
				}
				else
				{
					Debug.Log("'temp_BuyItemObject_Price'를 찾을 수 없습니다.");
				}

				#endregion
#endif

				if (buyitemDatabase.items.Length == buyitemsToDisplay)    // 구매 아이템 베이스에 등록되어 있는 아이템의 개수가 'buyitemsToDisplay'와 같다면
				{
					temp_BuyItemObject_Title.text = buyitemDatabase.items[_ItemCurrentIndex].itemName;
					temp_BuyItemObject_ItemImage.sprite = buyitemDatabase.items[_ItemCurrentIndex].SkillIcon;
					temp_BuyItemObject_ItemImage.SetNativeSize();

					temp_BuyItemObject_Price.text = buyitemDatabase.items[_ItemCurrentIndex].price.ToString("#,##0");

					temp_FreeItemObject_Button.onClick.AddListener(() => OnClickBuy(buyitemDatabase.items[_ItemCurrentIndex].itemName, temp_BuyItemObject.transform, buyitemDatabase.items[_ItemCurrentIndex], int.Parse(temp_BuyItemObject_Price.text)));

					switch (buyitemDatabase.items[_ItemCurrentIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_BuyItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_BuyItemObject_CurrencyType'의 화폐는 코인입니다.");
							break;
						case CurrencyType.Crystal:
							temp_BuyItemObject_CurrencyType.sprite = crystalSprite;
							temp_BuyItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_BuyItemObject_CurrencyType'의 화폐는 크리스탈입니다.");
							break;
					}

                    if (buyitemDatabase.items[_ItemCurrentIndex].isUnlocking)
                    {
                        GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
                        alreadyBuy_Panel.SetActive(true);
                    }

                    temp_BuyItemObject_CurrencyType.SetNativeSize();

					availablebuyItems.Add(buyitemDatabase.items[_ItemCurrentIndex]);

					_ItemCurrentIndex++;
				}
				else  // 구매 아이템 베이스에 등록되어 있는 아이템의 개수가 'buyItemsToDisplay'보다 크다면
				{
					while (availablebuyItems.Count < buyitemsToDisplay)	// 선택 될 아이템의 RandomIndex를 검증하고, 마지막으로 선택한다.
					{
						_ItemCurrentRandomIndex = Random.Range(0, buyitemDatabase.items.Length);
						_ItemCurrentInserted = false;
						
						if (_ItemsCurrentSelectIndex != null)
						{
							foreach (Item availablebuyItem in availablebuyItems)
							{
								if (availablebuyItem.itemName == buyitemDatabase.items[_ItemCurrentRandomIndex].itemName)
								{
									Debug.Log($"이미 삽입 되어 있는 같은 아이템이 있어 {buyitemDatabase.items[_ItemCurrentRandomIndex].itemName}은(는) 등록되지 않습니다.");
									_ItemCurrentInserted = true;
									break;
								}
							}
						}

						if (_ItemCurrentInserted)
						{
							Debug.Log($"아이템 삽입 감지 : {_ItemCurrentInserted}이므로 반복문을 넘겼습니다.");
							continue;
						}

						Debug.Log($"아이템 삽입 감지 : {_ItemCurrentInserted}이므로 반복문 넘기지 않았습니다.");

						availablebuyItems.Add(buyitemDatabase.items[_ItemCurrentRandomIndex]);
						break;
					}

					temp_BuyItemObject_Title.text = buyitemDatabase.items[_ItemCurrentRandomIndex].itemName;
					Debug.Log(temp_BuyItemObject_Title.text);
					temp_BuyItemObject_ItemImage.sprite = buyitemDatabase.items[_ItemCurrentRandomIndex].SkillIcon;
					temp_BuyItemObject_ItemImage.SetNativeSize();

					temp_BuyItemObject_Price.text = buyitemDatabase.items[_ItemCurrentRandomIndex].price.ToString("#,##0");

					temp_FreeItemObject_Button.onClick.AddListener(() => OnClickBuy(buyitemDatabase.items[_ItemCurrentRandomIndex].itemName, temp_BuyItemObject.transform, buyitemDatabase.items[_ItemCurrentRandomIndex], int.Parse(temp_BuyItemObject_Price.text)));

					switch (buyitemDatabase.items[_ItemCurrentRandomIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_BuyItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_BuyItemObject_CurrencyType'의 화폐는 코인입니다.");
							break;
						case CurrencyType.Crystal:
							temp_BuyItemObject_CurrencyType.sprite = crystalSprite;
							temp_BuyItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_BuyItemObject_CurrencyType'의 화폐는 크리스탈입니다.");
							break;
					}

                    if (buyitemDatabase.items[_ItemCurrentRandomIndex].isUnlocking)
                    {
                        GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
                        alreadyBuy_Panel.SetActive(true);
                    }

                    temp_BuyItemObject_CurrencyType.SetNativeSize();

					_ItemsCurrentSelectIndex.Add(buyitemDatabase.items[_ItemCurrentRandomIndex]);
				}
			}
		}

		#endregion

	}	 // 게임 설치 후 최초 상점 오픈 시 실행되는 코드

	public void InitializationItems()	// 남은 시간이 없을 때, 초기화 후 상점 아이템 랜덤 등록
	{
		if (CountdownTimer.Instance.remainingTime.TotalSeconds > 0)
		{
			Debug.Log($"초기화까지 {CountdownTimer.Instance.remainingTime.TotalSeconds.ToString("F2")}초가 남아있습니다.");
			return;
		}

		if (!PlayerPrefs.HasKey(firstTimeOpenShopKey))
		{
			Debug.LogError("상점을 처음 실행했습니다. 'InitializationItems' 매서드는 실행되지 않습니다.");
			return;
		}

		if (shopPanel.activeInHierarchy)
		{
			Debug.LogError($"{CountdownTimer.Instance.remainingTime.TotalSeconds.ToString("F0")}초 이지만 상점이 열리지 않아 초기화가 미뤄집니다.");
			return;
		}

		CountdownTimer.Instance.endTime = DateTime.Now.AddHours(24);
		PlayerPrefs.SetString("ShopEndTime", CountdownTimer.Instance.endTime.ToString());
		PlayerPrefs.Save();
		Debug.Log($"'CountdownTimer'의 endTime이 새로고침 되었습니다.");

		#region 무료 아이템

		foreach (Transform childObjectUI in itemFreeUIParent)	// 혹여나 이전에 등록 된 아이템 UI가 존재한다면 순회하며 삭제한다.
		{
			TMP_Text itemTitle = FindChildRecursive(childObjectUI.transform, "Item_Title").GetComponent<TMP_Text>();

			if (itemTitle != null)
			{
				Item itemToRemove = availablefreeItems.FirstOrDefault(item => item.itemName == itemTitle.text);

				if (itemToRemove != null)
				{
					availablefreeItems.Remove(itemToRemove);
				}
			}

			Destroy(childObjectUI);
		}

		#region 무료 아이템 상점 설정

		if (freeitemDatabase != null)
		{
			List<Item> _ItemsCurrentSelectIndex = new List<Item>();

			int _ItemCurrentRandomIndex = 0;
			int _ItemCurrentIndex = 0;

			bool _ItemCurrentInserted;

			for (int i = 0; i < freeItemsToDisplay; i++)
			{
				GameObject temp_FreeItemObject = Instantiate(itemUIPrefab, itemFreeUIParent);

				TMP_Text temp_FreeItemObject_Title = FindChildRecursive(temp_FreeItemObject.transform, "Item_Title").GetComponent<TMP_Text>();
				Image temp_FreeItemObject_ItemImage = FindChildRecursive(temp_FreeItemObject.transform, "Item_Image").GetComponent<Image>();

				TMP_Text temp_FreeItemObject_Price = FindChildRecursive(temp_FreeItemObject.transform, "Buy_PriceText").GetComponent<TMP_Text>();
				Image temp_FreeItemObject_CurrencyType = FindChildRecursive(temp_FreeItemObject.transform, "Buy_Coin_Image").GetComponent<Image>();

				Button temp_FreeItemObject_Button = FindChildRecursive(temp_FreeItemObject.transform, "Buy_Button").GetComponent<Button>();

#if UNITY_EDITOR
				#region 개발자 디버그 코드

				if (temp_FreeItemObject_Title != null)
				{
					Debug.Log("'temp_FreeItemObject_Title'를 찾았습니다.");
				}
				else
				{
					Debug.Log("'temp_FreeItemObject_Title'를 찾을 수 없습니다.");
				}

				if (temp_FreeItemObject_ItemImage != null)
				{
					Debug.Log("'temp_FreeItemObject_ItemImage'를 찾았습니다.");
				}
				else
				{
					Debug.Log("'temp_FreeItemObject_ItemImage'를 찾을 수 없습니다.");
				}

				if (temp_FreeItemObject_Price != null)
				{
					Debug.Log("'temp_FreeItemObject_Price'를 찾았습니다.");
				}
				else
				{
					Debug.Log("'temp_FreeItemObject_Price'를 찾을 수 없습니다.");
				}

				#endregion
#endif

				if (freeitemDatabase.items.Length == freeItemsToDisplay)    // 무료 아이템 베이스의 등록되어 있는 아이템의 개수가 'freeItemsToDisplay'와 같다면
				{
					temp_FreeItemObject_Title.text = freeitemDatabase.items[_ItemCurrentIndex].itemName;
					temp_FreeItemObject_ItemImage.sprite = freeitemDatabase.items[_ItemCurrentIndex].SkillIcon;
					temp_FreeItemObject_ItemImage.SetNativeSize();

					temp_FreeItemObject_Price.text = freeitemDatabase.items[_ItemCurrentIndex].price.ToString("#,##0");

					temp_FreeItemObject_Button.onClick.AddListener(() => OnClickBuy(freeitemDatabase.items[_ItemCurrentIndex].itemName, temp_FreeItemObject.transform, freeitemDatabase.items[_ItemCurrentIndex], int.Parse(temp_FreeItemObject_Price.text)));

					switch (freeitemDatabase.items[_ItemCurrentIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_FreeItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_FreeItemObject_CurrencyType'의 화폐는 코인입니다.");
							break;
						case CurrencyType.Crystal:
							temp_FreeItemObject_CurrencyType.sprite = crystalSprite;
							temp_FreeItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_FreeItemObject_CurrencyType'의 화폐는 크리스탈입니다.");
							break;
					}

					if (freeitemDatabase.items[_ItemCurrentIndex].isUnlocking)
                    {
                        GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
                        alreadyBuy_Panel.SetActive(true);
                    }

					temp_FreeItemObject_CurrencyType.SetNativeSize();

					availablefreeItems.Add(freeitemDatabase.items[_ItemCurrentIndex]);

					_ItemCurrentIndex++;
				}
				else
				{
					while (availablefreeItems.Count < freeItemsToDisplay) // 선택 될 아이템의 RandomIndex를 검증하고, 마지막으로 선택한다.
					{
						_ItemCurrentRandomIndex = Random.Range(0, freeitemDatabase.items.Length);
						_ItemCurrentInserted = false;

						if (_ItemsCurrentSelectIndex != null)
						{
							foreach (Item availablefreeItem in availablefreeItems)
							{
								if (availablefreeItem.itemName == freeitemDatabase.items[_ItemCurrentRandomIndex].itemName)
								{
									Debug.Log($"이미 삽입 되어 있는 같은 아이템이 있어 {freeitemDatabase.items[_ItemCurrentRandomIndex].itemName}은(는) 등록되지 않습니다.");
									_ItemCurrentInserted = true;
									break;
								}
							}
						}

						if (_ItemCurrentInserted)
						{
							Debug.Log($"아이템 삽입 감지 : {_ItemCurrentInserted}이므로 반복문을 넘겼습니다.");
							continue;
						}

						Debug.Log($"아이템 삽입 감지 : {_ItemCurrentInserted}이므로 반복문 넘기지 않았습니다.");

						availablefreeItems.Add(freeitemDatabase.items[_ItemCurrentRandomIndex]);
						break;
					}

					temp_FreeItemObject_Title.text = freeitemDatabase.items[_ItemCurrentRandomIndex].itemName;
					temp_FreeItemObject_ItemImage.sprite = freeitemDatabase.items[_ItemCurrentRandomIndex].SkillIcon;
					temp_FreeItemObject_ItemImage.SetNativeSize();

					temp_FreeItemObject_Price.text = freeitemDatabase.items[_ItemCurrentRandomIndex].price.ToString("#,##0");

					temp_FreeItemObject_Button.onClick.AddListener(() => OnClickBuy(freeitemDatabase.items[_ItemCurrentRandomIndex].itemName, temp_FreeItemObject.transform, freeitemDatabase.items[_ItemCurrentRandomIndex], int.Parse(temp_FreeItemObject_Price.text)));

					switch (freeitemDatabase.items[_ItemCurrentRandomIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_FreeItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_FreeItemObject_CurrencyType'의 화폐는 코인입니다.");
							break;
						case CurrencyType.Crystal:
							temp_FreeItemObject_CurrencyType.sprite = crystalSprite;
							temp_FreeItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_FreeItemObject_CurrencyType'의 화폐는 크리스탈입니다.");
							break;
					}

                    if (freeitemDatabase.items[_ItemCurrentRandomIndex].isUnlocking)
                    {
                        GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
                        alreadyBuy_Panel.SetActive(true);
                    }

                    temp_FreeItemObject_CurrencyType.SetNativeSize();

					_ItemsCurrentSelectIndex.Add(freeitemDatabase.items[_ItemCurrentRandomIndex]);
				}
			}
		}

		#endregion

		#endregion

		#region 구매 아이템

		foreach (Transform childObjectUI in itemBuyUIParent)   // 혹여나 이전에 등록 된 아이템 UI가 존재한다면 순회하며 삭제한다.
		{
			TMP_Text itemTitle = FindChildRecursive(childObjectUI.transform, "Item_Title").GetComponent<TMP_Text>();

			if (itemTitle != null)
			{
				Item itemToRemove = availablebuyItems.FirstOrDefault(item => item.itemName == itemTitle.text);

				if (itemToRemove != null)
				{
					availablebuyItems.Remove(itemToRemove);
				}
			}

			Destroy(childObjectUI);
		}

		#region 구매 아이템 상점 설정

		if (buyitemDatabase != null)
		{
			List<Item> _ItemsCurrentSelectIndex = new List<Item>();

			int _ItemCurrentRandomIndex = 0;
			int _ItemCurrentIndex = 0;

			bool _ItemCurrentInserted;

			for (int i = 0; i < buyitemsToDisplay; i++)
			{
				GameObject temp_BuyItemObject = Instantiate(itemUIPrefab, itemBuyUIParent);

				TMP_Text temp_BuyItemObject_Title = FindChildRecursive(temp_BuyItemObject.transform, "Item_Title").GetComponent<TMP_Text>();
				Image temp_BuyItemObject_ItemImage = FindChildRecursive(temp_BuyItemObject.transform, "Item_Image").GetComponent<Image>();

				TMP_Text temp_BuyItemObject_Price = FindChildRecursive(temp_BuyItemObject.transform, "Buy_PriceText").GetComponent<TMP_Text>();
				Image temp_BuyItemObject_CurrencyType = FindChildRecursive(temp_BuyItemObject.transform, "Buy_Coin_Image").GetComponent<Image>();

				Button temp_BuyItemObject_Button = FindChildRecursive(temp_BuyItemObject.transform, "Buy_Button").GetComponent<Button>();

#if UNITY_EDITOR
				#region 개발자 디버그 코드

				if (temp_BuyItemObject_Title != null)
				{
					Debug.Log("'temp_BuyItemObject_Title'를 찾았습니다.");
				}
				else
				{
					Debug.Log("'temp_BuyItemObject_Title'를 찾을 수 없습니다.");
				}

				if (temp_BuyItemObject_ItemImage != null)
				{
					Debug.Log("'temp_BuyItemObject_ItemImage'를 찾았습니다.");
				}
				else
				{
					Debug.Log("'temp_BuyItemObject_ItemImage'를 찾을 수 없습니다.");
				}

				if (temp_BuyItemObject_Price != null)
				{
					Debug.Log("'temp_BuyItemObject_Price'를 찾았습니다.");
				}
				else
				{
					Debug.Log("'temp_BuyItemObject_Price'를 찾을 수 없습니다.");
				}

				#endregion
#endif

				if (buyitemDatabase.items.Length == buyitemsToDisplay)    // 무료 아이템 베이스의 등록되어 있는 아이템의 개수가 'freeItemsToDisplay'와 같다면
				{
					temp_BuyItemObject_Title.text = buyitemDatabase.items[_ItemCurrentIndex].itemName;
					temp_BuyItemObject_ItemImage.sprite = buyitemDatabase.items[_ItemCurrentIndex].SkillIcon;
					temp_BuyItemObject_ItemImage.SetNativeSize();

					temp_BuyItemObject_Price.text = buyitemDatabase.items[_ItemCurrentIndex].price.ToString("#,##0");

					temp_BuyItemObject_Button.onClick.AddListener(() => OnClickBuy(buyitemDatabase.items[_ItemCurrentIndex].itemName, temp_BuyItemObject.transform, buyitemDatabase.items[_ItemCurrentIndex], int.Parse(temp_BuyItemObject_Price.text)));

					switch (buyitemDatabase.items[_ItemCurrentIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_BuyItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_BuyItemObject_CurrencyType'의 화폐는 코인입니다.");
							break;
						case CurrencyType.Crystal:
							temp_BuyItemObject_CurrencyType.sprite = crystalSprite;
							temp_BuyItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_BuyItemObject_CurrencyType'의 화폐는 크리스탈입니다.");
							break;
					}

					temp_BuyItemObject_CurrencyType.SetNativeSize();

                    if (buyitemDatabase.items[_ItemCurrentIndex].isUnlocking)
                    {
                        GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
                        alreadyBuy_Panel.SetActive(true);
                    }

                    availablebuyItems.Add(buyitemDatabase.items[_ItemCurrentIndex]);

					_ItemCurrentIndex++;
				}
				else  // 구매 아이템 베이스에 등록되어 있는 아이템의 개수가 'buyItemsToDisplay'보다 크다면
				{
					while (availablebuyItems.Count < buyitemsToDisplay) // 선택 될 아이템의 RandomIndex를 검증하고, 마지막으로 선택한다.
					{
						_ItemCurrentRandomIndex = Random.Range(0, buyitemDatabase.items.Length);
						_ItemCurrentInserted = false;

						if (_ItemsCurrentSelectIndex != null)
						{
							foreach (Item availablebuyItem in availablebuyItems)
							{
								if (availablebuyItem.itemName == buyitemDatabase.items[_ItemCurrentRandomIndex].itemName)
								{
									Debug.Log($"이미 삽입 되어 있는 같은 아이템이 있어 {buyitemDatabase.items[_ItemCurrentRandomIndex].itemName}은(는) 등록되지 않습니다.");
									_ItemCurrentInserted = true;
									break;
								}
							}
						}

						if (_ItemCurrentInserted)
						{
							Debug.Log($"아이템 삽입 감지 : {_ItemCurrentInserted}이므로 반복문을 넘겼습니다.");
							continue;
						}

						Debug.Log($"아이템 삽입 감지 : {_ItemCurrentInserted}이므로 반복문 넘기지 않았습니다.");

						availablebuyItems.Add(buyitemDatabase.items[_ItemCurrentRandomIndex]);
						break;
					}

					temp_BuyItemObject_Title.text = buyitemDatabase.items[_ItemCurrentRandomIndex].itemName;
					Debug.Log(temp_BuyItemObject_Title.text);
					temp_BuyItemObject_ItemImage.sprite = buyitemDatabase.items[_ItemCurrentRandomIndex].SkillIcon;
					temp_BuyItemObject_ItemImage.SetNativeSize();

					temp_BuyItemObject_Price.text = buyitemDatabase.items[_ItemCurrentRandomIndex].price.ToString("#,##0");

					temp_BuyItemObject_Button.onClick.AddListener(() => OnClickBuy(buyitemDatabase.items[_ItemCurrentRandomIndex].itemName, temp_BuyItemObject.transform, buyitemDatabase.items[_ItemCurrentRandomIndex], int.Parse(temp_BuyItemObject_Price.text)));

					switch (buyitemDatabase.items[_ItemCurrentRandomIndex].currencyType)
					{
						case CurrencyType.Coin:
							temp_BuyItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_BuyItemObject_CurrencyType'의 화폐는 코인입니다.");
							break;
						case CurrencyType.Crystal:
							temp_BuyItemObject_CurrencyType.sprite = crystalSprite;
							temp_BuyItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_BuyItemObject_CurrencyType'의 화폐는 크리스탈입니다.");
							break;
					}

                    if (buyitemDatabase.items[_ItemCurrentRandomIndex].isUnlocking)
                    {
                        GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
                        alreadyBuy_Panel.SetActive(true);
                    }

                    temp_BuyItemObject_CurrencyType.SetNativeSize();

					_ItemsCurrentSelectIndex.Add(buyitemDatabase.items[_ItemCurrentRandomIndex]);
				}
			}
		}

		#endregion

		#endregion
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

	private void OnApplicationQuit()
	{
		SaveItemData();
	}	// 게임이 종료할 때 실행되는 매서드

	private void SaveItemData()
	{
		string freeItemJson = JsonUtility.ToJson(new ItemListWrapper(availablefreeItems));
		Debug.Log(freeItemJson);
		PlayerPrefs.SetString(FreeItemKey, freeItemJson);

		string buyItemJson = JsonUtility.ToJson(new ItemListWrapper(availablebuyItems));
		Debug.Log(buyItemJson);
		PlayerPrefs.SetString(BuyItemKey, buyItemJson);

		// 테스트 코드
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}   // availableItems를 Json(String)으로 변환하고 변환 된 String을 PlayerPrefs에 저장한다.

	[Serializable]
	private class ItemListWrapper	// Json으로 변환 즉, 아이템 리스트를 직렬화하기 위해 사용 된 래퍼 클래스이다.
	{
		public List<Item> items;

		public ItemListWrapper(List<Item> items)
		{
			this.items = items;
		}
	}

	public void InstantiateItemUI()	// 아이템 등록이 되었다면, 이 매서드를 실행해 실제 볼 수 있는 게임 UI를 생성한다.
	{
		#region 무료 아이템

		foreach (Item avilableItem in availablefreeItems)	// 등록 된 아이템을 순회하면서,
		{
			foreach (Item dataBaseItem in freeitemDatabase.items)	// 데이터 베이스에 등록 된 아이템을 순회합니다.
			{
				if (dataBaseItem.itemName == avilableItem.itemName)	// 등록 된 아이템이 데이터 베이스에 존재한다면
				{
					Debug.Log("아이템 베이스에서 게임 종료 직전 저장 된 아이템을 찾았습니다.");

					GameObject temp_FreeItemObject = Instantiate(itemUIPrefab, itemFreeUIParent);

					TMP_Text temp_FreeItemObject_Title = FindChildRecursive(temp_FreeItemObject.transform, "Item_Title").GetComponent<TMP_Text>();
					Image temp_FreeItemObject_ItemImage = FindChildRecursive(temp_FreeItemObject.transform, "Item_Image").GetComponent<Image>();

					TMP_Text temp_FreeItemObject_Price = FindChildRecursive(temp_FreeItemObject.transform, "Buy_PriceText").GetComponent<TMP_Text>();
					Image temp_FreeItemObject_CurrencyType = FindChildRecursive(temp_FreeItemObject.transform, "Buy_Coin_Image").GetComponent<Image>();

					Button temp_FreeItemObject_Button = FindChildRecursive(temp_FreeItemObject.transform, "Buy_Button").GetComponent<Button>();

#if UNITY_EDITOR
					#region 개발자 디버그 코드

					if (temp_FreeItemObject_Title != null)
					{
						Debug.Log("'temp_FreeItemObject_Title'를 찾았습니다.");
					}
					else
					{
						Debug.Log("'temp_FreeItemObject_Title'를 찾을 수 없습니다.");
					}

					if (temp_FreeItemObject_ItemImage != null)
					{
						Debug.Log("'temp_FreeItemObject_ItemImage'를 찾았습니다.");
					}
					else
					{
						Debug.Log("'temp_FreeItemObject_ItemImage'를 찾을 수 없습니다.");
					}

					if (temp_FreeItemObject_Price != null)
					{
						Debug.Log("'temp_FreeItemObject_Price'를 찾았습니다.");
					}
					else
					{
						Debug.Log("'temp_FreeItemObject_Price'를 찾을 수 없습니다.");
					}

					#endregion
#endif

					temp_FreeItemObject_Title.text = avilableItem.itemName;
					temp_FreeItemObject_ItemImage.sprite = dataBaseItem.SkillIcon;
					temp_FreeItemObject_ItemImage.SetNativeSize();

					temp_FreeItemObject_Price.text = dataBaseItem.price.ToString("#,##0");

					temp_FreeItemObject_Button.onClick.AddListener(() => OnClickBuy(avilableItem.itemName, temp_FreeItemObject.transform, avilableItem, int.Parse(temp_FreeItemObject_Price.text)));

					switch (avilableItem.currencyType)
					{
						case CurrencyType.Coin:
							temp_FreeItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_FreeItemObject_CurrencyType'의 화폐는 코인입니다.");
							break;
						case CurrencyType.Crystal:
							temp_FreeItemObject_CurrencyType.sprite = crystalSprite;
							temp_FreeItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_FreeItemObject_CurrencyType'의 화폐는 크리스탈입니다.");
							break;
					}

					if (avilableItem.isUnlocking)
					{
                        GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
                        alreadyBuy_Panel.SetActive(true);
                    }

					temp_FreeItemObject_CurrencyType.SetNativeSize();
				}
			}

		}

		#endregion

		#region 구매 아이템

		foreach (Item avilableItem in availablebuyItems)   // 등록 된 아이템을 순회하면서,
		{
			foreach (Item dataBaseItem in buyitemDatabase.items)   // 데이터 베이스에 등록 된 아이템을 순회합니다.
			{
				if (dataBaseItem.itemName == avilableItem.itemName) // 등록 된 아이템이 데이터 베이스에 존재한다면
				{
					Debug.Log("아이템 베이스에서 게임 종료 직전 저장 된 아이템을 찾았습니다.");

					GameObject temp_BuyItemObject = Instantiate(itemUIPrefab, itemBuyUIParent);

					TMP_Text temp_BuyItemObject_Title = FindChildRecursive(temp_BuyItemObject.transform, "Item_Title").GetComponent<TMP_Text>();
					Image temp_BuyItemObject_ItemImage = FindChildRecursive(temp_BuyItemObject.transform, "Item_Image").GetComponent<Image>();

					TMP_Text temp_BuyItemObject_Price = FindChildRecursive(temp_BuyItemObject.transform, "Buy_PriceText").GetComponent<TMP_Text>();
					Image temp_BuyItemObject_CurrencyType = FindChildRecursive(temp_BuyItemObject.transform, "Buy_Coin_Image").GetComponent<Image>();

					Button temp_FreeItemObject_Button = FindChildRecursive(temp_BuyItemObject.transform, "Buy_Button").GetComponent<Button>();

#if UNITY_EDITOR
					#region 개발자 디버그 코드

					if (temp_BuyItemObject_Title != null)
					{
						Debug.Log("'temp_BuyItemObject_Title'를 찾았습니다.");
					}
					else
					{
						Debug.Log("'temp_BuyItemObject_Title'를 찾을 수 없습니다.");
					}

					if (temp_BuyItemObject_ItemImage != null)
					{
						Debug.Log("'temp_BuyItemObject_ItemImage'를 찾았습니다.");
					}
					else
					{
						Debug.Log("'temp_BuyItemObject_ItemImage'를 찾을 수 없습니다.");
					}

					if (temp_BuyItemObject_Price != null)
					{
						Debug.Log("'temp_BuyItemObject_Price'를 찾았습니다.");
					}
					else
					{
						Debug.Log("'temp_BuyItemObject_Price'를 찾을 수 없습니다.");
					}

					#endregion
#endif

					temp_BuyItemObject_Title.text = avilableItem.itemName;
					temp_BuyItemObject_ItemImage.sprite = dataBaseItem.SkillIcon;
					temp_BuyItemObject_ItemImage.SetNativeSize();

					temp_FreeItemObject_Button.onClick.AddListener(() => OnClickBuy(avilableItem.itemName, temp_BuyItemObject.transform, avilableItem, int.Parse(temp_BuyItemObject_Price.text)));

					temp_BuyItemObject_Price.text = dataBaseItem.price.ToString("#,##0");

					switch (avilableItem.currencyType)
					{
						case CurrencyType.Coin:
							temp_BuyItemObject_CurrencyType.sprite = coinSprite;
							Debug.Log("'temp_BuyItemObject_CurrencyType'의 화폐는 코인입니다.");
							break;
						case CurrencyType.Crystal:
							temp_BuyItemObject_CurrencyType.sprite = crystalSprite;
							temp_BuyItemObject_CurrencyType.transform.localScale = new Vector3(.8f, .8f, .8f);
							Debug.Log("'temp_BuyItemObject_CurrencyType'의 화폐는 크리스탈입니다.");
							break;
					}

					if (avilableItem.isUnlocking)
					{
                        GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
                        alreadyBuy_Panel.SetActive(true);
                    }

					temp_BuyItemObject_CurrencyType.SetNativeSize();
				}
			}

		}

		#endregion
	}

	public void OnClickBuy(string itemName, Transform transform, Item baseItem, int price)
	{
		foreach (Item item in availablefreeItems)
		{
			if (item.itemName == itemName)
			{
				if (!item.isUnlocking)
				{
					switch (item.currencyType)
					{
						case CurrencyType.Coin:
							if (PlayerWalletManager.Instance.TotalCoin >= price)
							{
								PlayerWalletManager.Instance.TotalCoin -= price;
								GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
								alreadyBuy_Panel.SetActive(true);
							}
							else
							{
								Debug.LogError("현재 코인이 부족합니다.");
								return;
							}
							break;
						case CurrencyType.Crystal:
							if (PlayerWalletManager.Instance.TotalCrystal >= price)
							{
								PlayerWalletManager.Instance.TotalCrystal -= price;
								GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
								alreadyBuy_Panel.SetActive(true);
							}
							else
							{
								Debug.LogError("현재 크리스탈이 부족합니다.");
								return;
							}
							break;
					}

					PlayerWalletManager.Instance.UnLockingItem(item);
					return;
				}
				else
				{
					Debug.LogError($"{item.itemName}은 이미 잠금해제한 아이템 입니다.");
				}
			}
		}

		foreach (Item item in availablebuyItems)
		{
			if (item.itemName == itemName)
			{
				if (!item.isUnlocking)
				{
					switch (item.currencyType)
					{
						case CurrencyType.Coin:
							if (PlayerWalletManager.Instance.TotalCoin >= price)
							{
								PlayerWalletManager.Instance.TotalCoin -= price;
								GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
								alreadyBuy_Panel.SetActive(true);
							}
							else
							{
								Debug.LogError("현재 코인이 부족합니다.");
								return;
							}
							break;
						case CurrencyType.Crystal:
							if (PlayerWalletManager.Instance.TotalCrystal >= price)
							{
								PlayerWalletManager.Instance.TotalCrystal -= price;
								GameObject alreadyBuy_Panel = FindChildRecursive(transform, "AlreadyBuy_Panel").gameObject;
								alreadyBuy_Panel.SetActive(true);
							}
							else
							{
								Debug.LogError("현재 크리스탈이 부족합니다.");
								return;
							}
							break;
					}

					PlayerWalletManager.Instance.UnLockingItem(item);
					return;
				}
				else
				{
					Debug.LogError($"{item.itemName}은 이미 잠금해제한 아이템 입니다.");
				}
			}
		}
	}
}
