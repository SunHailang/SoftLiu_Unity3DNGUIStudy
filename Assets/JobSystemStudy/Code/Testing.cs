using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Jobs;

public class Testing : MonoBehaviour
{

    public bool useJobs = false;
    public GameObject prefab;

    List<PrefabMover> prefabs = new List<PrefabMover>();

    public class PrefabMover
    {
        public Transform trans;
        public float speed;
    }

    private void Start()
    {
        for (int i = 0; i < 10000; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.localPosition = new Vector3(UnityEngine.Random.Range(-12.0f, 12.0f), UnityEngine.Random.Range(6.0f, -4.0f), 0);
            obj.transform.localRotation = Quaternion.identity;
            prefabs.Add(new PrefabMover() { trans = obj.transform, speed = UnityEngine.Random.Range(2, 3) });
        }
    }

    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (useJobs)
        {
            //NativeArray<float3> postions = new NativeArray<float3>(prefabs.Count, Allocator.TempJob);
            NativeArray<float> speeds = new NativeArray<float>(prefabs.Count, Allocator.TempJob);
            TransformAccessArray trans = new TransformAccessArray(prefabs.Count);
            for (int i = 0; i < prefabs.Count; i++)
            {
                //postions[i] = prefabs[i].trans.localPosition;
                trans.Add(prefabs[i].trans);
                speeds[i] = prefabs[i].speed;
            }
            //ReallyToughParallelJob job = new ReallyToughParallelJob()
            ReallyToughTransJob job = new ReallyToughTransJob()
            {
                //positionArray = postions,
                speedArray = speeds,
                deltaTime = Time.deltaTime
            };
            //JobHandle handle = job.Schedule(prefabs.Count, 100);
            JobHandle handle = job.Schedule(trans);
            handle.Complete();
            for (int i = 0; i < prefabs.Count; i++)
            {
                //prefabs[i].trans.localPosition = postions[i];
                //prefabs[i].trans = trans[i];
                prefabs[i].speed = speeds[i];
            }
            //postions.Dispose();
            speeds.Dispose();
            trans.Dispose();
        }
        else
        {
            ReallyToughMoveTask();
        }
        /*if (!useJobs)
        {
            for (int i = 0; i < 10; i++)
            {
                ReallyToughTask();
            }
        }
        else
        {
            NativeList<JobHandle> handles = new NativeList<JobHandle>(Allocator.Temp);
            for (int i = 0; i < 10; i++)
            {
                JobHandle handle = ReallyToughrJobTask();
                handles.Add(handle);
            }
            JobHandle.CompleteAll(handles);
            handles.Dispose();
        }*/
        Debug.Log((Time.realtimeSinceStartup - startTime) * 1000 + "ms");
    }

    private void ReallyToughTask()
    {
        float value = 0;
        for (int i = 0; i < 500000; i++)
        {
            value = math.exp10(math.rsqrt(value));
        }
    }

    private void ReallyToughMoveTask()
    {
        foreach (var item in prefabs)
        {
            item.trans.localPosition += Vector3.up * Time.deltaTime * item.speed;
            if (item.trans.localPosition.y > 6f)
            {
                item.speed = -Mathf.Abs(item.speed);
            }
            else if (item.trans.localPosition.y < -4f)
            {
                item.speed = Mathf.Abs(item.speed);
            }
            float value = 0;
            for (int i = 0; i < 1000; i++)
            {
                value = math.exp10(math.rsqrt(value));
            }
        }
    }

    private JobHandle ReallyToughrJobTask()
    {
        ReallyToughJob job = new ReallyToughJob();
        return job.Schedule();
    }
    //private JobHandle ReallyToughrMoveJobTask()
    //{
    //    ReallyToughParallelJob job = new ReallyToughParallelJob() {
    //        positionArray = 
    //    };
    //    return job.Schedule();
    //}

}
/*
  Job 不能运行在主线程
     
*/

[BurstCompile]
public struct ReallyToughJob : IJob
{
    public void Execute()
    {
        float value = 0;

        for (int i = 0; i < 500000; i++)
        {
            value = math.exp10(math.rsqrt(value));
        }
    }
}

[BurstCompile]
public struct ReallyToughParallelJob : IJobParallelFor
{
    public NativeArray<float3> positionArray;
    public NativeArray<float> speedArray;
    [ReadOnly]
    public float deltaTime;

    public void Execute(int index)
    {
        positionArray[index] += new float3(0, 1, 0) * deltaTime * speedArray[index];
        if (positionArray[index].y > 6f)
        {
            speedArray[index] = -math.abs(-speedArray[index]);
        }
        else if (positionArray[index].y < -4f)
        {
            speedArray[index] = math.abs(speedArray[index]);
        }
        float value = 0;
        for (int i = 0; i < 1000; i++)
        {
            value = math.exp10(math.rsqrt(value));
        }
    }
}

[BurstCompile]
public struct ReallyToughTransJob : IJobParallelForTransform
{
    public NativeArray<float> speedArray;
    [ReadOnly]
    public float deltaTime;
    public void Execute(int index, TransformAccess transform)
    {
        transform.localPosition += new Vector3(0, 1, 0) * deltaTime * speedArray[index];
        if (transform.localPosition.y > 6f)
        {
            speedArray[index] = -math.abs(-speedArray[index]);
        }
        else if (transform.localPosition.y < -4f)
        {
            speedArray[index] = math.abs(speedArray[index]);
        }
        float value = 0;
        for (int i = 0; i < 1000; i++)
        {
            value = math.exp10(math.rsqrt(value));
        }
    }
}
