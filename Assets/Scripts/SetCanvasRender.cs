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
        // 遍历子物体
        foreach (Transform child in this.transform)
        {
            // 查找Canvas组件
            Canvas canvas = child.GetComponent<Canvas>();
            GraphicRaycaster graphicRaycaster = child.GetComponent<GraphicRaycaster>();
            BoxCollider boxCollider = child.GetComponent<BoxCollider>();

            // 如果找到Canvas组件，则将其禁用
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
        // 遍历子物体
        foreach (Transform child in this.transform)
        {
            // 查找Canvas组件
            Canvas canvas = child.GetComponent<Canvas>();
            GraphicRaycaster graphicRaycaster = child.GetComponent<GraphicRaycaster>();
            BoxCollider boxCollider = child.GetComponent<BoxCollider>();

            // 如果找到Canvas组件，则将其禁用
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
