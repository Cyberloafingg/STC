using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

public class STCBox : MonoBehaviour
{
    public float oriLatitude; // 原始纬度
    public float oriLongitude;// 原始经度
    public string oriDateTimeString;  // 原始时间
    [SerializeField]
    public DateTime oriDate;   // 原始时间的秒数
    public DateTime nowDate;   // 当前时间的秒数

    CultureInfo englishCulture = new CultureInfo("en-US");

    public Material vrMapMaterial;

    /// <summary>
    /// x轴缩放比例
    /// </summary>
    [SerializeField]
    public float xScale = 1000.0f;
    [SerializeField]
    public float yScale = 1000.0f;
    [SerializeField]
    public float zScale = 0.01f;

    public static STCBox instance;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
            oriDate = DateTime.ParseExact(oriDateTimeString, "dd/MM/yyyy H:mm", null);
        }
    }


    private void Start()
    {
        oriDate = DateTime.ParseExact(oriDateTimeString, "dd/MM/yyyy H:mm", null);
        Debug.Log(oriDate);
        nowDate = oriDate;
        xScale *= transform.localScale.x;
        yScale *= transform.localScale.y;
        zScale *= transform.localScale.z;
        STCBox.instance.vrMapMaterial.mainTextureScale = new Vector2(1.0f, 1.0f);
        STCBox.instance.vrMapMaterial.mainTextureOffset = new Vector2(0.0f, 0.0f);
    }

    /// <summary>
    /// 输入经纬度，以及时间信息，返回世界坐标
    /// </summary>
    /// <param name="lat"></param>
    /// <param name="lon"></param>
    /// <param name="date"></param>
    public Vector3 Convert(float lat, float lon, string dateTimeString)
    {
        float x = (lat - oriLatitude) * xScale;
        float z = (lon - oriLongitude) * zScale;
        float y = Data2Second(dateTimeString) * yScale;
        return new Vector3(x, y, z);
    }



    float Data2Second(string dateTimeString)
    {
        DateTime dateTime = DateTime.ParseExact(dateTimeString, "dd/MM/yyyy H:mm", null);
        float minute = (float)(oriDate - dateTime).TotalMinutes;
        return minute;
    }

    public DateTime DateString2Date(string dataString)
    {
        return DateTime.ParseExact(dataString, "dd/MM/yyyy H:mm", null);
    }

    public float DateString2Minute(string dataString)
    {
        DateTime dateTime = DateString2Date(dataString);
        return (float)(oriDate - dateTime).TotalMinutes;
    }

    public float Date2Minute(DateTime dateTime)
    {
        return (float)(oriDate - dateTime).TotalMinutes;
    }

    public Vector3 OriData2Vector3(string lat, string lon, string dateString)
    {
        float x = float.Parse(lat);
        float z = float.Parse(lon);
        float y = DateString2Minute(dateString);
        return new Vector3(x, y, z);
    }

}
