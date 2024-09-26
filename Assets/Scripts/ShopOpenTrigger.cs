using UnityEngine;

public class ShopOpenTrigger : MonoBehaviour
{
	private bool alreayOpen;
	[SerializeField] private ShopManager shopManager;

	private void OnEnable()
	{
		InvokeRepeating("InvokeInitializationItems", 0.0f, 1.0f);

		if (!alreayOpen)
		{
			shopManager.LoadItemData(); // 게임 종료 직전 저장 된 아이템이 있는 지 검사 후 불러옵니다.
			shopManager.FirstTimeOpenShopUI(); // 최초로 상점을 오픈 했을 때 아이템 베이스에 있는 랜덤한 아이템을 등록합니다.
			alreayOpen = true;
		}
	}

	private void OnDisable()
	{
		CancelInvoke("InvokeInitializationItems");
	}

	private void InvokeInitializationItems()
	{
		shopManager.InitializationItems();
	}
}
