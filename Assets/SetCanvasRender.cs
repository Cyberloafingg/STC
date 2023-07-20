using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCanvasRender : MonoBehaviour
{
    public bool isRender = true;

    public void DisableCanvasRecursively()
    {
        isRender = false;
        // ����������
        foreach (Transform child in this.transform)
        {
            // ����Canvas���
            Canvas canvas = child.GetComponent<Canvas>();
            GraphicRaycaster graphicRaycaster = child.GetComponent<GraphicRaycaster>();
            BoxCollider boxCollider = child.GetComponent<BoxCollider>();

            // ����ҵ�Canvas������������
            if (canvas != null)
            {
                canvas.enabled = false;
            }
            if(graphicRaycaster != null)
            {
                graphicRaycaster.enabled = false;
            }
            if(boxCollider != null)
            {
                boxCollider.enabled = false;
            }
        }
    }

    public void EnableCanvasRecursively()
    {
        isRender = true;
        // ����������
        foreach (Transform child in this.transform)
        {
            // ����Canvas���
            Canvas canvas = child.GetComponent<Canvas>();
            GraphicRaycaster graphicRaycaster = child.GetComponent<GraphicRaycaster>();
            BoxCollider boxCollider = child.GetComponent<BoxCollider>();

            // ����ҵ�Canvas������������
            if (canvas != null)
            {
                canvas.enabled = true;
            }
            if (graphicRaycaster != null)
            {
                graphicRaycaster.enabled = true;
            }
            if (boxCollider != null)
            {
                boxCollider.enabled = true;
            }
        }
    }
}
