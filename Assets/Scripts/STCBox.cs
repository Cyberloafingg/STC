using System;
using UnityEngine;

public class STCBox : MonoBehaviour
{
    public float oriLatitude; // 原始纬度
    public float oriLongitude;// 原始经度
    public string oriDateTimeString;  // 原始时间
    [SerializeField]
    public DateTime oriDate;   // 原始时间的秒数

    /// <summary>
    /// x轴缩放比例
    /// </summary>
    [SerializeField]
    public float xScale = 1000.0f;
    [SerializeField]
    public float yScale = 1000.0f;
    [SerializeField]
    public float zScale = 0.01f;


    private void Start()
    {
        oriDate = DateTime.ParseExact(oriDateTimeString, "dd/MM/yyyy H:mm", null);
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
        DateTime dateTime = DateTime.ParseExact(dateTimeString.Substring(0,dateTimeString.Length-1), "dd/MM/yyyy H:mm", null);
        float minute = (float)(dateTime - oriDate).TotalMinutes;
        //Debug.Log("minutes: " + minute);
        return minute;
    }

}
