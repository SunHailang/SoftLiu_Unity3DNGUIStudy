using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;

public class PathFinding : MonoBehaviour
{
    public Transform m_player;
    public Transform m_target;

    private PathGrid grid;
    public PathData pathData;
    private void Awake()
    {
        grid = GetComponent<PathGrid>();
    }

    private void Start()
    {
        //m_target.position = m_player.position;
    }

    private void Update()
    {
        if (m_target.position == m_player.position) return;

        //NativeList<Node> 

        FindPathJob findPathJob = new FindPathJob()
        {
            gridData = grid.pathGridData,
            startPos = m_player.position,
            endPos = m_target.position,
            callback = FindPathJobCallback
        };
        JobHandle handle = findPathJob.Schedule();
        handle.Complete();
    }

    public void FindPathJobCallback(Node n)
    {
        Vector3[] points = RetarcePath(null, n);
        Debug.Log(points.Length);
    }

    private Vector3[] FindPath(Vector3 start, Vector3 end)
    {
        Node startNode = grid.pathGridData.NodeFromWorldPoint(start);
        Node endNode = grid.pathGridData.NodeFromWorldPoint(end);

        Node recentWalk = startNode;
        int minDistance = int.MaxValue;

        Heap<Node> openSet = new Heap<Node>(grid.pathGridData.MaxSize);
        HashSet<Node> closeSet = new HashSet<Node>();
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closeSet.Add(currentNode);

            if (currentNode == endNode)
            {
                // 找到了
                break;
            }
            // 获取相邻节点
            List<Node> neighbours = grid.pathGridData.GetNeighbours(currentNode);
            foreach (Node n in neighbours)
            {
                if (!n.walkable || closeSet.Contains(n))
                    continue;
                int newCostToNeighbours = currentNode.gCost + GetDistence(currentNode, endNode);
                if (newCostToNeighbours < n.gCost || !openSet.Contains(n))
                {
                    // 更新 gCost
                    n.gCost = newCostToNeighbours;
                    int hCost = GetDistence(n, endNode);
                    n.hCost = hCost;
                    n.parent = currentNode;
                    if (!openSet.Contains(n))
                    {
                        openSet.Add(n);
                        // 更新最近的节点
                        if (hCost < minDistance)
                        {
                            minDistance = hCost;
                            recentWalk = n;
                        }
                    }
                    else
                    {
                        openSet.UpdateItem(n);
                    }
                }
            }
        }
        // 获取路径
        if (recentWalk == startNode)
        {
            //在起始点没动
            return null;
        }
        else
        {
            return RetarcePath(startNode, recentWalk);
        }
    }

    private Vector3[] RetarcePath(Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node curNode = targetNode;
        while (curNode != startNode)
        {
            path.Add(curNode);
            curNode = curNode.parent;
        }
        Vector3[] wayPoints = SimplifyPath(path);
        Array.Reverse(wayPoints);
        return wayPoints;
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> wayPoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                wayPoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return wayPoints.ToArray();
    }

    private int GetDistence(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
}

[BurstCompile]
public struct FindPathJob : IJob
{
    public GridData gridData;
    public float3 startPos;
    public float3 endPos;

    public Action<Node> callback;

    public void Execute()
    {
        Node node = findPath(gridData, startPos, endPos);
        callback(node);
    }

    private Node findPath(GridData grid, float3 start, float3 end)
    {
        Node startNode = grid.NodeFromWorldPoint(start);
        Node endNode = grid.NodeFromWorldPoint(end);

        Node recentWalk = startNode;
        int minDistance = int.MaxValue;

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closeSet = new HashSet<Node>();
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closeSet.Add(currentNode);

            if (currentNode == endNode)
            {
                // 找到了
                recentWalk = endNode;
                break;
            }
            // 获取相邻节点
            List<Node> neighbours = grid.GetNeighbours(currentNode);
            foreach (Node n in neighbours)
            {
                if (!n.walkable || closeSet.Contains(n))
                    continue;
                int newCostToNeighbours = currentNode.gCost + GetDistence(currentNode, endNode);
                if (newCostToNeighbours < n.gCost || !openSet.Contains(n))
                {
                    // 更新 gCost
                    n.gCost = newCostToNeighbours;
                    int hCost = GetDistence(n, endNode);
                    n.hCost = hCost;
                    n.parent = currentNode;
                    if (!openSet.Contains(n))
                    {
                        openSet.Add(n);
                        // 更新最近的节点
                        if (hCost < minDistance)
                        {
                            minDistance = hCost;
                            recentWalk = n;
                        }
                    }
                    else
                    {
                        openSet.UpdateItem(n);
                    }
                }
            }
        }
        // 获取路径
        return recentWalk;//RetarcePath(startNode, recentWalk);
    }

    private float3[] RetarcePath(Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node curNode = targetNode;
        while (curNode != startNode)
        {
            path.Add(curNode);
            curNode = curNode.parent;
        }
        float3[] wayPoints = SimplifyPath(path);
        //Array.Reverse(wayPoints);
        return wayPoints;
    }

    private float3[] SimplifyPath(List<Node> path)
    {
        NativeList<float3> wayPoints = new NativeList<float3>(0);
        float2 directionOld = float2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            float2 directionNew = new float2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (!directionNew.Equals(directionOld))
            {
                wayPoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        float3[] points = wayPoints.ToArray();
        wayPoints.Dispose();
        return points;
    }

    private int GetDistence(Node nodeA, Node nodeB)
    {
        int dstX = math.abs(nodeA.gridX - nodeB.gridX);
        int dstY = math.abs(nodeA.gridY - nodeB.gridY);
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
