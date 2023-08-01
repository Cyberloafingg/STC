using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeIn : MonoBehaviour
{
    public string targetSceneName; // 设置目标场景的名称
    public CanvasGroup canvasGroup; // 引用新场景中的Canvas Group组件
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;
        StartCoroutine(LoadSceneWithFade());
    }


    private IEnumerator LoadSceneWithFade()
    {
        float fadeDuration = 1.0f; // 渐变持续时间为1秒钟
        float timer = 0.0f; // 用于计时的变量

        // 逐渐减少Canvas Group的alpha值实现渐变效果
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = 1 - timer / fadeDuration;
            timer += Time.deltaTime;
            yield return null;
        }

        // 确保Canvas Group完全透明
        canvasGroup.alpha = 0.0f;

        // 等待一帧以确保渐变完成
        yield return null;
    }
}
