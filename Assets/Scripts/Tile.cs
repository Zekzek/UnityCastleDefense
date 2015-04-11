using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
	public static readonly Vector3 BLOCK_SIZE = new Vector3(1f, 0.3f, 1f);
	public enum TILE_TYPE
	{
		Block,
		Ramp_N,
		Ramp_E,
		Ramp_S,
		Ramp_W
	};

	protected TILE_TYPE tileType = TILE_TYPE.Block;
	private GameController controller;
	private int[] indexPos;
	
	public void Awake() {
		indexPos = new int[3];
	}

	public void Destroy() {
		Destroy (gameObject);
	}

	public void setType(TILE_TYPE tileType) {
		this.tileType = tileType;
		if (tileType == TILE_TYPE.Ramp_N) {
			gameObject.transform.rotation = Quaternion.Euler(new Vector3(-90, -90, 0));
		} else if (tileType == TILE_TYPE.Ramp_E) {
			gameObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
		} else if (tileType == TILE_TYPE.Ramp_S) {
			gameObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 90, 0));
		} else if (tileType == TILE_TYPE.Ramp_W) {
			gameObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 180, 0));
		}
	}

	public void OnMouseDown() {
		controller.ClickedTile (this);
	}

	public static Vector3 ConvertToUnityPos(int[] indexPos) {
		return new Vector3 (indexPos[0] * BLOCK_SIZE[0], indexPos[1] * BLOCK_SIZE[1], indexPos[2] * BLOCK_SIZE[2]);
	}

	public static Vector3 GetOnTileUnityPos(PathfinderTile tile) {
		int[] indexPos = tile.IndexPos;
		if (tile.TileType == TILE_TYPE.Block) {
			return new Vector3 (indexPos[0] * BLOCK_SIZE[0], (indexPos[1] + 1) * BLOCK_SIZE[1], indexPos[2] * BLOCK_SIZE[2]);
		} else if (tile.TileType == TILE_TYPE.Ramp_N || tile.TileType == TILE_TYPE.Ramp_E
		           || tile.TileType == TILE_TYPE.Ramp_S || tile.TileType == TILE_TYPE.Ramp_W) {
			return new Vector3 (indexPos[0] * BLOCK_SIZE[0], (indexPos[1] + 0.5f) * BLOCK_SIZE[1], indexPos[2] * BLOCK_SIZE[2]);
		} else {
			return new Vector3 (indexPos[0] * BLOCK_SIZE[0], (indexPos[1] + 1) * BLOCK_SIZE[1], indexPos[2] * BLOCK_SIZE[2]);
		}
	}

	public static Vector3 GetOnTileUnityPos(Tile tile) {
		int[] indexPos = tile.IndexPos;
		if (tile.TileType == TILE_TYPE.Block) {
			return new Vector3 (indexPos[0] * BLOCK_SIZE[0], (indexPos[1] + 1) * BLOCK_SIZE[1], indexPos[2] * BLOCK_SIZE[2]);
		} else if (tile.TileType == TILE_TYPE.Ramp_N || tile.TileType == TILE_TYPE.Ramp_E
		           || tile.TileType == TILE_TYPE.Ramp_S || tile.TileType == TILE_TYPE.Ramp_W) {
			return new Vector3 (indexPos[0] * BLOCK_SIZE[0], (indexPos[1] + 0.5f) * BLOCK_SIZE[1], indexPos[2] * BLOCK_SIZE[2]);
		} else {
			return new Vector3 (indexPos[0] * BLOCK_SIZE[0], (indexPos[1] + 1) * BLOCK_SIZE[1], indexPos[2] * BLOCK_SIZE[2]);
		}
	}

	public string ToString() {
		return tileType + ": " + indexPos[0] + "," + indexPos[1] + "," + indexPos[2];
	}

	public Vector3 Position {
		get { return gameObject.transform.position; }
	}

	public int[] IndexPos {
		get { return indexPos; }
		set { 
			indexPos = value;
			gameObject.transform.position = ConvertToUnityPos(indexPos);
		}
	}

	public TILE_TYPE TileType{
		get { return tileType; }
		set { tileType = value; }
	}

	public GameController Controller{
		set { controller = value; }
	}
}
