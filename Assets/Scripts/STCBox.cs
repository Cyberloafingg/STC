using System;
using UnityEngine;

public class STCBox : MonoBehaviour
{
    public float oriLatitude; // ԭʼγ��
    public float oriLongitude;// ԭʼ����
    public string oriDateTimeString;  // ԭʼʱ��
    [SerializeField]
    public DateTime oriDate;   // ԭʼʱ�������

    /// <summary>
    /// x�����ű���
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
    /// ���뾭γ�ȣ��Լ�ʱ����Ϣ��������������
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
