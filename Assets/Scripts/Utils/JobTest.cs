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
    // ���ڴ洢transform��NativeArray
    private TransformAccessArray m_TransformsAccessArray;
    private NativeArray<Vector3> m_Velocities;

    private PositionUpdateJob m_Job;
    private JobHandle m_PositionJobHandle;
    private GameObject[] sphereGameObjects;
    [BurstCompile]
    struct PositionUpdateJob : IJobParallelForTransform
    {
        // ��ÿ����������һ���ٶ�
        [ReadOnly]
        public NativeArray<Vector3> velocity;

        public float deltaTime;

        // ʵ��IJobParallelForTransform�Ľṹ����Execute�����ڶ����������Ի�ȡ��Transform
        public void Execute(int i, TransformAccess transform)
        {
            transform.position += velocity[i] * deltaTime;
        }
    }

    void Start()
    {
        m_Velocities = new NativeArray<Vector3>(dataCount, Allocator.Persistent);

        // �ô�������һ������,��Ϊ���Ƶ�ģ��
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // �ر���Ӱ
        var renderer = sphere.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        // �ر���ײ��
        var collider = sphere.GetComponent<Collider>();
        collider.enabled = false;

        // ����transform������,��������transform��Native Array
        var transforms = new Transform[dataCount];
        sphereGameObjects = new GameObject[dataCount];
        int row = (int)Mathf.Sqrt(dataCount);
        // ����1W����
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
            // ʵ����һ��job,��������
            m_Job = new PositionUpdateJob()
            {
                deltaTime = Time.deltaTime,
                velocity = m_Velocities,
            };

            // ����jobִ��
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

    // ��֤��ǰ֡��Jobִ�����
    private void LateUpdate()
    {
        m_PositionJobHandle.Complete();
    }

    // OnDestroy���ͷ�NativeArray���ڴ�
    private void OnDestroy()
    {
        m_Velocities.Dispose();
        m_TransformsAccessArray.Dispose();
    }
}
