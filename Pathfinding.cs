﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

	GridSystem grid;
	PathRequestManager requestManager;

	void Start()
	{
		requestManager = GetComponent<PathRequestManager> ();
		grid = GetComponent<GridSystem> ();
	}

	public void StartFindPath (Vector3 startPos, Vector3 targetPos)
	{
		StartCoroutine (FindPath (startPos, targetPos));
	}

	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
	{
        Node startNode = grid.NodeFromWorldPoint (startPos);
		Node targetNode = grid.NodeFromWorldPoint (targetPos);

        Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		if (startNode.walkable && targetNode.walkable)
		{
			Heap<Node> openSet = new Heap<Node> (grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node> ();
			openSet.Add (startNode);

			while (openSet.Count > 0)
			{
				Node currentNode = openSet.RemoveFirst ();
				closedSet.Add (currentNode);

				if (currentNode == targetNode)
				{
					pathSuccess = true;
					break;
				}

				foreach (Node neighbor in grid.GetNeighbors(currentNode))
				{
					if (!neighbor.walkable || closedSet.Contains (neighbor))
					{
						continue;
					}

					int newMovementCostToNeighbor = currentNode.gCost + GetDistance (currentNode, neighbor) + neighbor.movementPenalty;

					if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains (neighbor))
					{
						neighbor.gCost = newMovementCostToNeighbor;
						neighbor.hCost = GetDistance (neighbor, targetNode);
						neighbor.parent = currentNode;

						if (!openSet.Contains (neighbor))
						{
							openSet.Add (neighbor);
						}
						else
						{
							openSet.UpdateItem (neighbor);
						}
					}
				}
			}
		}
		yield return null;
		if (pathSuccess)
		{
			waypoints = RetracePath (startNode, targetNode);
            grid.ResetCosts();
		}
		requestManager.FinishedProcessingPath (waypoints, pathSuccess);
	}

	Vector3[] RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath (path);
		Array.Reverse (waypoints);
		return waypoints;
	}

	Vector3[] SimplifyPath(List<Node> path)
	{
		List<Vector3> waypoints = new List<Vector3> ();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++)
		{
			Vector2 directionNew = new Vector2 (path [i - 1].gridX - path [i].gridX, path [i - 1].gridY - path [i].gridY);
			if (directionNew != directionOld)
			{
				waypoints.Add (path [i].worldPos);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}

	int GetDistance(Node nodeA, Node nodeB)
	{
		int distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (distX > distY) 
		{
			return 14 * distY + 10 * (distX - distY);
		}
		return 14 * distX + 10 * (distY - distX);
		
	}
}