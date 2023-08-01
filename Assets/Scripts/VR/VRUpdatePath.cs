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
    /// ��ȡ���ɵ�·������
    /// </summary>
    VRGeneratePath generatePath;
    Dictionary<string, List<PathObj>> pathDic;

    public bool isColorByTimeOn = false; // �Ƿ���ʱ���ɫ
    private bool isAutoMoveOn = false; // �Ƿ��������ƶ�
    private int autoMoveTmp = 0; // �����ƶ�����ʱ����
    private int autoMoveAdd = 10; // �����ƶ�������
    //private string lastClickedTag = ""; // ��һ�ε���ı�ǩ
    //private float lastClickTime = 0f; // ��һ�ε����ʱ��
    //private float doubleClickInterval = 0.3f; // ˫�����ʱ�䣬��λΪ��
    private string attentionPathName = ""; // ��ע��·������
    private bool isAttentionOnePath = false; // �Ƿ��עĳһ��·��


    [Tooltip("����XZ����ٶ�"), SerializeField, Range(10,50)]
    int scrollXZSpeed = 15;

    [Tooltip("����Y����ٶ�"), SerializeField, Range(0.001f,0.01f)]
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
    /// ����Y��Ļ�׼λ��
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
        // ��ÿ����������һ���ٶ�
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

        // ʵ��IJobParallelForTransform�Ľṹ����Execute�����ڶ����������Ի�ȡ��Transform
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
