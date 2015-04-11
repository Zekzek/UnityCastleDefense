using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathfinderPath
{
	private float cost;
	private List<PathfinderTile> tileList;

	public PathfinderPath() {
		tileList = new List<PathfinderTile> ();
	}

	public PathfinderPath Clone() {
		PathfinderPath path = new PathfinderPath ();
		for (int i = 0; i < tileList.Count; i++) {
			path.tileList.Add(tileList[i]);
		}
		path.cost = cost;
		return path;
	}

	public void AddTile(PathfinderTile tile) {
		if (tileList.Count == 0) {
			cost = 0;
		} else {
			cost += tileList [tileList.Count - 1].GetCost (tile);
		}
		tileList.Add (tile);
	}

	public void RemoveFirst() {
		if (tileList.Count > 0) {
			tileList.RemoveAt (0);
		}
	}

	public string ToString() {
		string path = "PathfinderPath(" + cost + "): ";
		for (int i = 0; i < tileList.Count; i++) {
			if (i > 0) {
				path += " => ";
			}
			if (tileList [i] != null) {
				path += tileList [i].ToString ();
			} else {
				path += "(null tile)";
			}
		}
		return path;
	}

	public PathfinderTile End {
		get { return tileList[tileList.Count-1]; }
	}
	
	public PathfinderTile First {
		get { return tileList[0]; }
	}
	
	public PathfinderTile Second {
		get { return tileList.Count > 1 ? tileList[1] : null; }
	}

	public int Count {
		get { return tileList.Count; }
	}

	public float Cost {
		get { return cost; }
		set { cost = value; }
	}
}
