using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// 桌宠窗口聚焦时的键盘彩蛋
/// - 不依赖任何插件
/// - 窗口有焦点时，按任意键 → 依次显示 q → w → e → r
/// - 累计 4 次触发 QWER! 彩蛋
/// </summary>
public class TypeSequenceLoop : MonoBehaviour
{
    [Header("字幕设置")]
    public TextMeshProUGUI subtitleText;
    public float letterDuration = 0.8f;
    public float popScale = 1.5f;

    private char[] sequence = new char[] { 'q', 'w', 'e', 'r' };
    private int currentIndex = 0;
    private bool isPlayingEffect = false;

    void Update()
    {
        if (Input.anyKeyDown
            && !Input.GetMouseButton(0)
            && !Input.GetMouseButton(1)
            && !Input.GetMouseButton(2)
            && Input.mouseScrollDelta == Vector2.zero)
        {
            OnKeyPressed();
        }
    }

    private void OnKeyPressed()
    {
        if (isPlayingEffect) return;

        TriggerLetter(sequence[currentIndex]);
        currentIndex++;

        if (currentIndex >= sequence.Length)
        {
            currentIndex = 0;
            StartCoroutine(CompleteEffect());
        }
    }

    private void TriggerLetter(char letter)
    {
        if (subtitleText == null) return;
        StopAllCoroutines();
        subtitleText.text = letter.ToString();
        subtitleText.color = Color.yellow;
        subtitleText.gameObject.SetActive(true);
        StartCoroutine(PopAnimation());
    }

    private IEnumerator PopAnimation()
    {
        subtitleText.transform.localScale = Vector3.one * popScale;
        float elapsed = 0f, duration = 0.15f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            subtitleText.transform.localScale = Vector3.Lerp(Vector3.one * popScale, Vector3.one, t);
            yield return null;
        }
        subtitleText.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(letterDuration);
        subtitleText.text = "";
        subtitleText.gameObject.SetActive(false);
    }

    private IEnumerator CompleteEffect()
    {
        isPlayingEffect = true;
        for (int i = 0; i < 3; i++)
        {
            subtitleText.text = "QWER!";
            subtitleText.color = Color.red;
            subtitleText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            subtitleText.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(1f);
        subtitleText.text = "";
        subtitleText.gameObject.SetActive(false);
        isPlayingEffect = false;
    }
}