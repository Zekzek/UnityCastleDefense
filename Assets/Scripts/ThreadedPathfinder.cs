using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ThreadedPathfinder
{
	public enum MOVE_TYPE
	{
		Walk,
		Walk_Diagonal,
		Walk_Up,
		Walk_Down,
		None
	};

	private System.Threading.Thread shortestPathThread;
	private static int mapXSize = 21;
	private static int mapYSize = 11;
	private static int mapZSize = 11;
	private PathfinderTile[,,] map = new PathfinderTile[ mapXSize, mapYSize, mapZSize ];

	private PathfinderTile start;
	private PathfinderTile end;
	private PathfinderPath shortestPath;

	public void AddTile(Tile.TILE_TYPE tileType, int[] pos) {
		map [pos [0],pos [1],pos [2]] = new PathfinderTile(tileType, pos);
	}

	public void RemoveTile(int[] pos) {
		map [pos [0],pos [1],pos [2]] = null;
	}

	public void Pack() {
		for (int x = 0; x < mapXSize; x++) {
			for (int y = 0; y < mapYSize; y++) {
				for (int z = 0; z < mapZSize; z++) {
					if (map[x,y,z] != null) {
						PathfinderTile[,,] neighbors = new PathfinderTile[3,3,3];
						for (int dx = 0; dx < 3; dx++) {
							for (int dy = 0; dy < 3; dy++) {
								for (int dz = 0; dz < 3; dz++) {
									try {
										neighbors[dx,dy,dz] = map[x + dx - 1, y + dy - 1, z + dz - 1];
									}
									catch (IndexOutOfRangeException ignorable) {}
								}
							}
						}
						map[x,y,z].Neighbors = neighbors;
						map[x,y,z].UpdateNeighborMoveTypes();
					}
				}
			}
		}
	}

	public void StartShortestPath(int[] startPos, int[] endPos) {
		StartShortestPath (map [startPos[0], startPos[1], startPos[2]],
		                  map [endPos[0], endPos[1], endPos[2]]);
	}

	public void StartShortestPath(PathfinderTile start, PathfinderTile end) {
		this.start = start;
		this.end = end;
		shortestPath = null;
		shortestPathThread = new System.Threading.Thread (ShortestPath);
		shortestPathThread.Start ();
	}

	public PathfinderPath CheckShortestPath() {
		return shortestPath;
	}

	public void ClearShortestPath() {
		shortestPath = null;
	}

	private void ShortestPath() {
		Debug.Log("Shortest path between " + start.ToString() + " and " + end.ToString());
		
		List<PathfinderTile> explored = new List<PathfinderTile> ();
		List<PathfinderPath> frontier = new List<PathfinderPath> ();
		PathfinderPath path = new PathfinderPath();
		
		path.AddTile (start);
		frontier.Add (path);
		
		while(true) {
			if (frontier.Count == 0) {
				shortestPath = new PathfinderPath(); //failed to find a path
				return;
			}
			double lowestCost = double.MaxValue;
			int lowestCostIndex = 0;
			for (int i = 0; i < frontier.Count; i++) {
				if (frontier[i].Cost < lowestCost) {
					lowestCost = frontier[i].Cost;
					lowestCostIndex = i;
				}
			}
			path = frontier[lowestCostIndex];
			if (path.End.Equals(end)) {
				shortestPath = path;
				return;
			}
			frontier.RemoveAt(lowestCostIndex);
			explored.Add(path.End);
			List<PathfinderTile> neighbors = path.End.GetNeighbors();
			for (int i = 0; i < neighbors.Count; i++) {
				if (!explored.Contains(neighbors[i])) {
					int index = -1;
					for (int j = 0; j < frontier.Count; j++) {
						if (frontier[j].End.Equals(neighbors[i])) {
							index = j;
							break;
						}
					}
					if(index < 0) {
						PathfinderPath neighborPath = path.Clone();
						neighborPath.AddTile(neighbors[i]);
						if (!double.IsNaN(neighborPath.Cost)) {
							frontier.Add (neighborPath);
						}
					}
					else if (frontier[index].Cost < path.Cost) {
						path = frontier[index];
					}
				}
			}
		}
	}
}
