using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingPanel : MonoBehaviour
{
    public UpdatePath UpdatePath;

    public static SettingPanel instance;

    public bool isMarking;

    List<GameObject> markerList = new List<GameObject>();

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
    }

    public void TurnToNormal()
    {
        isMarking = false;
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
        GameObject markerTmp = Instantiate(markerPrefab, clickPosition, Quaternion.identity,markerParent);
        
        markerList.Add(markerTmp);
    }
}
