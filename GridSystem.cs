using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour {

    static public List<Node> WALKABLENODES;
    

	Node[,] grid;
	public bool gridGizmos;
	public LayerMask unwalkableMask;
	public float nodeRadius;
    public List<Node> walkableNodes;
    public Vector2 gridWorldSize;
	float nodeDiameter;
	int gridSizeX;
	int gridSizeY;

    int penaltyMax = int.MinValue;
    int penaltyMin = int.MaxValue;

	void Awake()
	{
		nodeDiameter = nodeRadius * 2f;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
        WALKABLENODES = new List<Node>();
        CreateGrid ();
        walkableNodes = WALKABLENODES;
    }

	public int MaxSize
	{
		get
		{
			return gridSizeX * gridSizeY;
		}
	}

	void CreateGrid()
	{
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - transform.right * gridWorldSize.x / 2 - transform.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + transform.right * (x * nodeDiameter + nodeRadius) + transform.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckBox(worldPoint, Vector3.one * nodeRadius, Quaternion.identity, unwalkableMask));
                int movementPenalty = 0;
                if(!walkable)
                {
                    movementPenalty = 20;
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);

                if (walkable)
                {
                    WALKABLENODES.Add(grid[x,y]);
                }
            }
		}

        BlurPenaltyMap(3);
	}

    void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltiesVerticalPass = new int[gridSizeX,gridSizeY];

        //Horizontal pass
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
            }

            for (int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex,y].movementPenalty + grid[addIndex, y].movementPenalty;
            }
        }

        //Vertical pass
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid[x, y].movementPenalty = blurredPenalty;
                
                if (blurredPenalty > penaltyMax)
                {
                    penaltyMax = blurredPenalty;
                }
                if (blurredPenalty < penaltyMin)
                {
                    penaltyMin = blurredPenalty;
                }
            }
        }
    }

	public List<Node> GetNeighbors(Node node)
	{
		List<Node> neighbors = new List<Node> ();

		for (int x = -1; x <= 1; x++) 
		{
			for (int y = -1; y <= 1; y++) 
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if(checkX >=0 && checkX <gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbors.Add (grid [checkX, checkY]);
				}
			}
		}
		return neighbors;
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y;

		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        
		return grid [x, y];
	}

    public void ResetCosts()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                grid[x, y].gCost = 0;
                grid[x, y].hCost = 0;
            }
        }
    }

	void OnDrawGizmos()
	{
		if (grid != null && gridGizmos) 
		{
			foreach (Node n in grid) 
			{
                Gizmos.color = Color.Lerp(Color.white, Color.red, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));

				Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
				Gizmos.DrawCube (n.worldPos, Vector3.one * (nodeDiameter));
			}
		}
	}
}
