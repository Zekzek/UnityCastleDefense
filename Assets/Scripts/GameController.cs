using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	public GameObject unitPrefab;
	public GameObject projectilePrefab;
	public GameObject flagPrefab;
	public GameObject rampPrefab;
	public GameObject[] blockPrefabs;
	
	public char[,,] initialLayout = Maps.kingOfTheHill;
	private List<Tile> tileList = new List<Tile>();

	private Unit clickedUnit;
	private Tile clickedTile;
	private ThreadedPathfinder finder;

	void Start () {
		finder = new ThreadedPathfinder ();
		for (int x = 0; x < Maps.mapXSize; x++) {
			for (int y = 0; y < Maps.mapYSize; y++) {
				for (int z = 0; z < Maps.mapZSize; z++) {
					int[] indexPos = new int[] {x, y, Maps.mapZSize - 1 - z};
					// initialLayout encoded y,z,x(with z inverted) for easy visualization
					if (initialLayout [y, z, x] == 'B') {
						AddTile (GetRandomBlockPrefab (), indexPos, Tile.TILE_TYPE.Block);
					} else if (initialLayout [y, z, x] == 'b') {
						AddTile (blockPrefabs [0], indexPos, Tile.TILE_TYPE.Block);
					} else if (initialLayout [y, z, x] == 'N') {
						AddTile (rampPrefab, indexPos, Tile.TILE_TYPE.Ramp_N);
					} else if (initialLayout [y, z, x] == 'E') {
						AddTile (rampPrefab, indexPos, Tile.TILE_TYPE.Ramp_E);
					} else if (initialLayout [y, z, x] == 'S') {
						AddTile (rampPrefab, indexPos, Tile.TILE_TYPE.Ramp_S);
					} else if (initialLayout [y, z, x] == 'W') {
						AddTile (rampPrefab, indexPos, Tile.TILE_TYPE.Ramp_W);
					} else if (initialLayout [y, z, x] == 'F') {
						AddFlag (indexPos);
					}
				}
			}
		}
		finder.Pack ();
		removeTileAt(new int[] {10, 1, 5});
		finder.Pack ();

		Unit unit1 = AddUnit (new int[] {1, 2, 1});
		Unit unit2 = AddUnit (new int[] {19, 2, 10});
	}

	void Update() {
		PathfinderPath shortest = finder.CheckShortestPath ();
		if (shortest != null) {
			Debug.Log (shortest.ToString());
			clickedUnit.MovePath = shortest;
			finder.ClearShortestPath();
		}
	}

	private Tile AddTile(GameObject prefab, int[] indexPos, Tile.TILE_TYPE tileType) {
		finder.AddTile (tileType, indexPos);

		Vector3 position = new Vector3 
			(indexPos[0] * Tile.BLOCK_SIZE[0], indexPos[1] * Tile.BLOCK_SIZE[1], indexPos[2] + Tile.BLOCK_SIZE[2]);
		if (getTileAtIndex(indexPos) != null) {
			return null;
		}
		GameObject tileObject = (GameObject) Instantiate (prefab, position, Quaternion.identity);
		Tile aTile = tileObject.GetComponent(typeof(Tile)) as Tile;
		aTile.setType (tileType);
		aTile.IndexPos = indexPos;
		aTile.Controller = this;
		tileList.Add (aTile);

		return aTile;
	}

	private GameObject GetRandomBlockPrefab() {
		return blockPrefabs[Mathf.FloorToInt(Random.value * blockPrefabs.Length)];
	}
	
	private Unit AddUnit(int[] indexPos) {
		Vector3 position = new Vector3 
			(indexPos[0] * Tile.BLOCK_SIZE[0], indexPos[1] * Tile.BLOCK_SIZE[1], indexPos[2] + Tile.BLOCK_SIZE[2]);
		GameObject unitObject = (GameObject) Instantiate (unitPrefab, position, Quaternion.identity);
		Unit aUnit = unitObject.GetComponent(typeof(Unit)) as Unit;
		aUnit.Controller = this;
		aUnit.OnTile = getTileAtIndex (new int[] {indexPos[0], indexPos[1] - 1, indexPos[2]});
		aUnit.UpdateUnityPosition ();
		return aUnit;
	}

	private void AddFlag(int[] indexPos) {
		Vector3 position = new Vector3 
			(indexPos[0] * Tile.BLOCK_SIZE[0], indexPos[1] * Tile.BLOCK_SIZE[1], indexPos[2] + Tile.BLOCK_SIZE[2]);
		GameObject flagObject = (GameObject) Instantiate (flagPrefab, position, Quaternion.identity);
		flagObject.transform.rotation = Quaternion.Euler(new Vector3(-90, -30, 0));

		Debug.Log ("Flag added at " + indexPos [0] + "," + indexPos [1] + "," + indexPos [2]);

		//Unit aUnit = unitObject.GetComponent(typeof(Unit)) as Unit;
		//aUnit.Controller = this;
		//aUnit.OnTile = getTileAtIndex (new int[] {indexPos[0], indexPos[1] - 1, indexPos[2]});
		//aUnit.UpdateUnityPosition ();
		//return aUnit;
	}

	private Tile getTileAt(Vector3 position) {
		for (int i = 0; i < tileList.Count; i++) {
			if (tileList[i].Position == position) {
				return tileList[i];
			}
		}
		return null;
	}

	public Tile getTileAtIndex(int[] indexPos) {
		for (int i = 0; i < tileList.Count; i++) {
			if (tileList[i].IndexPos[0] == indexPos[0]
			    && tileList[i].IndexPos[1] == indexPos[1]
			    && tileList[i].IndexPos[2] == indexPos[2]) {
				return tileList[i];
			}
		}
		return null;
	}

	public void removeTileAt(int[] indexPos) {
		finder.RemoveTile (indexPos);
		Tile removeTile = getTileAtIndex (indexPos);
		removeTile.Destroy ();
		tileList.Remove (removeTile);
	}

	public void ClickedTile(Tile aTile) {
		clickedTile = aTile;
		if (clickedUnit != null) {
			Tile startTile = clickedUnit.OnTile;
			finder.StartShortestPath (startTile.IndexPos, aTile.IndexPos);
			/*ScoredTileList path = getShortestPath(startTile, aTile);
			if (path != null) {
				Debug.Log("Shortest Path: " + path.ToString());
				finder.StartShortestPath (startTile.IndexPos, aTile.IndexPos);
				clickedUnit.MovePath = path;
			}
			else {
				Debug.Log("Shortest Path: null");
			}*/
		}
	}

	public void ClickedUnit(Unit aUnit) {
		if (clickedUnit != null && clickedUnit != aUnit) {
			GameObject arrowObject = (GameObject) Instantiate (projectilePrefab, 
				clickedUnit.gameObject.transform.position, Quaternion.identity);
			Projectile aProjectile = arrowObject.GetComponent(typeof(Projectile)) as Projectile;
			aProjectile.Fire (clickedUnit, aUnit, 5);
		}
		clickedUnit = aUnit;
	}
}
