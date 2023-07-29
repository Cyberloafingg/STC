using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;
using Unity.VisualScripting;
using System.Reflection;
using Unity.Burst.CompilerServices;
using UnityEngine.InputSystem;
//细节当最头伸长时

public class VRVisualLabel : MonoBehaviour
{
    CultureInfo englishCulture;

    public Transform headTransform;

    //获取子物体中的所有canvas，并且tag为labelcanvas
    /// <summary>
    /// 所有的canvasLabel
    /// </summary>
    List<Canvas> canvasLabelList = new List<Canvas>();

    /// <summary>
    /// 所有的canvasPointer
    /// </summary>
    List<Canvas> canvasPointerList = new List<Canvas>();

    List<TMP_Text> pointerList = new List<TMP_Text>();

    /// <summary>
    /// 所有的Label
    /// </summary>
    List<List<TMP_Text>> labelTextList = new List<List<TMP_Text>>();

    public GameObject[] allPlane;

    public VRUpdatePath UpdatePath;

    public Transform allPathTransform;

    public Transform mapTransform;

    private Plane dragPlane;

    Vector3 offset;

    float dif = 5.0f;


    // 记录上一帧鼠标的位置
    private Vector3 lastMousePosition;

    public float transitionSpeed = 5f;

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

        UpdateLabel();

        PointerMove();
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
                DateTime labelDate = STCBox.instance.nowDate.AddMinutes(-tmp);
                labelTextList[i][j].text = Date2String(labelDate);
            }
        }
    }
    void PointerMove()
    {
        Vector3 headPosition = headTransform.position;
        Vector3 headForward = headTransform.forward;
        RaycastHit hit;
        if (Physics.Raycast(headPosition, headForward, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.name == "CanvasPointer")
            {
                Vector2 pointerLocalPos;
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, hit.point);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    hitObject.GetComponent<Canvas>().transform as RectTransform,
                    screenPoint,
                    Camera.main, out pointerLocalPos
                );
                for (int i = 0; i < pointerList.Count; i++)
                {
                    TMP_Text pointer = pointerList[i];
                    Vector2 outLocalPos = new Vector2(pointerList[i].rectTransform.anchoredPosition.x, pointerLocalPos.y + 2.5f);
                    // 设置指针的位置与鼠标指针的局部坐标保持一致
                    pointer.rectTransform.anchoredPosition = outLocalPos;
                    pointer.text = Date2String(STCBox.instance.nowDate.AddMinutes(-(pointerList[i].rectTransform.anchoredPosition.y + 37.5f) / STCBox.instance.yScale));
                }
            }

        }
    }

}
