using UnityEngine;
using UnityEngine.EventSystems; // EventSystem을 사용하기 위한 네임스페이스

public class UpgradeTower : MonoBehaviour
{
	[SerializeField] private UIManager uiManager;

	[Header("User Interface")]
	[SerializeField] private RectTransform towerManagementWindow; // RectTransform으로 변경

	[Header("System Variables")]
	[SerializeField] private LayerMask towerLayerMask; // 타워 레이어만 포함된 LayerMask 설정
	[SerializeField] private LayerMask uiLayerMask;
	[SerializeField] private Vector2 offset; // UI를 타워 위치로부터 이동시키는 오프셋

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			// UI를 클릭했는지 확인
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				// UI가 아닌 곳을 클릭했으면 UI를 닫음
				if (towerManagementWindow.gameObject.activeSelf)
				{
					towerManagementWindow.gameObject.SetActive(false);
				}
				else
				{
					SelectTower();
				}
			}
			else
			{
				return;
			}
		}
	}

	private void SelectTower()
	{
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		RaycastHit2D towerHit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, towerLayerMask);
		if (towerHit.collider != null && towerHit.transform.CompareTag("Tower"))
		{
			ShowTowerUI(towerHit.collider.gameObject);
			ViewerTowerInfo.Instance.RefreshInterface(towerHit.transform.gameObject);
		}
	}

	private void ShowTowerUI(GameObject tower)
	{
		// 타워의 월드 좌표 얻기
		Vector3 towerWorldPosition = tower.transform.position;

		// 타워의 화면 좌표 얻기
		Vector3 towerScreenPosition = Camera.main.WorldToScreenPoint(towerWorldPosition);

		// 화면 크기
		float screenWidth = Screen.width;
		float screenHeight = Screen.height;

		// UI의 RectTransform을 얻기
		RectTransform uiRectTransform = towerManagementWindow;

		// UI의 크기
		float uiWidth = uiRectTransform.rect.width;
		float uiHeight = uiRectTransform.rect.height;

		// 초기 UI 위치는 타워의 위쪽
		Vector3 newScreenPosition = towerScreenPosition + new Vector3(0, uiHeight / 2 + offset.y, 0);

		// UI의 위치 조정 (화면을 넘어가지 않도록)
		// 오른쪽 경계 확인 및 조정
		if (newScreenPosition.x + uiWidth / 2 > screenWidth)
		{
			newScreenPosition.x = screenWidth - uiWidth / 2 - offset.x;
		}

		// 왼쪽 경계 확인 및 조정
		if (newScreenPosition.x - uiWidth / 2 < 0)
		{
			newScreenPosition.x = uiWidth / 2 + offset.x;
		}

		// 상단 경계 확인 및 조정
		if (newScreenPosition.y + uiHeight / 2 > screenHeight)
		{
			newScreenPosition.y = screenHeight - uiHeight / 2 - offset.y;
		}

		// 하단 경계 확인 및 조정
		if (newScreenPosition.y - uiHeight / 2 < 0)
		{
			newScreenPosition.y = uiHeight / 2 + offset.y;
		}

		// 새로운 월드 좌표
		Vector3 newWorldPosition = Camera.main.ScreenToWorldPoint(newScreenPosition);
		newWorldPosition.z = towerWorldPosition.z; // z값은 타워의 z값으로 설정

		// UI의 새로운 위치 설정
		uiRectTransform.position = newWorldPosition;
		towerManagementWindow.gameObject.SetActive(true);
	}
}
