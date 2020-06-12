using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGrid : MonoBehaviour
{
    public bool displayGridGizmos = true;

    public int obstacleProximityPenalty = 10;

    public TerrainType[] walkableRegions;

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    LayerMask walkableMask;
    Dictionary<int, int> walkableRegionsDic = new Dictionary<int, int>();

    GridData grid;
    public GridData pathGridData
    {
        get { return grid; }
    }

    int penaltyMin = int.MinValue;
    int penaltyMax = int.MaxValue;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDic.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }

        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new GridData(gridSizeX, gridSizeY, gridWorldSize);
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                int movementPenalty = 0;
                Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, walkableMask))
                {
                    walkableRegionsDic.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                }

                if (!walkable)
                {
                    movementPenalty += obstacleProximityPenalty;
                }


                grid.SetData(x, y, new Node(walkable, worldPoint, x, y, movementPenalty));
            }
        }
        BlurPenaltyMap(5);
    }
    private void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExitents = (kernelSize - 1) / 2;

        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = -kernelExitents; x <= kernelExitents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExitents);
                penaltiesHorizontalPass[0, y] += grid.GetData(sampleX, y).movementPenalty;
            }
            for (int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExitents - 1, 0, gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExitents, 0, gridSizeX - 1);

                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid.GetData(removeIndex, y).movementPenalty + grid.GetData(addIndex, y).movementPenalty;
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExitents; y <= kernelExitents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExitents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }

            int blurredPenaltyZero = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            grid.GetData(x, 0).movementPenalty = blurredPenaltyZero;

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExitents - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExitents, 0, gridSizeY - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid.GetData(x, y).movementPenalty = blurredPenalty;

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

    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 0, gridWorldSize.y));

        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid.gridData)
            {
                //Gizmos.color = Color.white;// Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
            }
        }
    }
}
