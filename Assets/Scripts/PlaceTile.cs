using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceTile : MonoBehaviour
{
	public Tilemap buildTileMap;
	public TileBase buildTile;

	[SerializeField] private int cellCalibrationPosition;

	private bool isTowerTileBuild = false;
	private bool isTowerBuild = false;

	public bool IsTowerTileBuild
	{
		set => isTowerTileBuild = value;
		get => isTowerTileBuild;
	}

	public bool IsTowerBuild
	{
		set => isTowerBuild = value;
		get => isTowerBuild;
	}

	public void PlaceTowerTile()
	{
		if (isTowerTileBuild)
		{
			Debug.Log(55);
			return;
		}

		Vector3 towerPosition = transform.position;
		Vector3Int cellPosition = buildTileMap.WorldToCell(towerPosition);

		cellPosition.x -= cellCalibrationPosition;
		cellPosition.y -= cellCalibrationPosition;
		buildTileMap.SetTile(cellPosition, buildTile);

		isTowerTileBuild = true;
	}
}
