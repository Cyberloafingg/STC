using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using Valve.VR;

public class VRTorchRay : MonoBehaviour
{
    public SteamVR_Input_Sources rightInputSource = SteamVR_Input_Sources.RightHand; // 手柄输入源
    public SteamVR_Action_Boolean gripAction = SteamVR_Actions.default_GrabGrip; // 手柄抓取输入动作
    CultureInfo englishCulture;
    TMP_Text tipText;
    private LineRenderer lineRenderer;
    public CanvasGroup canvasGroup;
    public SteamVR_Action_Boolean upAction = SteamVR_Actions.default_Teleport;

    // Start is called before the first frame update
    void Start()
    {
        tipText = canvasGroup.GetComponentInChildren<TMP_Text>();
        englishCulture = new CultureInfo("en-US");
        lineRenderer = GetComponent<LineRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        canvasGroup.alpha = 0;
        lineRenderer.enabled = false;
        RayTorch();
    }


    private void RayTorch()
    {
        if(gripAction.GetState(rightInputSource) && !gripAction.GetState(SteamVR_Input_Sources.LeftHand))
        {
            //只要按下手柄抓取键，就显示射线和提示UI
            canvasGroup.alpha = 1;
            lineRenderer.enabled = true;
            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * 100);
            Ray ray = new Ray(origin, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.tag == "Path")
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
                    //tipUI.position = Input.mousePosition + tipUIOffset;
                    PathObj pathObj = gameObject.GetComponent<PathObj>();
                    TimeSpan timeInterval = pathObj.endDate - pathObj.startDate;
                    long relativeTicks = (long)(timeInterval.Ticks * relativeDistance);

                    // 计算对应的日期
                    DateTime resultDate = pathObj.startDate.AddTicks(relativeTicks);
                    string text = $"{pathObj.trackName}\n{Date2String(resultDate)}";
                    tipText.text = text;
                    if (upAction.GetStateDown(rightInputSource))
                    {
                        VRUpdatePath.instance.ChangeColor(gameObject);
                    }
                }

                if (hit.collider.gameObject.name == "Map" && VRSettingPanel.instance.isMarkering)
                {
                    Vector3 hitPosition = hit.point;
                    if(upAction.GetStateDown(rightInputSource))
                    {
                        tipText.text = "TODO USE API";
                        VRSettingPanel.instance.Marking(hitPosition);
                    }
                }

                if (hit.collider.gameObject.tag == "Marker" && VRSettingPanel.instance.isDeleteMarker)
                {
                    if (upAction.GetStateDown(rightInputSource))
                    {
                        tipText.text = "TODO USE API";
                        VRSettingPanel.instance.DeleteMarker(hit.collider.gameObject);
                    }
                }
            }


        }
    }

    string Date2String(DateTime date)
    {

        string formattedDate = date.ToString("dddd, MMMM dd, yyyy h:mm tt", englishCulture);
        return formattedDate;
    }


}
