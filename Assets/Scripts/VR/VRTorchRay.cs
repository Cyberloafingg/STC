using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class VRTorchRay : MonoBehaviour
{
    CultureInfo englishCulture;
    public TMP_Text tipText;
    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        englishCulture = new CultureInfo("en-US");
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        RayTorch();
    }


    private void RayTorch()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, origin + direction * 100);
        //Debug.DrawLine(origin, direction * 100, Color.red);

        Ray ray = new Ray(origin, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.collider.gameObject.tag == "Path")
            {
                GameObject gameObject = hit.collider.gameObject;
                // ��ȡ���߻��е�������ռ��λ��
                Vector3 hitPosition = hit.point;
                // ��ȡԲ����������ռ�ĵײ�λ��
                Vector3 bottomPosition = gameObject.transform.position - gameObject.transform.up * (gameObject.transform.localScale.y);
                // ��ȡԲ����������ռ�Ķ���λ��
                Vector3 topPosition = gameObject.transform.position + gameObject.transform.up * (gameObject.transform.localScale.y);
                // ����ӵײ�������������
                Vector3 cylinderAxis = topPosition - bottomPosition;
                // ����ӵײ������߻��е������
                Vector3 hitVector = hitPosition - bottomPosition;
                // ͶӰ���߻��е�������Բ���������ϣ��õ���Ծ���
                float projectedDistance = Vector3.Dot(hitVector, cylinderAxis.normalized) / cylinderAxis.magnitude;
                // ����Ծ���������0��1֮��
                projectedDistance = Mathf.Clamp01(projectedDistance);
                // ����ʵ�ʵ�����Բ�������ľ��룬����Բ����ĸ߶�
                float actualDistance = projectedDistance * Vector3.Distance(topPosition, bottomPosition);
                // �������λ�ã������е������峤�ȵļ���֮����
                float relativeDistance = actualDistance / gameObject.transform.localScale.y / 2;
                //tipUI.position = Input.mousePosition + tipUIOffset;
                PathObj pathObj = gameObject.GetComponent<PathObj>();
                TimeSpan timeInterval = pathObj.endDate - pathObj.startDate;
                long relativeTicks = (long)(timeInterval.Ticks * relativeDistance);

                // �����Ӧ������
                DateTime resultDate = pathObj.startDate.AddTicks(relativeTicks);
                string text = $"{pathObj.trackName}\n{Date2String(resultDate)}";
                tipText.text = text;
            }
        }
    }

    string Date2String(DateTime date)
    {

        string formattedDate = date.ToString("dddd, MMMM dd, yyyy h:mm tt", englishCulture);
        return formattedDate;
    }
}
