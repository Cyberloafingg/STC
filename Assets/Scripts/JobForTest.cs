using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobForTest : MonoBehaviour
{
    public bool useJob;
    public int dataCount;
    private NativeArray<float> a;

    private NativeArray<float> b;

    private NativeArray<float> result;

    private List<float> noJobA;

    private List<float> noJobB;

    private List<float> noJobResult;
    // Update is called once per frame
    private void Start()
    {
        a = new NativeArray<float>(dataCount, Allocator.Persistent);
        b = new NativeArray<float>(dataCount, Allocator.Persistent);
        result = new NativeArray<float>(dataCount, Allocator.Persistent);
        noJobA = new List<float>();
        noJobB = new List<float>();
        noJobResult = new List<float>();

        for (int i = 0; i < dataCount; ++i)
        {
            a[i] = 1.0f;
            b[i] = 2.0f;
            noJobA.Add(1.0f);
            noJobB.Add(2.0f);
            noJobResult.Add(0.0f);
        }
    }

    void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (useJob)
        {
            MyParallelJob jobData = new MyParallelJob();
            jobData.a = a;
            jobData.b = b;
            jobData.result = result;
            // 调度作业，为结果数组中的每个索引执行一个 Execute 方法，且每个处理批次只处理一项
            JobHandle handle = jobData.Schedule(result.Length, 1);
            // 等待作业完成
            handle.Complete();

            Debug.Log(("Use Job:" + (Time.realtimeSinceStartup - startTime) * 1000f) + "ms");

        }
        else
        {

            for (int i = 0; i < dataCount; i++)
            {
                noJobA[i] = 1;
                noJobB[i] = 2;
                noJobResult[i] = noJobA[i] + noJobB[i];
            }
            Debug.Log(("Not Use Job:" + (Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
        }
    }

    private void OnDestroy()
    {
        // 释放数组分配的内存
        a.Dispose();
        b.Dispose();
        result.Dispose();
    }
}

// 将两个浮点值相加的作业
public struct MyParallelJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<float> a;
    [ReadOnly]
    public NativeArray<float> b;
    public NativeArray<float> result;

    public void Execute(int i)
    {
        result[i] = a[i] + b[i];
    }
}