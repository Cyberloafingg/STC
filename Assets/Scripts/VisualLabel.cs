using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;
using Unity.VisualScripting;
using System.Reflection;
//ϸ�ڵ���ͷ�쳤ʱ

public class VisualLabel : MonoBehaviour
{
    [SerializeField]
    STCBox STCBox;

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

    public UpdatePath UpdatePath;

    float dif = 5.0f;


    // ������Ҫ���Ƶı���
    public int controlledVariable = 0;

    // ��¼��һ֡����λ��
    private Vector3 lastMousePosition;

    // Start is called before the first frame update
    void Start()
    {
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
        
        for (int i = 0; i < labelTextList.Count; i++)
        {
            for (int j = 0; j < labelTextList[i].Count; j++)
            {
                float tmp = dif * j / STCBox.yScale;
                DateTime labelDate = STCBox.oriDate.AddMinutes(tmp);
                labelTextList[i][j].text = Date2String(labelDate);
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            //Debug.Log(hitObject.name);
            if (hitObject.name == "CanvasPointer")
            {
                Vector3 mouseScreenPos = Input.mousePosition;
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane));
                Vector2 pointerLocalPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(hitObject.GetComponent<Canvas>().transform as RectTransform, mouseScreenPos, Camera.main, out pointerLocalPos);
                for(int i = 0; i < pointerList.Count; i++)
                {
                    TMP_Text pointer = pointerList[i];
                    Vector2 outLocalPos = new Vector2(pointerList[i].rectTransform.anchoredPosition.x, pointerLocalPos.y + 2.5f);
                    // ����ָ���λ�������ָ��ľֲ����걣��һ��
                    pointer.rectTransform.anchoredPosition = outLocalPos;
                    pointer.text = Date2String(STCBox.oriDate.AddMinutes((pointerList[i].rectTransform.anchoredPosition.y + 37.5f) / STCBox.yScale));
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
                        STCBox.oriDate = STCBox.oriDate.AddMinutes(20);
                        UpdatePath.UpdateEveryPath();

                    }
                    else if (deltaY > 0)
                    {
                        STCBox.oriDate = STCBox.oriDate.AddMinutes(-20);
                        UpdatePath.UpdateEveryPath();
                    }

                    // ������һ֡���λ��
                    lastMousePosition = Input.mousePosition;
                }
                /////////////////////////////////////////////
            }
        }


    }


    string Date2String(DateTime date)
    {
         
        string formattedDate = date.ToString("dddd, MMMM dd, yyyy h:mm tt",englishCulture);
        //string output = $"{STCBox.oriDate.DayOfWeek.ToString()},{STCBox.oriDate.Month.ToString()} {STCBox.oriDate.Day},{STCBox.oriDate.Year}";
        return formattedDate;
    }
}
