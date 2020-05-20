using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuadTree
{

    public Rectangle boundary;
    public int capacity;
    public Queue<Point> points;

    public QuadTree northeast;
    public QuadTree northwest;
    public QuadTree southeast;
    public QuadTree southwest;

    public bool divided;
    public QuadTree(Rectangle boundary, int n)
    {
        this.boundary = boundary;
        this.capacity = n;
        this.points = new Queue<Point>();
        this.divided = false;
    }

    private void subdivde()
    {
        float x = this.boundary.x;
        float y = this.boundary.y;
        float w = this.boundary.w;
        float h = this.boundary.h;
        Rectangle ne = new Rectangle(x + w / 4, y + h / 4, w / 2, h / 2);
        this.northeast = new QuadTree(ne, this.capacity);
        Rectangle nw = new Rectangle(x - w / 4, y + h / 4, w / 2, h / 2);
        this.northwest = new QuadTree(nw, this.capacity);
        Rectangle se = new Rectangle(x + w / 4, y - h / 4, w / 2, h / 2);
        this.southeast = new QuadTree(se, this.capacity);
        Rectangle sw = new Rectangle(x - w / 4, y - h / 4, w / 2, h / 2);
        this.southwest = new QuadTree(sw, this.capacity);
        this.divided = true;
    }

    public bool insert(Point point)
    {
        if (!this.boundary.contains(point)) return false;

        if (this.points.Count < this.capacity)
        {
            this.points.Enqueue(point);
            return true;
        }
        else
        {
            if (!divided)
            {
                this.subdivde();
            }
            if (this.northeast.insert(point)) return true;
            else if (this.northwest.insert(point)) return true;
            else if (this.southeast.insert(point)) return true;
            else if (this.southwest.insert(point)) return true;
            else
                return false;
        }
    }

    public Queue<Point> query(Rectangle range, Queue<Point> found = null)
    {
        if (found == null) found = new Queue<Point>();

        if (!this.boundary.intersects(range))
        {
            return found;
        }
        else
        {
            IEnumerator ie = this.points.GetEnumerator();
            while (ie.MoveNext())
            {
                Point p = ie.Current as Point;
                if (p != null && range.contains(p))
                {
                    found.Enqueue(p);
                }
            }
            if (this.divided)
            {
                this.northwest.query(range, found);
                this.northeast.query(range, found);
                this.southwest.query(range, found);
                this.southeast.query(range, found);
            }
        }
        return found;
    }


    public void Show()
    {
        IEnumerator ie = this.points.GetEnumerator();
        while (ie.MoveNext())
        {
            Point p = (Point)ie.Current;
            if (p != null)
                p.data.color = Color.white;
        }

        Debug.DrawLine(this.boundary.leftDown, this.boundary.leftUp, Color.green);
        Debug.DrawLine(this.boundary.leftDown, this.boundary.rightDown, Color.green);
        Debug.DrawLine(this.boundary.leftUp, this.boundary.rightUp, Color.green);
        Debug.DrawLine(this.boundary.rightDown, this.boundary.rightUp, Color.green);
        if (this.divided)
        {
            this.northeast.Show();
            this.northwest.Show();
            this.southeast.Show();
            this.southwest.Show();
        }
    }
}

/// <summary>
/// 假设点是带有碰撞盒的
/// </summary>
public class Point
{
    public Transform trans;
    /// <summary>
    /// BoxCollider的大小
    /// </summary>
    public float w = 0;
    public float h = 0;

    private Image m_data;
    public Image data { get { return m_data; } }
    public Point(Transform trans, Image data)
    {
        this.trans = trans;
        this.m_data = data;
        // 初始化为 0
        //this.w = 0.01851845f;
        //this.h = 0.01851845f;
    }
}

public class Rectangle
{
    public float x;
    public float y;
    public float w;
    public float h;

    // 得到4个顶点的坐标
    public Vector2 leftDown;
    public Vector2 leftUp;
    public Vector2 rightDown;
    public Vector2 rightUp;

    public Rectangle(float x, float y, float w, float h)
    {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;

        this.leftDown = new Vector2(this.x - w / 2, this.y - h / 2);
        this.leftUp = new Vector2(this.x - w / 2, this.y + h / 2);
        this.rightDown = new Vector2(this.x + w / 2, this.y - h / 2);
        this.rightUp = new Vector2(this.x + w / 2, this.y + h / 2);

    }
    /// <summary>
    /// 带BoxCollider的大小  假设 BoxCollider的中心点和物体本身重合
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool contains(Point point)
    {
        return ((point.trans.position.x + point.w / 2) >= (this.x - this.w / 2) &&
            (point.trans.position.x - point.w / 2) <= (this.x + this.w / 2) &&
            (point.trans.position.y + point.h / 2) >= (this.y - this.h / 2) &&
            (point.trans.position.y - point.h / 2) <= (this.y + this.h / 2));
    }

    public bool intersects(Rectangle range)
    {
        return !(range.x - range.w / 2 > this.x + this.w / 2 ||
            range.x + range.w / 2 < this.x - this.w / 2 ||
            range.y - range.h / 2 > this.y + this.h / 2 ||
            range.y + range.h / 2 < this.y - this.h / 2);
    }
}

