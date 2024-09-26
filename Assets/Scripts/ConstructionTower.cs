using UnityEngine;
using UnityEngine.EventSystems;

public class ConstructionTower : MonoBehaviour
{
	[SerializeField] private GameObject towerConstructionObject;
	[SerializeField] private Vector2 offset = Vector2.one;
	[SerializeField] private LayerMask towerPlaceLayer;
	[SerializeField] private UIManager UIManager;

	[SerializeField] private ViewerTowerConstruction towerConstruction;

	private Camera cam;

	private void Awake()
	{
		cam = Camera.main;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			GUIManage();
		}
	}

	private void DetectPlaceTile()
	{
		Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
		mouseWorldPosition.z = 0;

		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}

		RaycastHit2D raycastHit2D = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, Mathf.Infinity, towerPlaceLayer);

		if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("TowerPlace"))
		{
			PlaceTile _PlaceTile = raycastHit2D.transform.GetComponent<PlaceTile>();

			if (_PlaceTile.IsTowerBuild)
			{
				UIManager.onTriggerTowerErrorNotify();
				return;
			}

			towerConstruction.PlaceTile = _PlaceTile;

			Vector2 tileWordlPosition = _PlaceTile.transform.position * offset;

			towerConstructionObject.transform.position = tileWordlPosition;
			towerConstruction.gameObject.SetActive(true);
		}
	}

	private void GUIManage()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			// UI를 클릭하지 않았다면
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				// UI가 아닌 곳을 클릭했으면 UI를 닫음
				if (towerConstruction.gameObject.activeSelf)
				{
					towerConstruction.gameObject.SetActive(false);
				}
				else
				{
					DetectPlaceTile();
				}
			}
			else
			{
				return;
			}
		}
	}
}
