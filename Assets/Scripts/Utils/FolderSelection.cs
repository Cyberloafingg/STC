using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using System.Collections;

public class FolderSelection : MonoBehaviour
{
    string stringPath = "";

    //public string targetSceneName; // 在Inspector面板中指定要切换到的场景名称

    CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OpenFolderDialog(string sceneName)
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);
        //可进行修改选择的文件类型
        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;
        string path = Application.streamingAssetsPath;
        path = path.Replace('/', '\\');
        //默认路径
        ofn.initialDir = path;

        ofn.title = "选择文件夹";

        ofn.defExt = "JPG";//显示文件的类型
                           //注意 一下项目不一定要全选 但是0x00000008项不要缺少
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (WindowDll.GetOpenFileName(ofn))
        {
            Debug.Log(ofn.file);
            // 获取其父目录
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
        float targetLoadTime = 1.0f; // 目标加载时间为2秒钟
        float timer = 0.0f; // 用于计时的变量
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone && timer < targetLoadTime)
        {
            canvasGroup.alpha = 1 - timer / targetLoadTime;
            timer += Time.deltaTime; // 使用Time.deltaTime来获取上一帧到当前帧的时间间隔
            Debug.Log("Timer: " + timer);
            yield return null;
        }
        // 确保加载完成
        asyncLoad.allowSceneActivation = true;
    }
}
