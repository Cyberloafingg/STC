using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using UnityEngine;

public class UpdatePath : MonoBehaviour
{
    GeneratePath generatePath;
    STCBox STCBox;
    [SerializeField]
    Dictionary<string, List<PathObj>> pathDic;

    bool isColorByTimeOn = false;



    private string lastClickedTag = "";
    private float lastClickTime = 0f;
    private float doubleClickInterval = 0.3f; // 双击间隔时间，单位为秒
    private string attentionPathName = ""; // 关注的路径名称
    bool isAttentionOnePath = false; // 是否关注某一条路径
    // Start is called before the first frame update
    void Start()
    {
        generatePath = GetComponent<GeneratePath>();
        pathDic = generatePath.pathDic;
        STCBox = generatePath.STCBox;
    }

    // Update is called once per frame
    void Update()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {

            //changeXZScale(scrollDelta * 10);
            changeYScale(scrollDelta * 0.0001f);
            UpdateEveryPath();
        }

        ChangeColor();

        if(Input.GetKeyDown(KeyCode.L))
        {
            ColorByTime();
        }
        ScrollByYAxis();
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateEveryPath()
    {
        foreach (KeyValuePair<string, List<PathObj>> kvp in pathDic)
        {
            List<PathObj> paths = kvp.Value;
            for (int i = 0; i < paths.Count; i++)
            {
                paths[i].start = STCBox.Convert(paths[i].startLatitude, paths[i].startLongitude, paths[i].startDateString);
                paths[i].end = STCBox.Convert(paths[i].endLatitude, paths[i].endLongitude, paths[i].endDateString);
                float height = Vector3.Distance(paths[i].start, paths[i].end);
                Vector3 center = (paths[i].start + paths[i].end) / 2f;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, paths[i].end - paths[i].start);
                paths[i].transform.position = center;
                paths[i].transform.rotation = rotation;
                paths[i].transform.localScale = new Vector3(0.2f, height, 0.2f);
            }
        }
    }




    void changeYScale(float value)
    {
        STCBox.yScale += value;
    }


    void ChangeColor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                string hitTag = hitObject.tag;
                

                // 判断是否为双击
                if (Time.time - lastClickTime < doubleClickInterval && hitTag == "Path" && lastClickedTag == hitTag)
                {
                    string hitName = hitObject.GetComponent<PathObj>().trackName;
                    // 执行双击操作
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
                else
                {
                    // 记录上一次点击的信息
                    lastClickedTag = hitTag;
                    lastClickTime = Time.time;
                }
            }
        }
    }

    void ColorByTime()
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

    /// <summary>
    /// 调整Y轴的基准位置
    /// </summary>
    void ScrollByYAxis()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            STCBox.oriDate = STCBox.oriDate.AddHours(1);
            UpdateEveryPath();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            STCBox.oriDate = STCBox.oriDate.AddHours(-1);
            UpdateEveryPath();
        }
    }



}
