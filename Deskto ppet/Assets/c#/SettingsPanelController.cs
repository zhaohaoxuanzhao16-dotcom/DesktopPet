using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanelController : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject settingsPanel;
    public Button openButton;
    public Button closeButton;

    [Header("Tab 按钮")]
    public Button btnTabCloset;
    public Button btnTabEaster;
    public Button btnTabSettings;

    [Header("页面")]
    public GameObject pageCloset;
    public GameObject pageEaster;
    public GameObject pageSettings;

    [Header("彩蛋页")]
    public Toggle toggleRememberPos;
    public Toggle toggleEasterEgg;
    public TMP_InputField inputSequence;
    public Button btnApplySequence;
    public Button btnConfirm;

    [Header("设置页")]
    public Button btnZoomIn;
    public Button btnZoomOut;
    public Button btnFlip;
    public Toggle toggleHideButton;

    [Header("隐藏设置按钮")]
    public GameObject settingsButton;

    [Header("要控制的脚本")]
    public PetScreenAdapter petAdapter;
    public TypeSequenceLoop easterEgg;

    [Header("缩放设置")]
    public Camera targetCamera;
    public Transform petModel;
    public float defaultDistance = 4.31f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float zoomStep = 0.5f;

    private float currentDistance;
    private bool isFlipped = false;
    private Vector3 originalEuler;   // 原始旋转

    void Start()
    {
        Debug.Log("=== SettingsPanelController Start ===");

        if (targetCamera == null) targetCamera = Camera.main;
        if (petModel == null)
        {
            var go = GameObject.Find("player");
            if (go != null) petModel = go.transform;
        }

        // 记录原始旋转
        if (petModel != null)
            originalEuler = petModel.localEulerAngles;

        // 读取设置
        if (toggleRememberPos != null)
            toggleRememberPos.isOn = PlayerPrefs.GetInt("RememberPos", 1) == 1;
        if (toggleEasterEgg != null)
            toggleEasterEgg.isOn = PlayerPrefs.GetInt("EasterEgg", 1) == 1;
        if (toggleHideButton != null)
            toggleHideButton.isOn = PlayerPrefs.GetInt("HideSettingsButton", 0) == 1;

        // 缩放距离
        currentDistance = PlayerPrefs.GetFloat("ZoomDistance", defaultDistance);
        ApplyDistance();

        // 读取旋转状态
        isFlipped = PlayerPrefs.GetInt("PetFlipped", 0) == 1;
        ApplyRotation();

        // 输入框
        if (inputSequence != null)
            inputSequence.text = PlayerPrefs.GetString("EasterEggSequence", "qwer");

        ApplySettings();
        ApplyHideSettingsButton();

        // ---- 绑定按钮 ----
        if (openButton != null)
            openButton.onClick.AddListener(() => settingsPanel.SetActive(true));

        if (closeButton != null)
            closeButton.onClick.AddListener(() => settingsPanel.SetActive(false));

        if (btnTabCloset != null)
            btnTabCloset.onClick.AddListener(() => ShowPage("closet"));
        if (btnTabEaster != null)
            btnTabEaster.onClick.AddListener(() => ShowPage("easter"));
        if (btnTabSettings != null)
            btnTabSettings.onClick.AddListener(() => ShowPage("settings"));

        if (btnZoomIn != null)  btnZoomIn.onClick.AddListener(ZoomIn);
        if (btnZoomOut != null) btnZoomOut.onClick.AddListener(ZoomOut);
        if (btnFlip != null)    btnFlip.onClick.AddListener(FlipHorizontal);

        if (btnApplySequence != null)
        {
            btnApplySequence.onClick.AddListener(() =>
            {
                if (inputSequence != null && easterEgg != null)
                    easterEgg.SetSequence(inputSequence.text);
            });
        }

        if (btnConfirm != null)
            btnConfirm.onClick.AddListener(OnConfirmPressed);

        if (toggleRememberPos != null)
            toggleRememberPos.onValueChanged.AddListener(_ => ApplySettings());
        if (toggleEasterEgg != null)
            toggleEasterEgg.onValueChanged.AddListener(_ => ApplySettings());

        ShowPage("closet");

        Debug.Log("=== 初始化完成 ===");
    }

    // ---------------- Tab 切换 ----------------
    void ShowPage(string pageName)
    {
        if (pageCloset != null)   pageCloset.SetActive(false);
        if (pageEaster != null)   pageEaster.SetActive(false);
        if (pageSettings != null) pageSettings.SetActive(false);

        switch (pageName)
        {
            case "closet":
                if (pageCloset != null) pageCloset.SetActive(true);
                break;
            case "easter":
                if (pageEaster != null) pageEaster.SetActive(true);
                break;
            case "settings":
                if (pageSettings != null) pageSettings.SetActive(true);
                break;
        }

        Debug.Log($"切换到页: {pageName}");
    }

    // ---------------- 确认按钮 ----------------
    void OnConfirmPressed()
    {
        bool hide = toggleHideButton != null && toggleHideButton.isOn;
        PlayerPrefs.SetInt("HideSettingsButton", hide ? 1 : 0);
        PlayerPrefs.Save();

        ApplyHideSettingsButton();
        if (settingsPanel != null) settingsPanel.SetActive(false);

        Debug.Log($"确认：隐藏设置按钮 = {hide}");
    }

    void ApplyHideSettingsButton()
    {
        bool hide = PlayerPrefs.GetInt("HideSettingsButton", 0) == 1;
        if (settingsButton != null)
            settingsButton.SetActive(!hide);
    }

    // ---------------- 缩放 ----------------
    public void ZoomIn()
    {
        if (targetCamera == null || petModel == null) return;
        currentDistance = Mathf.Max(minDistance, currentDistance - zoomStep);
        ApplyDistance();
        SaveZoom();
    }

    public void ZoomOut()
    {
        if (targetCamera == null || petModel == null) return;
        currentDistance = Mathf.Min(maxDistance, currentDistance + zoomStep);
        ApplyDistance();
        SaveZoom();
    }

    void ApplyDistance()
    {
        if (targetCamera == null || petModel == null) return;
        Vector3 camPos = targetCamera.transform.position;
        camPos.z = petModel.position.z - currentDistance;
        targetCamera.transform.position = camPos;
    }

    void SaveZoom()
    {
        PlayerPrefs.SetFloat("ZoomDistance", currentDistance);
        PlayerPrefs.Save();
    }

    // ---------------- 旋转 180° ----------------
    public void FlipHorizontal()
    {
        if (petModel == null) return;

        isFlipped = !isFlipped;
        ApplyRotation();

        PlayerPrefs.SetInt("PetFlipped", isFlipped ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"旋转: {isFlipped}, euler = {petModel.localEulerAngles}");
    }

    void ApplyRotation()
    {
        if (petModel == null) return;

        Vector3 euler = originalEuler;
        if (isFlipped)
            euler.y += 300f;

        petModel.localEulerAngles = euler;
    }

    // ---------------- 应用设置 ----------------
    void ApplySettings()
    {
        if (petAdapter != null && toggleRememberPos != null)
            petAdapter.rememberPosition = toggleRememberPos.isOn;

        if (easterEgg != null && toggleEasterEgg != null)
            easterEgg.enabled = toggleEasterEgg.isOn;

        if (toggleRememberPos != null)
            PlayerPrefs.SetInt("RememberPos", toggleRememberPos.isOn ? 1 : 0);
        if (toggleEasterEgg != null)
            PlayerPrefs.SetInt("EasterEgg", toggleEasterEgg.isOn ? 1 : 0);

        PlayerPrefs.Save();
    }
}