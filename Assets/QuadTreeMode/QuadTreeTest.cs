using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuadTreeTest : MonoBehaviour
{
    [SerializeField]
    private Transform m_enemyParent = null;
    [SerializeField]
    private Transform m_enemyPrefab = null;
    [SerializeField]
    private Camera m_camera = null;

    private QuadTree m_tree = null;

    private float m_height = 0.2f;
    private float m_width = 0.2f;
    private Rectangle m_range = null;

    private void Start()
    {

        Vector3 test = m_camera.ScreenToWorldPoint(new Vector3(10, 0, 0));
        Vector3 zo = m_camera.ScreenToWorldPoint(Vector3.zero);
        Debug.Log(test.x - zo.x);

        //return;

        Vector3 start = new Vector3(520, 115);
        Vector3 startWorld = m_camera.ScreenToWorldPoint(start);
        Vector3 endWorld = m_camera.ScreenToWorldPoint(new Vector3(start.x + 1300, start.y + 850, 0));
        Debug.Log(startWorld + " -> " + endWorld);
        float w = endWorld.x - startWorld.x;
        float h = endWorld.y - startWorld.y;
        Rectangle rect = new Rectangle(startWorld.x + w / 2, startWorld.y + h / 2, w, h);


        m_tree = new QuadTree(rect, 4);

        for (int i = 0; i < 500; i++)
        {
            Transform obj = GameObject.Instantiate<Transform>(m_enemyPrefab, m_enemyParent);
            obj.name = string.Format("enemy" + i);
            float x = UnityEngine.Random.Range(-600, 600);
            float y = UnityEngine.Random.Range(-400, 400);
            Vector3 pos = new Vector3(x, y, 0);
            obj.localPosition = pos;
            //obj.position = m_camera.ScreenToWorldPoint(pos);
            //Debug.Log(obj.position);
            m_tree.insert(new Point(obj.position.x, obj.position.y, obj.GetComponent<Image>()));
        }
        Debug.Log("Insert End");

        m_range = new Rectangle(startWorld.x - m_width / 2, startWorld.y - m_height / 2, m_width, m_height);
    }

    private void FixedUpdate()
    {

    }

    private void Update()
    {

        //float horizontal = Input.GetAxis("Horizontal"); //获取垂直轴
        //float vertical = Input.GetAxis("Vertical");    //获取水平轴
        //m_range.y += vertical * Time.deltaTime;
        //m_range.x += horizontal * Time.deltaTime;

        Vector3 start = new Vector3(520, 115);
        Vector3 leftDown = m_camera.ScreenToWorldPoint(start);
        Vector3 leftUp = m_camera.ScreenToWorldPoint(new Vector3(start.x, start.y + 850));
        Vector3 rightDown = m_camera.ScreenToWorldPoint(new Vector3(start.x + 1300, start.y));
        Vector3 rightUp = m_camera.ScreenToWorldPoint(new Vector3(start.x + 1300, start.y + 850));

        Debug.DrawLine(leftDown, leftUp, Color.red);
        Debug.DrawLine(leftDown, rightDown, Color.red);
        Debug.DrawLine(leftUp, rightUp, Color.red);
        Debug.DrawLine(rightDown, rightUp, Color.red);

        m_tree.Show();

        Vector3 mouseWorld = m_camera.ScreenToWorldPoint(Input.mousePosition);

        m_range.y = mouseWorld.y;
        m_range.x = mouseWorld.x;

        //if (Input.GetMouseButtonDown(1))
        //{
        //    Debug.Log(m_range.x + " -> " + m_range.y);
        Queue<Point> points = m_tree.query(m_range);
        //Debug.Log(points.Count);
        IEnumerator ie = points.GetEnumerator();
        while (ie.MoveNext())
        {
            Point p = (Point)ie.Current;
            if (p != null)
                p.data.color = Color.red;
        }
        //}

        Vector3 leftDownRange = new Vector3(m_range.x - m_range.w / 2, m_range.y - m_range.h / 2);
        Vector3 leftUpRange = new Vector3(m_range.x - m_range.w / 2, m_range.y + m_range.h / 2);
        Vector3 rightDownRange = new Vector3(m_range.x + m_range.w / 2, m_range.y - m_range.h / 2);
        Vector3 rightUpRange = new Vector3(m_range.x + m_range.w / 2, m_range.y + m_range.h / 2);
        Debug.DrawLine(leftDownRange, leftUpRange, Color.blue);
        Debug.DrawLine(leftDownRange, rightDownRange, Color.blue);
        Debug.DrawLine(leftUpRange, rightUpRange, Color.blue);
        Debug.DrawLine(rightDownRange, rightUpRange, Color.blue);
        //m_tree.query()

    }
}
