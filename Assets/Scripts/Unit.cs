using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
	private float MAGIC_NUMBER_FORCE = 100;
	private float CENTERED_ON_TILE_DISTANCE = 0.3f;
	private float ON_TILE_DISTANCE = 0.5f;

	private float increaseSpeedHistory = 0;
	public float speed = 1f;
	private List<ThreadedPathfinder.MOVE_TYPE> movementAvailable;

	private GameController controller;
	private Bounds colliderBounds;
	private PathfinderPath movePath;

	private Tile onTile;
	private float startTime = 0f;
	private float duration = 0f;

	public void Awake() {
		movementAvailable = new List<ThreadedPathfinder.MOVE_TYPE> ();
		movementAvailable.Add (ThreadedPathfinder.MOVE_TYPE.Walk);
		movementAvailable.Add (ThreadedPathfinder.MOVE_TYPE.Walk_Diagonal);
		colliderBounds = (gameObject.GetComponent (typeof(CapsuleCollider)) as CapsuleCollider).bounds;
	}

	public void Update() {
		if (movePath != null && movePath.Count > 1) {
			float time = Time.time;
			if (time - startTime >= duration) {
				Debug.Log ("OnTile: " + onTile.ToString());
				startTime = time;
				duration = 1f;
				movePath.RemoveFirst ();
				onTile = controller.getTileAtIndex(movePath.First.IndexPos);
				if (movePath.Second == null) {
					return;
				}
			}
			gameObject.transform.position = Vector3.Lerp(
				Tile.GetOnTileUnityPos(movePath.First), 
				Tile.GetOnTileUnityPos(movePath.Second), 
				(time - startTime) / duration);
			Debug.Log("Path is: " + movePath.ToString());
		}
	}
	
	public void Destroy() {
		Destroy (gameObject);
	}

	public void OnMouseDown() {
		controller.ClickedUnit (this);
	}
	
	public bool isAtPosition(Vector3 position) {
		return gameObject.transform.position == position;
	}

	public void UpdateUnityPosition() {
		if (onTile != null) {
			gameObject.transform.position = Tile.GetOnTileUnityPos (onTile); 
		}
	}

	public string ToString() {
		return "Unit: " + transform.position;
	}

	public GameController Controller{
		set { controller = value; }
	}

	public Tile OnTile {
		get { return onTile; }
		set { onTile = value; }
	}

	public PathfinderPath MovePath {
		set { 
			movePath = value; 
			startTime = Time.time;
		}
	}
}
