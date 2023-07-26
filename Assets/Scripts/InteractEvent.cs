using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Globalization;

public class InteractEvent : MonoBehaviour
{
    public RectTransform tipUI;
    public Vector3 tipUIOffset;
    public int rayLength;
    CultureInfo englishCulture;
    Image image;
    TMP_Text tipText;

    void Start()
    {
        image = tipUI.gameObject.GetComponent<Image>();
        tipText = image.gameObject.GetComponentInChildren<TMP_Text>();
        image.enabled = false;
        tipText.enabled = false;
        englishCulture = new CultureInfo("en-US");
    }


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool raycast = Physics.Raycast(ray, out hit, rayLength);
        if(raycast && hit.collider.gameObject.tag == "Path")
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
            tipUI.position = Input.mousePosition + tipUIOffset;
            PathObj pathObj= gameObject.GetComponent<PathObj>();
            TimeSpan timeInterval = pathObj.endDate - pathObj.startDate;
            long relativeTicks = (long)(timeInterval.Ticks * relativeDistance);

            // �����Ӧ������
            DateTime resultDate = pathObj.startDate.AddTicks(relativeTicks);
            string text = $"{pathObj.trackName}\n{Date2String(resultDate)}";
            tipText.text = text;
            image.enabled = true;
            tipText.enabled = true;
        }
        else
        {
            image.enabled = false;
            tipText.enabled = false;
        }        
    }
    string Date2String(DateTime date)
    {

        string formattedDate = date.ToString("dddd, MMMM dd, yyyy h:mm tt", englishCulture);
        return formattedDate;
    }
}
