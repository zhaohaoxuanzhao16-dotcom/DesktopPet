using UnityEngine;
using TMPro;
using System.Collections;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// Windows 全局键盘监听 + 桌宠彩蛋
/// 挂载到桌宠物体上即可使用
/// 无需任何外部插件
/// </summary>
public class WindowsGlobalKey : MonoBehaviour
{
    [Header("字幕设置")]
    public TextMeshProUGUI subtitleText;
    public float letterDuration = 0.8f;
    public float popScale = 1.5f;

    // 字母序列
    private char[] letters = new char[] { 'q', 'w', 'e', 'r' };
    private int currentIndex = 0;
    private bool isCompleted = false;

    // ---- Windows API 全局键盘钩子 ----
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;

    private static IntPtr _hookID = IntPtr.Zero;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static WindowsGlobalKey _instance;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        // 安装全局键盘钩子
        _hookID = SetHook(_proc);
        if (_hookID == IntPtr.Zero)
        {
            Debug.LogError("❌ 全局键盘钩子安装失败！请以管理员身份运行。");
        }
        else
        {
            Debug.Log("✅ 全局键盘钩子安装成功！");
        }
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (System.Diagnostics.Process curProcess = System.Diagnostics.Process.GetCurrentProcess())
        using (System.Diagnostics.ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            if (_instance != null)
            {
                _instance.TriggerLetter();
            }
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    void OnDestroy()
    {
        if (_hookID != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookID);
            _hookID = IntPtr.Zero;
            Debug.Log("全局键盘钩子已卸载。");
        }
    }

    // ---- 彩蛋逻辑 ----
    public void TriggerLetter()
    {
        if (isCompleted) return;

        ShowLetter(letters[currentIndex]);
        currentIndex++;

        if (currentIndex >= letters.Length)
        {
            currentIndex = 0;
            StartCoroutine(CompleteEffect());
        }
    }

    private void ShowLetter(char letter)
    {
        if (subtitleText == null) return;

        subtitleText.text = letter.ToString();
        subtitleText.color = Color.yellow;
        subtitleText.gameObject.SetActive(true);
        StartCoroutine(PopAnimation());
    }

    private IEnumerator PopAnimation()
    {
        Vector3 originalScale = subtitleText.transform.localScale;
        subtitleText.transform.localScale = Vector3.one * popScale;

        float elapsed = 0;
        float duration = 0.15f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            subtitleText.transform.localScale = Vector3.Lerp(Vector3.one * popScale, Vector3.one, t);
            yield return null;
        }

        subtitleText.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(letterDuration);

        if (currentIndex < letters.Length)
        {
            subtitleText.text = "";
            subtitleText.gameObject.SetActive(false);
        }
    }

    private IEnumerator CompleteEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            subtitleText.text = "QWER!";
            subtitleText.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            subtitleText.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1f);
        subtitleText.text = "";
        subtitleText.gameObject.SetActive(false);
        isCompleted = false;
    }
}
