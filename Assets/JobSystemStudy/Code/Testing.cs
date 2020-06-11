using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;

public class Testing : MonoBehaviour
{

    public bool useJobs = false;

    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (!useJobs)
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
        }
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

    private JobHandle ReallyToughrJobTask()
    {
        ReallyToughrJob job = new ReallyToughrJob();
        return job.Schedule();
    }

}

[BurstCompile]
public struct ReallyToughrJob : IJob
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
