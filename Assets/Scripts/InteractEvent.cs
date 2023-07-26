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
            // 获取射线击中点在世界空间的位置
            Vector3 hitPosition = hit.point;
            // 获取圆柱体在世界空间的底部位置
            Vector3 bottomPosition = gameObject.transform.position - gameObject.transform.up * (gameObject.transform.localScale.y);
            // 获取圆柱体在世界空间的顶部位置
            Vector3 topPosition = gameObject.transform.position + gameObject.transform.up * (gameObject.transform.localScale.y);
            // 计算从底部到顶部的向量
            Vector3 cylinderAxis = topPosition - bottomPosition;
            // 计算从底部到射线击中点的向量
            Vector3 hitVector = hitPosition - bottomPosition;
            // 投影射线击中点向量到圆柱体轴向上，得到相对距离
            float projectedDistance = Vector3.Dot(hitVector, cylinderAxis.normalized) / cylinderAxis.magnitude;
            // 将相对距离限制在0到1之间
            projectedDistance = Mathf.Clamp01(projectedDistance);
            // 计算实际的沿着圆柱体表面的距离，考虑圆柱体的高度
            float actualDistance = projectedDistance * Vector3.Distance(topPosition, bottomPosition);
            // 计算相对位置，即击中点在整体长度的几分之几处
            float relativeDistance = actualDistance / gameObject.transform.localScale.y / 2;
            tipUI.position = Input.mousePosition + tipUIOffset;
            PathObj pathObj= gameObject.GetComponent<PathObj>();
            TimeSpan timeInterval = pathObj.endDate - pathObj.startDate;
            long relativeTicks = (long)(timeInterval.Ticks * relativeDistance);

            // 计算对应的日期
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
