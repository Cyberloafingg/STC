using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractEvent : MonoBehaviour
{
    public RectTransform tip;
    public int length;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool raycast = Physics.Raycast(ray, out hit, length);
        if (raycast && hit.collider.gameObject.tag == "Path")
        {
            GameObject go = hit.collider.gameObject;
            tip.gameObject.SetActive(true);
            tip.GetComponentInChildren<Text>().text = go.GetComponent<PathObj>().trackName+ "  " + go.GetComponent<PathObj>().startDateString;
            //FollowMouse();
        }
        else
        {
            tip.gameObject.SetActive(false);
        }
        //Debug.DrawLine(ray.origin, ray.origin + ray.direction * length, Color.black, 1.0f);
    }

    void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(tip, mousePosition, Camera.main, out Vector3 worldPosition);
        tip.position = worldPosition;
    }

}