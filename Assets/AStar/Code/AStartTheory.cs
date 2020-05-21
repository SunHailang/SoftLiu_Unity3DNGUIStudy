/*
    create：
    time：
 
 */


namespace Assets.AStar.Code
{
    // 计算玩家行走的最短路
    /*
     避开障碍物
     
     不停地找周围的点，选出一个新的点作为起点再循环的找， 一直到找到终点
     
    */

    /*
     起点：startPoint
     终点：endPoint
     
     寻路消耗公式： fCost = gCost + hCost;
     gCost: 离起点的距离  (1 和 1.4(勾股定理))

     曼哈顿街区算法
     hCost: 离终点的距离 (数格子， x方向 + y方向)

     开启列表(openSet)： 待搜索的点(找到 fCost最小的) (使用堆(Heap) 升序排)
        每次找堆(Heap)的根节点

     关闭列表(closeSet)： 已被搜索的点

     currentPoint = startPoint
     openSet.push(currentPoint)
     while openSet.Count > 0:
        currentPoint = openSet.RemoveFirst()
        closeSet.push(currentPoint)
        if currentPoint is endPoint :
            break;
        // 遍历 当前节点的邻居节点
        for p in grid.getNeighbours(currentPoint):
            if p not walkable || closeSet.contains(p):
                continue;
            // 获取邻居节点p的权重 gCost 和 hCost
            gCost = gValue
            hCost = hValue
            // 设置邻居节点的权重
            p.gCost = gCost
            p.hCost = hCost
            p.parent = currentPoint
            if !openSet.contains(p):
                openSet.push(p)
            else:
                openSet.updateItem(p)
            
    */
}
