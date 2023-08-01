using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    public UpdatePath UpdatePath;

    public static SettingPanel instance;

    public bool isMarking;

    public bool isDeleteMarker;

    public Image markerLabel;

    List<GameObject> markerList = new List<GameObject>();
    List<Vector3> oriMarkerList = new List<Vector3>();

    Transform markerParent;


    public GameObject markerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        markerParent = STCBox.instance.GetComponentInChildren<UpdatePath>().transform;
    }


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(isMarking)
        {
            markerLabel.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("BeginScene");
    }

    public void ColorByTime()
    {
        UpdatePath.ColorByTime();
    }

    public void TurnToMarking()
    {
        isMarking = true;
        isDeleteMarker = false;
        markerLabel.enabled = true;
    }

    public void TurnToDeleteMarker()
    {
        isMarking = false;
        isDeleteMarker = true;
        markerLabel.enabled = false;
    }

    public void TurnToNormal()
    {
        isMarking = false;
        isDeleteMarker = false;
        markerLabel.enabled = false;
    }

    public void ClearAllMarker()
    {
        for(int i = 0; i < markerList.Count; i++)
        {
            Destroy(markerList[i]);
        }
    }

    public void AutoMove()
    {
        UpdatePath.AutoMoveOnOrOff();
    }    

    public void Marking(Vector3 clickPosition)
    {
        float scale = STCBox.instance.xScale / 1000;
        GameObject markerTmp = Instantiate(markerPrefab, clickPosition, Quaternion.identity,markerParent);  
        markerList.Add(markerTmp);
        clickPosition = new Vector3(clickPosition.x / scale, clickPosition.y, clickPosition.z / scale);
        oriMarkerList.Add(clickPosition);
    }

    public void DeleteMarker(GameObject gameObject)
    {
        for(int i = 0; i < markerList.Count; i++)
        {
            if (markerList[i] == gameObject)
            {
                markerList.RemoveAt(i);
                oriMarkerList.RemoveAt(i);
                Destroy(gameObject);
                return;
            }
        }
    }

    public void UpdateMarkerPosition(float scale)
    {
        for(int i = 0; i < markerList.Count; i++)
        {
            markerList[i].transform.localPosition = new Vector3(
                oriMarkerList[i].x * scale,
                oriMarkerList[i].y, 
                oriMarkerList[i].z * scale
            );
        }
    }
}
