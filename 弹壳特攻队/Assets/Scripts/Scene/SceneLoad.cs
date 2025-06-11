using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoads : MonoBehaviour
{
    // 公共变量，用于在Unity编辑器中绑定Slider组件
    public Slider slider;
    // 公共变量，用于在Unity编辑器中绑定Text组件
    public Text progreeText;

    // Start方法在游戏开始时调用
    void Start()
    {
        // 启动协程LoadProgress
        StartCoroutine(LoadProgress());
    }

    // 协程方法，用于模拟加载进度
    private IEnumerator LoadProgress()
    {
        // 定义加载持续时间为5秒
        float duration = 5f;
        // 初始化已过去的时间为0
        float elapsedTime = 0f;

        // 当已过去的时间小于持续时间时，循环执行
        while (elapsedTime < duration)
        {
            // 每帧增加已过去的时间
            elapsedTime += Time.deltaTime;
            // 计算当前进度，并限制在0到1之间
            float progress = Mathf.Clamp01(elapsedTime / duration);
            // 设置Slider的值为当前进度
            slider.value = progress;
            // 设置Text的文本为当前进度的百分比
            progreeText.text = $"{(progress * 100):0}%";
            // 暂停协程，等待下一帧
            yield return null;
        }
        // 当循环结束后，将Slider的值设置为1
        slider.value = 1f;
        // 将Text的文本设置为100%
        progreeText.text = "100%";
        // 异步加载场景，场景索引为2
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(2);
        // 禁止场景自动激活
        asyncOperation.allowSceneActivation = false;
        // 当异步操作未完成时，循环执行
        while (!asyncOperation.isDone)
        {
            // 如果异步操作的进度大于等于0.9
            if (asyncOperation.progress >= 0.9f)
            {
                // 允许场景激活
                asyncOperation.allowSceneActivation = true;
            }
            // 暂停协程，等待下一帧
            yield return null;
        }
    }
}
