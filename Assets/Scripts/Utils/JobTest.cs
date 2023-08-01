using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class JobTest : MonoBehaviour
{
    public bool useJob;
    public int dataCount = 100;
    //public int batchCount;
    // 用于存储transform的NativeArray
    private TransformAccessArray m_TransformsAccessArray;
    private NativeArray<Vector3> m_Velocities;

    private PositionUpdateJob m_Job;
    private JobHandle m_PositionJobHandle;
    private GameObject[] sphereGameObjects;
    [BurstCompile]
    struct PositionUpdateJob : IJobParallelForTransform
    {
        // 给每个物体设置一个速度
        [ReadOnly]
        public NativeArray<Vector3> velocity;

        public float deltaTime;

        // 实现IJobParallelForTransform的结构体中Execute方法第二个参数可以获取到Transform
        public void Execute(int i, TransformAccess transform)
        {
            transform.position += velocity[i] * deltaTime;
        }
    }

    void Start()
    {
        m_Velocities = new NativeArray<Vector3>(dataCount, Allocator.Persistent);

        // 用代码生成一个球体,作为复制的模板
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // 关闭阴影
        var renderer = sphere.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        // 关闭碰撞体
        var collider = sphere.GetComponent<Collider>();
        collider.enabled = false;

        // 保存transform的数组,用于生成transform的Native Array
        var transforms = new Transform[dataCount];
        sphereGameObjects = new GameObject[dataCount];
        int row = (int)Mathf.Sqrt(dataCount);
        // 生成1W个球
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < row; j++)
            {
                var go = GameObject.Instantiate(sphere);
                go.transform.position = new Vector3(j, 0, i);
                sphereGameObjects[i * row + j] = go;
                transforms[i * row + j] = go.transform;
                m_Velocities[i * row + j] = new Vector3(0.1f * j, 0, 0.1f * j);
            }
        }

        m_TransformsAccessArray = new TransformAccessArray(transforms);
    }

    void Update()
    {
        //float startTime = Time.realtimeSinceStartup;
        if (useJob)
        {
            // 实例化一个job,传入数据
            m_Job = new PositionUpdateJob()
            {
                deltaTime = Time.deltaTime,
                velocity = m_Velocities,
            };

            // 调度job执行
            m_PositionJobHandle = m_Job.Schedule(m_TransformsAccessArray);
            //Debug.Log(("Use Job:"+ (Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
        }
        else
        {
            for (int i = 0; i < dataCount; ++i)
            {
                sphereGameObjects[i].transform.position += m_Velocities[i] * Time.deltaTime;
            }
            //Debug.Log(("Not Use Job:"+ (Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
        }

    }

    // 保证当前帧内Job执行完毕
    private void LateUpdate()
    {
        m_PositionJobHandle.Complete();
    }

    // OnDestroy中释放NativeArray的内存
    private void OnDestroy()
    {
        m_Velocities.Dispose();
        m_TransformsAccessArray.Dispose();
    }
}
