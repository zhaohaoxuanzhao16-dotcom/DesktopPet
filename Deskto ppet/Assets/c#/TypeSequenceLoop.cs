using UnityEngine;
using TMPro;
using System.Collections;

public class TypeSequenceLoop : MonoBehaviour
{
    [Header("字幕设置")]
    public TextMeshProUGUI subtitleText;
    public float letterDuration = 0.8f;
    public float popScale = 1.5f;

    // 当前序列（默认 qwer）
    private string sequence = "qwer";
    private int currentIndex = 0;
    private bool isPlayingEffect = false;

    // 保存用的 key
    private const string SaveKey = "EasterEggSequence";

    void Start()
    {
        // 读取保存的序列
        sequence = PlayerPrefs.GetString(SaveKey, "qwer").ToLower();
        currentIndex = 0;

        // 把当前序列显示到设置面板的输入框（后面会说）
        Debug.Log($"彩蛋序列: {sequence}");
    }

    // 允许设置面板调用，改序列
public void SetSequence(string newSeq)
{
    if (string.IsNullOrEmpty(newSeq)) return;

    newSeq = newSeq.Trim();

    // 过滤掉空格、换行、符号（保留中文、字母、数字）
    string filtered = "";
    foreach (char c in newSeq)
    {
        // 中文范围 + 英文字母 + 数字
        if ((c >= '\u4e00' && c <= '\u9fff') ||   // 常用汉字
            (c >= 'a' && c <= 'z') ||
            (c >= 'A' && c <= 'Z') ||
            (c >= '0' && c <= '9'))
        {
            filtered += c;
        }
    }

    if (filtered.Length == 0) return;

    // 长度限制
    if (filtered.Length > 8)
        filtered = filtered.Substring(0, 8);

    sequence = filtered.ToLower();   // 英文统一小写
    currentIndex = 0;

    PlayerPrefs.SetString(SaveKey, sequence);
    PlayerPrefs.Save();

    Debug.Log($"彩蛋序列已更新: {sequence}（{sequence.Length} 个字符）");
}

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
            subtitleText.text = sequence.ToUpper() + "!";
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