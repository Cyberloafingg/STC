using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.IO;
using Unity.Collections;
using Unity.Core;
using UnityEngine.Jobs;
using Valve.VR.InteractionSystem;

public class VRGeneratePath : MonoBehaviour
{


    /// <summary>
    /// 生成的路径所使用的材质
    /// </summary>
    [Tooltip("生成的路径所使用的材质")]
    [SerializeField]
    Material material;

    [Tooltip("所打开的路径")]
    [SerializeField]
    string pathName;

    /// <summary>
    /// ECS部分的数据
    /// </summary>
    [ReadOnly]
    public NativeArray<Vector3> startPos;
    [ReadOnly]
    public NativeArray<Vector3> endPos;
    public TransformAccessArray transformsAccessArray;

    /// <summary>
    /// 一些记录数据
    /// </summary>
    private List<List<string>> nameList = new List<List<string>>();
    private List<Vector3> startPosTempList = new List<Vector3>();
    private List<Vector3> endPosTempList = new List<Vector3>();
    private List<string> startDate = new List<string>();
    private List<string> endDate = new List<string>();
    int dataCount = 0;

    /// <summary>
    /// 存储所有的路径，按照名称存储
    /// </summary>
    [SerializeField]
    public Dictionary<string, List<PathObj>> pathDic;

    /// <summary>
    /// 颜色配置
    /// </summary>
    Color[] colors = new Color[]
    {
        new Color(1f, 0f, 0f),       // 红色
        new Color(0f, 1f, 0f),       // 绿色
        new Color(0f, 0f, 1f),       // 蓝色
        new Color(1f, 1f, 0f),       // 黄色
        new Color(1f, 0f, 1f),       // 品红
        new Color(0f, 1f, 1f),       // 青色
        new Color(1f, 0.5f, 0f),     // 橙色
        new Color(0.5f, 0f, 1f),     // 紫色
        new Color(0f, 1f, 0.5f),     // 青绿色
        new Color(0.5f, 1f, 0f),     // 橄榄绿
        new Color(0f, 0.5f, 1f),    // 天蓝色
        new Color(1f, 0f, 0.5f),    // 深红色
        new Color(0.5f, 1f, 1f),    // 浅青色
        new Color(1f, 0.5f, 1f),    // 紫红色
        new Color(0.5f, 0.5f, 1f),  // 灰蓝色
        new Color(1f, 1f, 0.5f),    // 浅黄色
        new Color(0.5f, 1f, 0.5f),  // 绿灰色
        new Color(0.5f, 0f, 0.5f),  // 深紫色
        new Color(0.7f, 0.7f, 0.7f),// 浅灰色
        new Color(0.3f, 0.3f, 0.3f),// 深灰色
        new Color(0.8f, 0.2f, 0.2f),// 深红色
        new Color(0.2f, 0.8f, 0.2f),// 深绿色
        new Color(0.2f, 0.2f, 0.8f),// 深蓝色
        new Color(0.8f, 0.8f, 0.2f)// 深黄色
    };


    private void Awake()
    {
        pathDic = new Dictionary<string, List<PathObj>>();
        
    }

    void Start()
    {
        ProcessCSVFiles(pathName);
        int idx = 0;
        var transforms = new Transform[dataCount];
        for(int i = 0; i < nameList.Count; i++)
        {
            List<PathObj> tmpPaths = new List<PathObj>();
            string trackNumber = nameList[i][0];
            GameObject childObject = new GameObject(nameList[i][0]);
            childObject.transform.parent = this.transform;
            Material originalMaterial = material;
            Material cubeMaterialInstance = Instantiate(originalMaterial);
            cubeMaterialInstance.enableInstancing = true;
            cubeMaterialInstance.color = colors[i];
            for(int j = 0; j < nameList[i].Count; j++)
            {
                Vector3 start = startPosTempList[idx];
                Vector3 end = endPosTempList[idx];
                GameObject cubePath = CreateCylinder(
                    STCBox.instance.Convert(start.x,start.z, startDate[idx]),
                    STCBox.instance.Convert(end.x, end.z, endDate[idx]), 
                    0.2f * STCBox.instance.transform.localScale.x, 
                    childObject.transform
                );
                cubePath.AddComponent<PathObj>();
                cubePath.GetComponent<PathObj>().SetInit(
                    startPos[idx].x, startPos[idx].z,
                    endPos[idx].x, endPos[idx].z, 
                    startDate[idx], endDate[idx],
                    start, end, trackNumber, colors[i]
                );
                cubePath.GetComponent<Renderer>().sharedMaterial = cubeMaterialInstance;
                cubePath.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cubePath.GetComponent<MeshRenderer>().receiveShadows = false;
                cubePath.GetComponent<Collider>().isTrigger = true;
                tmpPaths.Add(cubePath.GetComponent<PathObj>());
                cubePath.tag = "Path";
                //Destroy(cubePath.GetComponent<Collider>());
                transforms[idx] = cubePath.transform;
                idx++;
            }
            pathDic.Add(trackNumber, tmpPaths);
        }
        transformsAccessArray = new TransformAccessArray(transforms);
    }

    /// <summary>
    /// 生成圆柱体路径
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="radius"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    GameObject CreateCylinder(Vector3 startPoint, Vector3 endPoint, float radius, Transform parent)
    {
        // 计算柱体的高度
        float height = Vector3.Distance(startPoint, endPoint);
        // 计算柱体的中心点位置
        Vector3 center = (startPoint + endPoint) / 2f;
        // 计算柱体的旋转角度
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, endPoint - startPoint);
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        // 设置柱体的位置、旋转和大小
        cylinder.transform.position = center;
        cylinder.transform.rotation = rotation;
        cylinder.transform.localScale = new Vector3(radius, height / 2f, radius);
        cylinder.transform.parent = parent;
        return cylinder;
    }

    /// <summary>
    /// 生成立方体路径
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="size"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    GameObject CreateCube(Vector3 startPoint, Vector3 endPoint, float size, Transform parent)
    {
        // 计算长方体的长度
        float length = Vector3.Distance(startPoint, endPoint);
        // 计算长方体的中心点位置
        Vector3 center = (startPoint + endPoint) / 2f;
        // 计算长方体的旋转角度
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, endPoint - startPoint);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // 设置长方体的位置、旋转和大小
        cube.transform.position = center;
        cube.transform.rotation = rotation;
        cube.transform.localScale = new Vector3(size, length, size);
        cube.transform.parent = parent;
        return cube;
    }

    /// <summary>
    /// 解析CSV
    /// </summary>
    /// <param name="csvFolderPath"></param>
    public void ProcessCSVFiles(string csvFolderPath)
    {
        csvFolderPath = PlayerPrefs.GetString("SelectedFolderPath");
        if (csvFolderPath == null) {
            csvFolderPath = "Assets/Resources/data";
        }
        string[] csvFiles = Directory.GetFiles(csvFolderPath, "*.csv");

        float totalLatitude = 0f;
        float totalLongitude = 0f;
        STCBox.instance.oriDateTimeString = "12/02/2013 00:00"; 
        //STCBox.instance.oriDate = DateTime.ParseExact(STCBox.instance.oriDateTimeString, "MM/dd/yyyy H:mm");
        foreach (string filePath in csvFiles)
        {
            string[] lines = File.ReadAllLines(filePath);
            List<string> tmpNameList = new List<string>();
            string tkName = Path.GetFileNameWithoutExtension(filePath);
            for (int i = 1; i < lines.Length - 2; i++) // 跳过标题行
            {
                string[] startValues = lines[i].Split(',');
                string[] endValues = lines[i+1].Split(',');
                
                if (startValues.Length >= 3)
                {
                    if (float.TryParse(startValues[1], out float latitude) && 
                        float.TryParse(startValues[2], out float longitude))
                    {
                        startPosTempList.Add(STCBox.instance.OriData2Vector3(startValues[1], startValues[2], startValues[3]));
                        endPosTempList.Add(STCBox.instance.OriData2Vector3(endValues[1], endValues[2], endValues[3]));
                        startDate.Add(startValues[3]);
                        endDate.Add(endValues[3]);
                        tmpNameList.Add(tkName);
                        totalLatitude += latitude;
                        totalLongitude += longitude;
                        dataCount++;
                    }
                }
            }
            nameList.Add(tmpNameList);
        }
        startPos =  new NativeArray<Vector3>(startPosTempList.ToArray(), Allocator.Persistent);
        endPos = new NativeArray<Vector3>(endPosTempList.ToArray(), Allocator.Persistent);
        float averageLatitude = 0;
        float averageLongitude = 0;
        if (dataCount > 0)
        {
            averageLatitude = totalLatitude / dataCount;
            averageLongitude = totalLongitude / dataCount;

            Debug.Log("Average Latitude: " + averageLatitude);
            Debug.Log("Average Longitude: " + averageLongitude);
            Debug.Log("All data count: " + dataCount + ",Length of pos: " + startPos.Length);
            STCBox.instance.oriLatitude = averageLatitude;
            STCBox.instance.oriLongitude = averageLongitude;
        }
        else
        {
            Debug.Log("No valid data found in CSV files.");
        }

    }

    private void OnDestroy()
    {
        transformsAccessArray.Dispose();
    }

}
