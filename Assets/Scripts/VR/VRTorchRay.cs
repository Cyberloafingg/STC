using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using Valve.VR;

public class VRTorchRay : MonoBehaviour
{
    public SteamVR_Input_Sources rightInputSource = SteamVR_Input_Sources.RightHand; // �ֱ�����Դ
    public SteamVR_Action_Boolean gripAction = SteamVR_Actions.default_GrabGrip; // �ֱ�ץȡ���붯��
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
            //ֻҪ�����ֱ�ץȡ��������ʾ���ߺ���ʾUI
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
