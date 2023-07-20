using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;
using Unity.VisualScripting;
using System.Reflection;
using Unity.Burst.CompilerServices;
//ϸ�ڵ���ͷ�쳤ʱ

public class VisualLabel : MonoBehaviour
{
    CultureInfo englishCulture;

    //��ȡ�������е�����canvas������tagΪlabelcanvas
    /// <summary>
    /// ���е�canvasLabel
    /// </summary>
    List<Canvas> canvasLabelList = new List<Canvas>();

    /// <summary>
    /// ���е�canvasPointer
    /// </summary>
    List<Canvas> canvasPointerList = new List<Canvas>();

    List<TMP_Text> pointerList = new List<TMP_Text>();

    /// <summary>
    /// ���е�Label
    /// </summary>
    List<List<TMP_Text>> labelTextList = new List<List<TMP_Text>>();

    public GameObject[] allPlane;

    public UpdatePath UpdatePath;

    public Transform allPathTransform;

    public Transform mapTransform;

    private Plane dragPlane;

    public float moveXZSpeed = 0.5f;

    Vector3 offset;

    float dif = 5.0f;


    // ������Ҫ���Ƶı���
    public int controlledVariable = 0;

    // ��¼��һ֡����λ��
    private Vector3 lastMousePosition;

    // Start is called before the first frame update
    void Start()
    {

        dragPlane = new Plane(mapTransform.up, mapTransform.position);
        Canvas[] group = GetComponentsInChildren<Canvas>();
        for (int i = 0; i < group.Length; i++)
        {
            if (group[i].gameObject.name != "CanvasPointer")
            {
                canvasLabelList.Add(group[i]);
            }
            if (group[i].gameObject.name == "CanvasPointer")
            {
                canvasPointerList.Add(group[i]);//4
                pointerList.Add(group[i].GetComponentInChildren<TMP_Text>());
            }
        }
        for(int i = 0; i < canvasLabelList.Count; i++)
        {
            List<TMP_Text> tmp = new List<TMP_Text>();
            canvasLabelList[i].GetComponentsInChildren<TMP_Text>(tmp);
            labelTextList.Add(tmp);
        }
        englishCulture =  new CultureInfo("en-US");
    }

    // Update is called once per frame

    void Update()
    {
        DisableBackFacingToCamera();

        UpdateLabel();

        PointerMove();
    }


    void DisableBackFacingToCamera()
    {
        float cameraYAngle = Camera.main.transform.rotation.eulerAngles.y;
        cameraYAngle = (cameraYAngle + 3600) % 360;
        if ((cameraYAngle <= 360 && cameraYAngle >= 315) || (cameraYAngle < 45 && cameraYAngle >= 0))
        {
            CheckIsRender(0);
        }
        else if (cameraYAngle < 135 && cameraYAngle >= 45)
        {
            CheckIsRender(1);
        }
        else if (cameraYAngle < 225 && cameraYAngle >= 135)
        {
            CheckIsRender(2);
        }
        else
        {
            CheckIsRender(3);
        }
        void CheckIsRender(int idx)
        {
            if (allPlane[idx].GetComponent<SetCanvasRender>().isRender)
            {
                allPlane[idx].GetComponent<SetCanvasRender>().DisableCanvasRecursively();
                allPlane[(idx + 1) % 4].GetComponent<SetCanvasRender>().EnableCanvasRecursively();
                allPlane[(idx + 2) % 4].GetComponent<SetCanvasRender>().EnableCanvasRecursively();
                allPlane[(idx + 3) % 4].GetComponent<SetCanvasRender>().EnableCanvasRecursively();
            }
        }
    }



    string Date2String(DateTime date)
    {
         
        string formattedDate = date.ToString("dddd, MMMM dd, yyyy h:mm tt",englishCulture);
        return formattedDate;
    }


    void UpdateLabel()
    {
        for (int i = 0; i < labelTextList.Count; i++)
        {
            for (int j = 0; j < labelTextList[i].Count; j++)
            {
                float tmp = dif * j / STCBox.instance.yScale;
                DateTime labelDate = STCBox.instance.nowDate.AddMinutes(tmp);
                labelTextList[i][j].text = Date2String(labelDate);
            }
        }
    }
    void PointerMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        RaycastHit hit;


        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.name == "CanvasPointer")
            {
                Vector3 mouseScreenPos = Input.mousePosition;
                Vector2 pointerLocalPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    hitObject.GetComponent<Canvas>().transform as RectTransform, 
                    mouseScreenPos, 
                    Camera.main, out pointerLocalPos
                );
                for (int i = 0; i < pointerList.Count; i++)
                {
                    TMP_Text pointer = pointerList[i];
                    Vector2 outLocalPos = new Vector2(pointerList[i].rectTransform.anchoredPosition.x, pointerLocalPos.y + 2.5f);
                    // ����ָ���λ�������ָ��ľֲ����걣��һ��
                    pointer.rectTransform.anchoredPosition = outLocalPos;
                    pointer.text = Date2String(STCBox.instance.nowDate.AddMinutes((pointerList[i].rectTransform.anchoredPosition.y + 37.5f) / STCBox.instance.yScale));
                }
                /////////////////////////////////////////////
                // ����갴�����ʱ
                if (Input.GetMouseButtonDown(0))
                {
                    // ��¼��굱ǰλ��
                    lastMousePosition = Input.mousePosition;
                }
                // �������קʱ
                else if (Input.GetMouseButton(0))
                {
                    // ���������Y���ϵ��ƶ�����
                    float deltaY = Input.mousePosition.y - lastMousePosition.y;

                    // �����ƶ��������ı���Ƶı���
                    if (deltaY < 0)
                    {
                        STCBox.instance.nowDate = STCBox.instance.nowDate.AddMinutes(20);
                        UpdatePath.UpdateEveryPath();

                    }
                    else if (deltaY > 0)
                    {
                        STCBox.instance.nowDate = STCBox.instance.nowDate.AddMinutes(-20);
                        UpdatePath.UpdateEveryPath();
                    }

                    // ������һ֡���λ��
                    lastMousePosition = Input.mousePosition;
                }
                /////////////////////////////////////////////
                UpdatePath.ScrollByYAxis();
            }
            else if(hitObject.name == "Map")
            {
                
                if (Input.GetMouseButtonDown(0))
                {
                    dragPlane.Raycast(ray, out float distance);
                    Vector3 hitPoint = ray.GetPoint(distance);
                    offset = allPathTransform.position - hitPoint;
                }
                // �������קʱ
                else if (Input.GetMouseButton(0))
                {
                    dragPlane.Raycast(ray, out float distance);
                    Vector3 hitPoint = ray.GetPoint(distance);
                    allPathTransform.localPosition = new Vector3(
                        hitPoint.x + offset.x, 
                        allPathTransform.localPosition.y, 
                        hitPoint.z + offset.z
                    );
                }
                UpdatePath.ScrollByXZAxis();
            }
        }
    }

}
