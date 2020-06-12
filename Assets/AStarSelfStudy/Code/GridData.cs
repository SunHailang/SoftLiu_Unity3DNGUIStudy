using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridData
{


    private Node[,] grid;
    public Node[,] gridData
    {
        get { return grid; }
    }

    private int gridSizeX;
    private int gridSizeY;

    float2 gridWorldSize;


    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public GridData(int sizeX, int sizeY, float2 worldSize)
    {
        grid = new Node[sizeX, sizeY];
        gridWorldSize = worldSize;
    }

    public void SetData(int x, int y, Node n)
    {
        grid[x, y] = n;
    }

    public Node GetData(int x, int y)
    {
        return grid[x, y];
    }

    public void SetWays(float3[] points)
    {

    }

    public Node NodeFromWorldPoint(float3 worldPosition)
    {
        float precentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float precentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        precentX = Mathf.Clamp01(precentX);
        precentY = Mathf.Clamp01(precentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * precentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * precentY);
        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

}
