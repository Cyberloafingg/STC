using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using System.Collections;

public class FolderSelection : MonoBehaviour
{
    string stringPath = "";

    //public string targetSceneName; // ��Inspector�����ָ��Ҫ�л����ĳ�������

    CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OpenFolderDialog(string sceneName)
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);
        //�ɽ����޸�ѡ����ļ�����
        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;
        string path = Application.streamingAssetsPath;
        path = path.Replace('/', '\\');
        //Ĭ��·��
        ofn.initialDir = path;

        ofn.title = "ѡ���ļ���";

        ofn.defExt = "JPG";//��ʾ�ļ�������
                           //ע�� һ����Ŀ��һ��Ҫȫѡ ����0x00000008�Ҫȱ��
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (WindowDll.GetOpenFileName(ofn))
        {
            Debug.Log(ofn.file);
            // ��ȡ�丸Ŀ¼
            stringPath = System.IO.Path.GetDirectoryName(ofn.file);
            PlayerPrefs.SetString("SelectedFolderPath", stringPath);
            Debug.Log(stringPath);
            LoadTargetSceneAsync(sceneName);
        }
    }


    public void LoadTargetSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        float targetLoadTime = 1.0f; // Ŀ�����ʱ��Ϊ2����
        float timer = 0.0f; // ���ڼ�ʱ�ı���
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone && timer < targetLoadTime)
        {
            canvasGroup.alpha = 1 - timer / targetLoadTime;
            timer += Time.deltaTime; // ʹ��Time.deltaTime����ȡ��һ֡����ǰ֡��ʱ����
            Debug.Log("Timer: " + timer);
            yield return null;
        }
        // ȷ���������
        asyncLoad.allowSceneActivation = true;
    }
}
