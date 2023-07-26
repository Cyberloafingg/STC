using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeIn : MonoBehaviour
{
    public string targetSceneName; // ����Ŀ�곡��������
    public CanvasGroup canvasGroup; // �����³����е�Canvas Group���
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;
        StartCoroutine(LoadSceneWithFade());
    }


    private IEnumerator LoadSceneWithFade()
    {
        float fadeDuration = 1.0f; // �������ʱ��Ϊ1����
        float timer = 0.0f; // ���ڼ�ʱ�ı���

        // �𽥼���Canvas Group��alphaֵʵ�ֽ���Ч��
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = 1 - timer / fadeDuration;
            timer += Time.deltaTime;
            yield return null;
        }

        // ȷ��Canvas Group��ȫ͸��
        canvasGroup.alpha = 0.0f;

        // �ȴ�һ֡��ȷ���������
        yield return null;
    }
}
