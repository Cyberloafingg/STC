using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;

public class VRUpdatePath : MonoBehaviour
{
    private JobHandle m_JobHandle;
    private PathUpdateJob m_Job;

    /// <summary>
    /// 获取生成的路径数据
    /// </summary>
    VRGeneratePath generatePath;
    Dictionary<string, List<PathObj>> pathDic;

    public bool isColorByTimeOn = false; // 是否按照时间变色
    private bool isAutoMoveOn = false; // 是否开启自由移动
    private int autoMoveTmp = 0; // 自由移动的临时变量
    private int autoMoveAdd = 10; // 自由移动的增量
    //private string lastClickedTag = ""; // 上一次点击的标签
    //private float lastClickTime = 0f; // 上一次点击的时间
    //private float doubleClickInterval = 0.3f; // 双击间隔时间，单位为秒
    private string attentionPathName = ""; // 关注的路径名称
    private bool isAttentionOnePath = false; // 是否关注某一条路径


    [Tooltip("缩放XZ轴的速度"), SerializeField, Range(10,50)]
    int scrollXZSpeed = 15;

    [Tooltip("缩放Y轴的速度"), SerializeField, Range(0.001f,0.01f)]
    float scrollYSpeed = 0.001f;

    public static VRUpdatePath instance;


    // Start is called before the first frame update
    void Start()
    {
        generatePath = GetComponent<VRGeneratePath>();
        pathDic = generatePath.pathDic;
        Physics.autoSyncTransforms = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            ColorByTime();
        }
        if(isAutoMoveOn)
        {
            AutoMove();
        }
    }

    private void Awake()
    {
        instance = this;
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ScrollByXZAxis()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            STCBox.instance.xScale += scrollDelta * scrollXZSpeed;
            STCBox.instance.zScale += scrollDelta * scrollXZSpeed;
        }
        UpdateEveryPath();
    }

    /// <summary>
    /// 调整Y轴的基准位置
    /// </summary>
    public void ScrollByYAxis()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            STCBox.instance.yScale += scrollDelta * scrollYSpeed;
        }
        UpdateEveryPath();
    }

    private void FixedUpdate()
    {
        Physics.autoSyncTransforms = false;
    }

    public void AutoMove()
    {
        autoMoveTmp += 10;
        if (autoMoveTmp % 6000 == 0)
        {
            autoMoveAdd = -autoMoveAdd;
        }
        STCBox.instance.nowDate = STCBox.instance.nowDate.AddMinutes(-autoMoveAdd);
        UpdateEveryPath();
    }

    public void AutoMoveOnOrOff()
    {
        if(isAutoMoveOn)
        {
            isAutoMoveOn = false;
        }
        else
        {
            isAutoMoveOn = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateEveryPath()
    {
        /*foreach (KeyValuePair<string, List<PathObj>> kvp in pathDic)
        //{
        //    List<PathObj> paths = kvp.Value;
        //    for (int i = 0; i < paths.Count; i++)
        //    {
        //        paths[i].start = STCBox.instance.Convert(paths[i].startLatitude, paths[i].startLongitude, paths[i].startDateString);
        //        paths[i].end = STCBox.instance.Convert(paths[i].endLatitude, paths[i].endLongitude, paths[i].endDateString);
        //        float height = Vector3.Distance(paths[i].start, paths[i].end);
        //        Vector3 center = (paths[i].start + paths[i].end) / 2f;
        //        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, paths[i].end - paths[i].start);
        //        paths[i].transform.position = center;
        //        paths[i].transform.rotation = rotation;
        //        paths[i].transform.localScale = new Vector3(0.2f, height, 0.2f);
        //    }
        }*/

        m_Job = new PathUpdateJob()
        {
            startPos = generatePath.startPos,
            endPos = generatePath.endPos,
            scaleVector = new Vector3(STCBox.instance.xScale, STCBox.instance.yScale, STCBox.instance.zScale),
            oriVector = new Vector3(
                STCBox.instance.oriLatitude,
                STCBox.instance.Date2Minute(STCBox.instance.nowDate),
                STCBox.instance.oriLongitude
            ),
            xscale = STCBox.instance.transform.localScale.x
        };
        m_JobHandle = m_Job.Schedule(generatePath.transformsAccessArray);
    }

    [BurstCompile]
    struct PathUpdateJob : IJobParallelForTransform
    {
        // 给每个物体设置一个速度
        [ReadOnly]
        public NativeArray<Vector3> startPos;
        [ReadOnly]
        public NativeArray<Vector3> endPos;
        [ReadOnly]
        public Vector3 scaleVector;
        [ReadOnly]
        public Vector3 oriVector;
        [ReadOnly]
        public float xscale;


        public static Vector3 ElementWiseMultiply(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        // 实现IJobParallelForTransform的结构体中Execute方法第二个参数可以获取到Transform
        public void Execute(int i, TransformAccess transform)
        {
            Vector3 startPosNew = ElementWiseMultiply((startPos[i] - oriVector),scaleVector);
            Vector3 endPosNew = ElementWiseMultiply((endPos[i] - oriVector), scaleVector);
            float height = Vector3.Distance(startPosNew, endPosNew);
            Vector3 center = (startPosNew + endPosNew) / 2f;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, endPosNew - startPosNew);
            transform.localPosition = center;
            transform.localRotation = rotation;
            transform.localScale = new Vector3(0.2f * xscale, height/2f, 0.2f * xscale);
        }
    }


    public void ChangeColor(GameObject hitObject)
    {
        string hitName = hitObject.GetComponent<PathObj>().trackName;
        if (!isAttentionOnePath)
        {

            attentionPathName = hitName;
            isAttentionOnePath = true;
            Debug.Log("Double click on object: " + hitName);
            foreach (KeyValuePair<string, List<PathObj>> kvp in pathDic)
            {
                string trackNumber = kvp.Key;
                List<PathObj> paths = kvp.Value;
                Color color = Color.gray;
                color.a = 0.1f;
                for (int i = 0; i < paths.Count; i++)
                {
                    if (trackNumber != hitName)
                    {
                        paths[i].GetComponent<Renderer>().material.color = color;
                    }

                }
            }
        }
        else
        {
            isAttentionOnePath = false;
            attentionPathName = "";
            Debug.Log("Double click on object: " + hitName);
            foreach (KeyValuePair<string, List<PathObj>> kvp in pathDic)
            {
                string trackNumber = kvp.Key;
                List<PathObj> paths = kvp.Value;
                for (int i = 0; i < paths.Count; i++)
                {
                    if (trackNumber != hitName)
                    {
                        if (isColorByTimeOn)
                        {
                            paths[i].GetComponent<Renderer>().material.color = paths[i].colorByTime;
                        }
                        else
                        {
                            paths[i].GetComponent<Renderer>().material.color = paths[i].color;
                        }
                    }
                }
            }   
        }
    }

    public void ColorByTime()
    {
        if (!isColorByTimeOn)
        {
            isColorByTimeOn = true;   
            if(isAttentionOnePath)
            {
                List<PathObj> paths = pathDic[attentionPathName];
                Debug.Log(paths[0].trackName);
                for (int i = 0; i < paths.Count; i++)
                {
                    paths[i].GetComponent<Renderer>().material.color = paths[i].colorByTime;
                }
            }
            else
            {
                foreach (KeyValuePair<string, List<PathObj>> kvp in pathDic)
                {
                    List<PathObj> paths = kvp.Value;
                    for (int i = 0; i < paths.Count; i++)
                    {
                        paths[i].GetComponent<Renderer>().material.color = paths[i].colorByTime;
                    }
                }
            }
        }
        else
        {
            isColorByTimeOn = false;
            if (isAttentionOnePath)
            {
                List<PathObj> paths = pathDic[attentionPathName];
                Debug.Log(paths[0].trackName);
                for (int i = 0; i < paths.Count; i++)
                {
                    paths[i].GetComponent<Renderer>().material.color = paths[i].color;
                }
            }
            else
            {
                foreach (KeyValuePair<string, List<PathObj>> kvp in pathDic)
                {
                    List<PathObj> paths = kvp.Value;
                    for (int i = 0; i < paths.Count; i++)
                    {
                        paths[i].GetComponent<Renderer>().material.color = paths[i].color;
                    }
                }
            }
            
        }
    }


    private void LateUpdate()
    {
        m_JobHandle.Complete();
    }


}
