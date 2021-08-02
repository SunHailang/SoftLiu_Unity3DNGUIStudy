using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    const float miniPathUpdateTime = 0.2f;
    const float pathUpdateMoveThreshold = 0.5f;

    public Grid m_grid;

    public Transform target;

    public float speed = 10;
    public float turnDst = 5;
    public float turnSpeed = 5;
    public float stoppingDst = 10;

    Path path;

    private List<Node> openList = new List<Node>();
    private List<Node> closeList = new List<Node>();

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    public void OnPathFound(Vector3[] wayPoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(wayPoints, transform.position, turnDst, stoppingDst);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }
        openList.Clear();
        closeList.Clear();
        Vector3 tarPos = m_grid.FindCanWalkPoint(target.position, ref openList, ref closeList);

        PathRequestManager.RequestPath(new PathRequest(transform.position, tarPos, OnPathFound));

        float sqrMoveTreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(miniPathUpdateTime);
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveTreshold)
            {
                openList.Clear();
                closeList.Clear();
                tarPos = m_grid.FindCanWalkPoint(target.position, ref openList, ref closeList);
                PathRequestManager.RequestPath(new PathRequest(transform.position, tarPos, OnPathFound));
                targetPosOld = target.position;
            }
        }
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.loolPoints[0]);

        float speedPercent = 1;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                    if (stoppingDst < 0.01f)
                    {
                        followingPath = false;
                    }
                }
                Quaternion targetRotation = Quaternion.LookRotation(path.loolPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }

    float _time = 0.0f;
    System.Random _random;

    int seek = 10;

    private void OnEnable()
    {
        
        
    }

    private void LateUpdate()
    {
        _time += Time.deltaTime;
        if (_time > 1.5f)
        {
            UnityEngine.Random.InitState(seek);
            Debug.LogError($"Random Key: {UnityEngine.Random.Range(1, 15)}, Value:{UnityEngine.Random.value}");
            _random = new System.Random(seek);
            Debug.LogError($"Random Value:{_random.Next(1, 15)}, Seek:{0}");
            _time = 0f;
            //seek++;
        }
    }
}
