using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoads : MonoBehaviour
{
    // ����������������Unity�༭���а�Slider���
    public Slider slider;
    // ����������������Unity�༭���а�Text���
    public Text progreeText;

    // Start��������Ϸ��ʼʱ����
    void Start()
    {
        // ����Э��LoadProgress
        StartCoroutine(LoadProgress());
    }

    // Э�̷���������ģ����ؽ���
    private IEnumerator LoadProgress()
    {
        // ������س���ʱ��Ϊ5��
        float duration = 5f;
        // ��ʼ���ѹ�ȥ��ʱ��Ϊ0
        float elapsedTime = 0f;

        // ���ѹ�ȥ��ʱ��С�ڳ���ʱ��ʱ��ѭ��ִ��
        while (elapsedTime < duration)
        {
            // ÿ֡�����ѹ�ȥ��ʱ��
            elapsedTime += Time.deltaTime;
            // ���㵱ǰ���ȣ���������0��1֮��
            float progress = Mathf.Clamp01(elapsedTime / duration);
            // ����Slider��ֵΪ��ǰ����
            slider.value = progress;
            // ����Text���ı�Ϊ��ǰ���ȵİٷֱ�
            progreeText.text = $"{(progress * 100):0}%";
            // ��ͣЭ�̣��ȴ���һ֡
            yield return null;
        }
        // ��ѭ�������󣬽�Slider��ֵ����Ϊ1
        slider.value = 1f;
        // ��Text���ı�����Ϊ100%
        progreeText.text = "100%";
        // �첽���س�������������Ϊ2
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(2);
        // ��ֹ�����Զ�����
        asyncOperation.allowSceneActivation = false;
        // ���첽����δ���ʱ��ѭ��ִ��
        while (!asyncOperation.isDone)
        {
            // ����첽�����Ľ��ȴ��ڵ���0.9
            if (asyncOperation.progress >= 0.9f)
            {
                // ����������
                asyncOperation.allowSceneActivation = true;
            }
            // ��ͣЭ�̣��ȴ���һ֡
            yield return null;
        }
    }
}
