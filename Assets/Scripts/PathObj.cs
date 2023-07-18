using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public enum TimeType
{
    _1,
    _2,
    _3,
    _4
}

public class PathObj : MonoBehaviour
{
    /// <summary>
    /// 原始数据
    /// </summary>
    public float startLatitude; 
    public float startLongitude;

    public float endLatitude;
    public float endLongitude;

    public string startDateString;
    public string endDateString;

    public DateTime startDate;
    public DateTime endDate;
 
    /// <summary>
    /// 路径起始点与终点
    /// </summary>
    public Vector3 start;
    public Vector3 end;
    /// <summary>
    /// 路径所属物体的名称
    /// </summary>
    public string trackName;
    public Color color;

    public TimeType timeType;
    public Color colorByTime;

    static Color[] colorByTimeList = new Color[]
    {
        new Color(1f, 0f, 0f),       // 红色
        new Color(0f, 1f, 0f),       // 绿色
        new Color(0f, 0f, 1f),       // 蓝色
        new Color(1f, 1f, 0f),       // 黄色
    };


    public void SetInit(
        float startLatitude,
        float tartLongitude, 
        float endLatitude, 
        float endLongitude,
        string startDateString,
        string endDateString,
        Vector3 start,
        Vector3 end,
        string trackName,
        Color color)
    {
        this.startLatitude = startLatitude;
        this.startLongitude = tartLongitude;
        this.endLatitude = endLatitude;
        this.endLongitude = endLongitude;
        this.startDateString = startDateString;
        this.endDateString = endDateString;
        this.start = start;
        this.end = end;
        this.trackName = trackName;
        this.color = color;

        startDate = DateTime.ParseExact(startDateString.Substring(0, startDateString.Length - 1), "dd/MM/yyyy H:mm", null);
        endDate = DateTime.ParseExact(endDateString.Substring(0, endDateString.Length - 1), "dd/MM/yyyy H:mm", null);
        GetTimeType(startDate,ref colorByTime,ref timeType);
    }
    public void GetTimeType(DateTime startDate,ref Color colorByTime, ref TimeType timeType)
    {
        int hour = startDate.Hour;
        if (hour >= 0 && hour < 6)
        {
            timeType = TimeType._1;
            colorByTime = colorByTimeList[0];
        }
        else if (hour >= 6 && hour < 12)
        {
            timeType = TimeType._2;
            colorByTime = colorByTimeList[1];
        }
        else if (hour >= 12 && hour < 18)
        {
            timeType = TimeType._3;
            colorByTime = colorByTimeList[2];
        }
        else if (hour >= 18 && hour < 24)
        {
            timeType = TimeType._4;
            colorByTime = colorByTimeList[3];
        }
    }


}
