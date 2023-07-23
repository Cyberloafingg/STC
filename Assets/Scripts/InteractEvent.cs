using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractEvent : MonoBehaviour
{
    public RectTransform tipUI;
    public Vector3 tipUIOffset;
    public int rayLength;

    Image image;
    TMP_Text tipText;

    void Start()
    {
        image = tipUI.gameObject.GetComponent<Image>();
        tipText = image.gameObject.GetComponentInChildren<TMP_Text>();
        image.enabled = false;
        tipText.enabled = false;
    }


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool raycast = Physics.Raycast(ray, out hit, rayLength);
        if(raycast && hit.collider.gameObject.tag == "Path")
        {
            tipUI.position = Input.mousePosition + tipUIOffset;
            PathObj pathObj= hit.collider.gameObject.GetComponent<PathObj>();
            tipText.text = pathObj.trackName + " " + pathObj.startDateString;
            image.enabled = true;
            tipText.enabled = true;
        }
        else
        {
            image.enabled = false;
            tipText.enabled = false;
        }        
    }

}
