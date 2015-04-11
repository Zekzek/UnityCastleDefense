using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathfinderTile
{
	// B : Block
	// R : Ramp
	
	private Tile.TILE_TYPE tileType;
	private PathfinderTile[,,] neighbors = new PathfinderTile[3,3,3];
	private ThreadedPathfinder.MOVE_TYPE[,,] moveTypes = new ThreadedPathfinder.MOVE_TYPE[3,3,3];
	private int x, y, z;
	
	public PathfinderTile(Tile.TILE_TYPE tileType, int[] pos) {
		this.tileType = tileType;
		x = pos [0];
		y = pos [1];
		z = pos [2];
	}
	
	public void Destroy() {
		for (int x = 0; x < 3; x++) {
			for (int y = 0; y < 3; y++) {
				for (int z = 0; z < 3; z++) {
					neighbors[x,y,z].neighbors[2-x,2-y,2-z] = null;
					neighbors[x,y,z].moveTypes[2-x,2-y,2-z] = ThreadedPathfinder.MOVE_TYPE.None;
					neighbors[x,y,z] = null;
					moveTypes[x,y,z] = ThreadedPathfinder.MOVE_TYPE.None;
				}
			}
		}
	}

	private void UpdateWalkMoves() {
		if (tileType == Tile.TILE_TYPE.Block) {
			//Check to the N, E, S, and W. 
			//Has a 'Block' on current level and nothing on top of it
			if (neighbors [1, 1, 2] != null && neighbors [1 ,1 ,2].TileType == Tile.TILE_TYPE.Block 
			    && neighbors [1, 2, 2] == null) {
				// N
				moveTypes [1, 1, 2] = ThreadedPathfinder.MOVE_TYPE.Walk;
			}
			if (neighbors [2, 1, 1] != null && neighbors [2 ,1 ,1].TileType == Tile.TILE_TYPE.Block 
			    && neighbors [2, 2, 1] == null) {
				// E
				moveTypes [2, 1, 1] = ThreadedPathfinder.MOVE_TYPE.Walk;
			}
			if (neighbors [1, 1, 0] != null && neighbors [1 ,1 ,0].TileType == Tile.TILE_TYPE.Block 
			    && neighbors [1, 2, 0] == null) {
				// S
				moveTypes [1, 1, 0] = ThreadedPathfinder.MOVE_TYPE.Walk;
			}
			if (neighbors [0, 1, 1] != null && neighbors [0 ,1 ,1].TileType == Tile.TILE_TYPE.Block 
			    && neighbors [0, 2, 1] == null) {
				// W
				moveTypes [0, 1, 1] = ThreadedPathfinder.MOVE_TYPE.Walk;
			}
		}
	}

	private void UpdateWalkDiagonalMoves() {
		if (tileType == Tile.TILE_TYPE.Block) {
			//Check to the NE, SE, SW, and NW.
			//Has a 'Block' on current level with nothing on top
			//'Walk' is available in adjacent squares (ie. N and E) 
			if (neighbors [2, 1, 2] != null  && neighbors [2 ,1 ,2].TileType == Tile.TILE_TYPE.Block
			    && neighbors [2, 2, 2] == null && moveTypes [1, 1, 2] == ThreadedPathfinder.MOVE_TYPE.Walk 
			    && moveTypes [2, 1, 1] == ThreadedPathfinder.MOVE_TYPE.Walk) {
				// NE
				moveTypes [2, 1, 2] = ThreadedPathfinder.MOVE_TYPE.Walk_Diagonal;
			}
			if (neighbors [0, 1, 2] != null  && neighbors [0 ,1 ,2].TileType == Tile.TILE_TYPE.Block
			    && neighbors [0, 2, 2] == null && moveTypes [1, 1, 2] == ThreadedPathfinder.MOVE_TYPE.Walk 
			    && moveTypes [0, 1, 2] == ThreadedPathfinder.MOVE_TYPE.Walk) {
				// SE
				moveTypes [0, 1, 2] = ThreadedPathfinder.MOVE_TYPE.Walk_Diagonal;
			}
			if (neighbors [0, 1, 0] != null && neighbors [0 ,1 ,0].TileType == Tile.TILE_TYPE.Block
			    && neighbors [0, 2, 0] == null && moveTypes [1, 1, 0] == ThreadedPathfinder.MOVE_TYPE.Walk 
			    && moveTypes [0, 1, 1] == ThreadedPathfinder.MOVE_TYPE.Walk) {
				// SW
				moveTypes [0, 1, 0] = ThreadedPathfinder.MOVE_TYPE.Walk_Diagonal;
			}
			if (neighbors [2, 1, 0] != null  && neighbors [2 ,1 ,0].TileType == Tile.TILE_TYPE.Block
			    && neighbors [2, 2, 0] == null && moveTypes [1, 1, 0] == ThreadedPathfinder.MOVE_TYPE.Walk 
			    && moveTypes [2, 1, 1] == ThreadedPathfinder.MOVE_TYPE.Walk) {
				// NW
				moveTypes [2, 1, 0] = ThreadedPathfinder.MOVE_TYPE.Walk_Diagonal;
			}

		}
	}

	private void UpdateWalkUpDownMoves() {
		//Check top and bottom of each ramp type (N, E, S, and W)
		//Bottom has a 'Block' or 'Ramp'(same type) below with nothing on top of it
		//Top has a 'Block' or 'Ramp'(same type) on current level with nothing on top of it

		if (tileType == Tile.TILE_TYPE.Ramp_N) {
			if (neighbors [1, 2, 2] == null) {
				if (neighbors [1, 1, 2] != null && neighbors [1, 1, 2].TileType == Tile.TILE_TYPE.Block) {
					moveTypes [1, 1, 2] = ThreadedPathfinder.MOVE_TYPE.Walk_Up;
				}
			} else if (neighbors [1, 2, 2].TileType == Tile.TILE_TYPE.Ramp_N) {
				moveTypes [1, 2, 2] = ThreadedPathfinder.MOVE_TYPE.Walk_Up;
			}
			if (neighbors [1, 0, 0] != null && (neighbors [1, 0, 0].TileType == Tile.TILE_TYPE.Block 
				|| neighbors [1, 0, 0].TileType == Tile.TILE_TYPE.Ramp_N) && neighbors [1, 1, 0] == null) {
				// S Bottom
				moveTypes [1, 0, 0] = ThreadedPathfinder.MOVE_TYPE.Walk_Down;
			}
		} else if (tileType == Tile.TILE_TYPE.Ramp_E) {
			if (neighbors [2, 2, 1] == null) {
				if (neighbors [2, 1, 1] != null && neighbors [2, 1, 1].TileType == Tile.TILE_TYPE.Block) {
					moveTypes [2, 1, 1] = ThreadedPathfinder.MOVE_TYPE.Walk_Up;
				}
			} else if (neighbors [2, 2, 1].TileType == Tile.TILE_TYPE.Ramp_E) {
				moveTypes [2, 2, 1] = ThreadedPathfinder.MOVE_TYPE.Walk_Up;
			}
			if (neighbors [0, 0, 1] != null && (neighbors [0, 0, 1].TileType == Tile.TILE_TYPE.Block 
				|| neighbors [0, 0, 1].TileType == Tile.TILE_TYPE.Ramp_E) && neighbors [0, 1, 1] == null) {
				// W Bottom
				moveTypes [0, 0, 1] = ThreadedPathfinder.MOVE_TYPE.Walk_Down;
			}
		} else if (tileType == Tile.TILE_TYPE.Ramp_S) {
			if (neighbors [1, 2, 0] == null) {
				if (neighbors [1, 1, 0] != null && neighbors [1, 1, 0].TileType == Tile.TILE_TYPE.Block) {
					moveTypes [1, 1, 0] = ThreadedPathfinder.MOVE_TYPE.Walk_Up;
				}
			} else if (neighbors [1, 2, 0].TileType == Tile.TILE_TYPE.Ramp_S) {
				moveTypes [1, 2, 0] = ThreadedPathfinder.MOVE_TYPE.Walk_Up;
			}
			if (neighbors [1, 0, 2] != null && (neighbors [1, 0, 2].TileType == Tile.TILE_TYPE.Block 
				|| neighbors [1, 0, 2].TileType == Tile.TILE_TYPE.Ramp_S) && neighbors [1, 1, 2] == null) {
				moveTypes [1, 0, 2] = ThreadedPathfinder.MOVE_TYPE.Walk_Down;
			}
		} else if (tileType == Tile.TILE_TYPE.Ramp_W) {
			if (neighbors [0, 2, 1] == null) {
				if (neighbors [0, 1, 1] != null && neighbors [0, 1, 1].TileType == Tile.TILE_TYPE.Block) {
					moveTypes [0, 1, 1] = ThreadedPathfinder.MOVE_TYPE.Walk_Up;
				}
			} else if (neighbors [0, 2, 1].TileType == Tile.TILE_TYPE.Ramp_W) {
				moveTypes [0, 2, 1] = ThreadedPathfinder.MOVE_TYPE.Walk_Up;
			}
			if (neighbors [2, 0, 1] != null && (neighbors [2, 0, 1].TileType == Tile.TILE_TYPE.Block 
				|| neighbors [2, 0, 1].TileType == Tile.TILE_TYPE.Ramp_W) && neighbors [2, 1, 1] == null) {
				moveTypes [2, 0, 1] = ThreadedPathfinder.MOVE_TYPE.Walk_Down;
			}
		} else if (tileType == Tile.TILE_TYPE.Block) {
			// N
			if (neighbors [1, 1, 2] != null && neighbors [1 ,1 ,2].TileType == Tile.TILE_TYPE.Ramp_S
			    && neighbors [1, 2, 2] == null) {
				moveTypes [1, 1, 2] = ThreadedPathfinder.MOVE_TYPE.Walk_Down;
			}
			else if (neighbors [1, 2, 2] != null && neighbors [1 ,2 ,2].TileType == Tile.TILE_TYPE.Ramp_N) {
				moveTypes [1, 2, 2] = ThreadedPathfinder.MOVE_TYPE.Walk_Up;
			}
			// E
			if (neighbors [2, 1, 1] != null && neighbors [2, 1, 1].TileType == Tile.TILE_TYPE.Ramp_W
			    && neighbors [2, 2, 1] == null) {
				moveTypes [2, 1, 1] = ThreadedPathfinder.MOVE_TYPE.Walk_Down;
			}
			else if (neighbors [2, 2, 1] != null && neighbors [2, 2, 1].TileType == Tile.TILE_TYPE.Ramp_E) {
				moveTypes [2, 2, 1] = ThreadedPathfinder.MOVE_TYPE.Walk_Up;
			}
			// S
			if (neighbors [1, 1, 0] != null && neighbors [1 ,1 ,0].TileType == Tile.TILE_TYPE.Ramp_N
			    && neighbors [1, 2, 0] == null) {
				moveTypes [1, 1, 0] = ThreadedPathfinder.MOVE_TYPE.Walk_Down;
			}
			else if (neighbors [1, 2, 0] != null && neighbors [1 ,2 ,0].TileType == Tile.TILE_TYPE.Ramp_S) {
				moveTypes [1, 2, 0] = ThreadedPathfinder.MOVE_TYPE.Walk_Up;
			}
			// W
			if (neighbors [0, 1, 1] != null && neighbors [0, 1, 1].TileType == Tile.TILE_TYPE.Ramp_E 
			    && neighbors [0, 2, 1] == null) {
				moveTypes [0, 1, 1] = ThreadedPathfinder.MOVE_TYPE.Walk_Down;
			}
			else if (neighbors [0, 2, 1] != null && neighbors [0, 2, 1].TileType == Tile.TILE_TYPE.Ramp_W) {
				moveTypes [0, 2, 1] = ThreadedPathfinder.MOVE_TYPE.Walk_Up;
			}
		}
	}
	
	public void UpdateNeighborMoveTypes() {
		for (int x = 0; x < 3; x++) {
			for (int y = 0; y < 3; y++) {
				for (int z = 0; z < 3; z++) {
					moveTypes[x,y,z] = ThreadedPathfinder.MOVE_TYPE.None;
				}
			}
		}
		if (neighbors[1,2,1] != null) {
			return;
		}
		UpdateWalkMoves ();
		UpdateWalkDiagonalMoves ();
		UpdateWalkUpDownMoves ();
	}

	public List<PathfinderTile> GetNeighbors() {
		List<PathfinderTile> accessibleNeighbors = new List<PathfinderTile> ();
		for (int x = 0; x < 3; x++) {
			for (int y = 0; y < 3; y++) {
				for (int z = 0; z < 3; z++) {
					if (!moveTypes [x, y, z].Equals (ThreadedPathfinder.MOVE_TYPE.None)) {
						accessibleNeighbors.Add (neighbors [x, y, z]);
					}
				}
			}
		}
		return accessibleNeighbors;
	}

	public float GetCost(PathfinderTile neighborTile) {
		ThreadedPathfinder.MOVE_TYPE moveType = ThreadedPathfinder.MOVE_TYPE.None; 
		for (int x = 0; x < 3; x++) {
			for (int y = 0; y < 3; y++) {
				for (int z = 0; z < 3; z++) {
					if(neighbors[x,y,z] != null && neighbors[x,y,z].Equals(neighborTile)) {
						moveType = moveTypes[x,y,z];
					}
				}
			}
		}

		if (moveType == null) { //not a neighbor
			return float.NaN;
		} else if (moveType == ThreadedPathfinder.MOVE_TYPE.Walk) {
			return 1f;
		} else if (moveType == ThreadedPathfinder.MOVE_TYPE.Walk_Diagonal) {
			return 1.5f;
		} else if (moveType == ThreadedPathfinder.MOVE_TYPE.Walk_Up) {
			return 1.5f;
		} else if (moveType == ThreadedPathfinder.MOVE_TYPE.Walk_Down) {
			return 0.75f;
		} else { //A neighbor not reachable with any known moveTypesment type
			return float.NaN;
		}
	}
	
	public string ToString() {
		return tileType + ": " + x + "," + y + "," + z;
	}

	public bool Equals(PathfinderTile other) {
		return x == other.x && y == other.y && z == other.z && tileType == other.tileType;
	}

	public Tile.TILE_TYPE TileType {
		get { return tileType; }
	}

	public PathfinderTile[,,] Neighbors {
		set { neighbors = value; }
	}

	public int[] IndexPos {
		get { return new int[] { x, y, z }; }
	}
}