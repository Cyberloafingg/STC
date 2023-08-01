using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRSettingPanel : MonoBehaviour
{
    public Canvas canvas;
    CanvasGroup canvasGroup;
    [Range(70.0f,100.0f)]
    public float angle = 80f; // ������Z����ת�ĽǶ�
    [Range(0.1f,1.0f)]
    public float duration = 0.5f; // Canvas��͸����
    public static VRSettingPanel instance;
    // Start is called before the first frame update
    public GameObject markerPrefab;
    List<GameObject> markerList = new List<GameObject>();
    List<Vector3> oriMarkerList = new List<Vector3>();
    public bool isMarkering = false;
    public bool isDeleteMarker = false;

    public Transform markerParent;

    void Start()
    {
        canvasGroup = canvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // ��ʼ��Canvas��͸����Ϊ0
        canvasGroup.interactable = false; // ���ɽ���
        canvasGroup.blocksRaycasts = false; // ���赲����
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
        Quaternion handRotation = transform.localRotation;
        float zRotation = handRotation.eulerAngles.z;
        zRotation = (zRotation + 3600) % 360; // ��֤zRotation��0~360֮��
        // ���������Z����ת����90��
        if (zRotation > angle && zRotation < 200f)
        {
            StopCoroutine(HideCanvasCoroutine()); // ֹͣ����Canvas��Э��
            StartCoroutine(ShowCanvasCoroutine()); // ������ʾCanvas��Э��
        }
        else
        {
            StopCoroutine(ShowCanvasCoroutine()); // ֹͣ��ʾCanvas��Э��
            StartCoroutine(HideCanvasCoroutine()); // ��������Canvas��Э��
        }
    }

    public void Marking(Vector3 clickPosition)
    {
        //��ȡԭʼ�仯
        float scale = STCBox.instance.xScale / 1000;
        GameObject markerTmp = Instantiate(markerPrefab, clickPosition, Quaternion.identity, markerParent);
        markerList.Add(markerTmp);
        float angle = VRUpdatePath.instance.transform.localRotation.eulerAngles.y;
        angle = (angle + 360) % 360;
        Quaternion rotationQuaternion = Quaternion.Euler(0f, -angle, 0f);
        clickPosition = rotationQuaternion * clickPosition;
        clickPosition = new Vector3(clickPosition.x / scale, clickPosition.y, clickPosition.z / scale);
        oriMarkerList.Add(clickPosition);
    }

    public void DeleteMarker(GameObject gameObject)
    {
        for (int i = 0; i < markerList.Count; i++)
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

    public void TurnToDelete()
    {
        isMarkering = false;
        isDeleteMarker = true;
    }

    public void TurnToNormal()
    {
        isMarkering = false;
        isDeleteMarker = false;
    }

    public void TurnToMarkering()
    {
        isMarkering = true;
        isDeleteMarker = false;
    }

    public void ClearAllMarker()
    {
        for (int i = 0; i < markerList.Count; i++)
        {
            Destroy(markerList[i]);
        }
    }

    private IEnumerator ShowCanvasCoroutine()
    {
        float currentTime = 0.0f;
        canvasGroup.interactable = true; // �ɽ���
        canvasGroup.blocksRaycasts = true; // �赲����

        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / duration);
            canvasGroup.alpha = alpha;
            currentTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f; // ȷ��������ʾCanvas
    }

    private IEnumerator HideCanvasCoroutine()
    {
        float currentTime = 0.0f;
        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            canvasGroup.alpha = alpha;
            currentTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f; // ȷ����������Canvas
        canvasGroup.interactable = false; // ���ɽ���
        canvasGroup.blocksRaycasts = false; // ���赲����
    }

    public void UpdateMarkerPosition(float scale)
    {
        for (int i = 0; i < markerList.Count; i++)
        {
            markerList[i].transform.localPosition = new Vector3(
                oriMarkerList[i].x * scale,
                oriMarkerList[i].y,
                oriMarkerList[i].z * scale
            );
        }
    }

}
