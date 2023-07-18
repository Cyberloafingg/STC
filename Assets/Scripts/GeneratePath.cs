using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class GeneratePath : MonoBehaviour
{
    //��ȡcsv�ļ�������һ��PathObj��List
    //����List������·��
    //��Щ·��Ӧ�ö�����ͬһ��������������
    [SerializeField]
    public STCBox STCBox;

    [SerializeField]
    TextAsset[] allCsv;

    [SerializeField]
    public Dictionary<string, List<PathObj>> pathDic;

    [SerializeField]
    Material material;


    /// <summary>
    /// ��ɫ����
    /// </summary>
    Color[] colors = new Color[]
    {
        new Color(1f, 0f, 0f),       // ��ɫ
        new Color(0f, 1f, 0f),       // ��ɫ
        new Color(0f, 0f, 1f),       // ��ɫ
        new Color(1f, 1f, 0f),       // ��ɫ
        new Color(1f, 0f, 1f),       // Ʒ��
        new Color(0f, 1f, 1f),       // ��ɫ
        new Color(1f, 0.5f, 0f),     // ��ɫ
        new Color(0.5f, 0f, 1f),     // ��ɫ
        new Color(0f, 1f, 0.5f),     // ����ɫ
        new Color(0.5f, 1f, 0f),     // �����
        new Color(0f, 0.5f, 1f),    // ����ɫ
        new Color(1f, 0f, 0.5f),    // ���ɫ
        new Color(0.5f, 1f, 1f),    // ǳ��ɫ
        new Color(1f, 0.5f, 1f),    // �Ϻ�ɫ
        new Color(0.5f, 0.5f, 1f),  // ����ɫ
        new Color(1f, 1f, 0.5f),    // ǳ��ɫ
        new Color(0.5f, 1f, 0.5f),  // �̻�ɫ
        new Color(0.5f, 0f, 0.5f),  // ����ɫ
        new Color(0.7f, 0.7f, 0.7f),// ǳ��ɫ
        new Color(0.3f, 0.3f, 0.3f),// ���ɫ
        new Color(0.8f, 0.2f, 0.2f),// ���ɫ
        new Color(0.2f, 0.8f, 0.2f),// ����ɫ
        new Color(0.2f, 0.2f, 0.8f),// ����ɫ
        new Color(0.8f, 0.8f, 0.2f)// ���ɫ
    };


    private void Awake()
    {
        pathDic = new Dictionary<string, List<PathObj>>();
    }

    void Start()
    {
        for (int i = 0; i < allCsv.Length; i++)
        {
            List<PathObj> tmpPaths = new List<PathObj>();

            string[][] csvFile = CSVReader.CSVReadAsset(allCsv[i], 1);
            //�½�һ������������ΪcsvFile
            string trackNumber = allCsv[i].name;
            GameObject childObject = new GameObject(allCsv[i].name);
            childObject.transform.parent = this.transform;
            Material originalMaterial = material;
            Material cubeMaterialInstance = Instantiate(originalMaterial);
            cubeMaterialInstance.enableInstancing = true;
            cubeMaterialInstance.color = colors[i];

            for (int j = 0; j < csvFile.Length - 2; j++)
            {
                float lat = float.Parse(csvFile[j][1]);
                float lon = float.Parse(csvFile[j][2]);
                float lat2 = float.Parse(csvFile[j + 1][1]);
                float lon2 = float.Parse(csvFile[j + 1][2]);
                string date = csvFile[j][3];
                string date2 = csvFile[j + 1][3];
                Vector3 start = STCBox.Convert(lat, lon, date);
                Vector3 end = STCBox.Convert(lat2, lon2, date2);
                GameObject cubePath =  CreateCube(
                    STCBox.Convert(lat, lon, date),
                    STCBox.Convert(lat2, lon2, date2),
                    0.2f,
                    childObject.transform
                    );
                cubePath.AddComponent<PathObj>();
                cubePath.GetComponent<PathObj>().SetInit(lat, lon, lat2, lon2, date, date2, start, end, trackNumber, colors[i]);
                cubePath.GetComponent<Renderer>().sharedMaterial = cubeMaterialInstance;
                cubePath.tag = "Path";
                /*cubePath.GetComponent<PathObj>().trackName = trackNumber;
                //cubePath.GetComponent<PathObj>().start = start;
                //cubePath.GetComponent<PathObj>().end = end;
                //cubePath.GetComponent<PathObj>().startLatitude = lat;
                //cubePath.GetComponent<PathObj>().startLongitude = lon;
                //cubePath.GetComponent<PathObj>().startDateString = date;
                //cubePath.GetComponent<PathObj>().endLatitude = lat2;
                //cubePath.GetComponent<PathObj>().endLongitude = lon2;
                //cubePath.GetComponent<PathObj>().endDateString = date2;
                //cubePath.GetComponent<PathObj>().color = Color.yellow;
                //�ڵ�ǰ������������һ������,�����ֱ���������ֱ������һ��,����Ϊ�����������
                //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //float diameter = cubePath.transform.localScale.x; // �����ֱ��
                //sphere.transform.localScale = Vector3.one * diameter;
                //sphere.transform.SetParent(cubePath.transform);
                //float spherePosY = cubePath.transform.localScale.y;
                //sphere.transform.localPosition = new Vector3(0f, spherePosY, 0f);
                //sphere.GetComponent<MeshRenderer>().material.color = Color.red;
                allPath.Add(cubePath.GetComponent<PathObj>());*/
                tmpPaths.Add(cubePath.GetComponent<PathObj>());
            }
            pathDic.Add(trackNumber, tmpPaths);
        }
        Debug.Log(pathDic.Count);
    }

    // Update is called once per frame
    void Update()
    {
    }

    GameObject CreateCylinder(Vector3 startPoint, Vector3 endPoint, float radius, Transform parent)
    {
        // ��������ĸ߶�
        float height = Vector3.Distance(startPoint, endPoint);
        // ������������ĵ�λ��
        Vector3 center = (startPoint + endPoint) / 2f;
        // �����������ת�Ƕ�
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, endPoint - startPoint);
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        // ���������λ�á���ת�ʹ�С
        cylinder.transform.position = center;
        cylinder.transform.rotation = rotation;
        cylinder.transform.localScale = new Vector3(radius, height / 2f, radius);
        cylinder.transform.parent = parent;
        return cylinder;
    }

    GameObject CreateCube(Vector3 startPoint, Vector3 endPoint, float size, Transform parent)
    {
        // ���㳤����ĳ���
        float length = Vector3.Distance(startPoint, endPoint);
        // ���㳤��������ĵ�λ��
        Vector3 center = (startPoint + endPoint) / 2f;
        // ���㳤�������ת�Ƕ�
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, endPoint - startPoint);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // ���ó������λ�á���ת�ʹ�С
        cube.transform.position = center;
        cube.transform.rotation = rotation;
        cube.transform.localScale = new Vector3(size, length, size);
        cube.transform.parent = parent;
        return cube;
    }

}
