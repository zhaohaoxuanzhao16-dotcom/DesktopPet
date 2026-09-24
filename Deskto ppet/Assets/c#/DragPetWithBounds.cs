using UnityEngine;

/// <summary>
/// 桌宠窗口内边界限制 + 位置记忆
/// - 拖拽交给 UniWindowMoveHandle
/// - 本脚本只保证：桌宠永远不会跑出窗口可视区域
/// - 与窗口在屏幕上的位置完全无关（左右对称）
/// </summary>
public class PetScreenAdapter : MonoBehaviour
{
    private Camera mainCamera;

    [Header("边界设置")]
    [Range(0f, 1f)]
    [Tooltip("至少保留多少比例的桌宠在窗口内。0.5 = 一半，1 = 完全在窗口内")]
    public float keepInsideRatio = 0.5f;

    [Header("位置记忆")]
    public bool rememberPosition = true;
    private string saveKey = "PetPosition";

    // 模型在“视口”中的半宽 / 半高（0~1，不是像素！）
    private float modelHalfW = 0.1f;
    private float modelHalfH = 0.1f;

    void Start()
    {
        mainCamera = Camera.main;

        // ✅ 只在启动时估算一次，用视口单位
        EstimateModelViewportSize();

        if (rememberPosition)
            LoadPosition();
    }

    void Update()
    {
        // 只在启动时限制一次边界，之后不再每帧强拉
        // 如果你希望“窗口改变尺寸时自动回归”，可以打开下面这行
        // transform.position = ClampToViewport(transform.position);
    }

    // -------------------------------------------------
    // 核心：把模型限制在“视口范围 0~1”内
    // -------------------------------------------------
    private Vector3 ClampToViewport(Vector3 worldPos)
    {
        Vector3 vp = mainCamera.WorldToViewportPoint(worldPos);

        // 模型边缘在视口中的安全内缩量
        float marginX = modelHalfW * keepInsideRatio;
        float marginY = modelHalfH * keepInsideRatio;

        vp.x = Mathf.Clamp(vp.x, marginX, 1f - marginX);
        vp.y = Mathf.Clamp(vp.y, marginY, 1f - marginY);

        Vector3 newWorld = mainCamera.ViewportToWorldPoint(vp);
        newWorld.z = worldPos.z;
        return newWorld;
    }

    // -------------------------------------------------
    // 用“视口单位”估算模型尺寸（只调用一次）
    // -------------------------------------------------
    private void EstimateModelViewportSize()
    {
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning("PetScreenAdapter: 没有 Collider，无法估算尺寸");
            return;
        }

        Bounds b = col.bounds;
        Vector3 c = b.center;
        Vector3 e = b.extents;

        Vector3[] corners = new Vector3[8];
        corners[0] = c + new Vector3( e.x,  e.y,  e.z);
        corners[1] = c + new Vector3( e.x,  e.y, -e.z);
        corners[2] = c + new Vector3( e.x, -e.y,  e.z);
        corners[3] = c + new Vector3( e.x, -e.y, -e.z);
        corners[4] = c + new Vector3(-e.x,  e.y,  e.z);
        corners[5] = c + new Vector3(-e.x,  e.y, -e.z);
        corners[6] = c + new Vector3(-e.x, -e.y,  e.z);
        corners[7] = c + new Vector3(-e.x, -e.y, -e.z);

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (var p in corners)
        {
            Vector3 vp = mainCamera.WorldToViewportPoint(p);
            minX = Mathf.Min(minX, vp.x);
            maxX = Mathf.Max(maxX, vp.x);
            minY = Mathf.Min(minY, vp.y);
            maxY = Mathf.Max(maxY, vp.y);
        }

        modelHalfW = (maxX - minX) * 0.5f;
        modelHalfH = (maxY - minY) * 0.5f;

        Debug.Log($"模型视口半宽={modelHalfW:F3}, 半高={modelHalfH:F3}");
    }

    // -------------------------------------------------
    // 位置记忆
    // -------------------------------------------------
    private void SavePosition()
    {
        Vector3 p = transform.position;
        PlayerPrefs.SetFloat(saveKey + "_x", p.x);
        PlayerPrefs.SetFloat(saveKey + "_y", p.y);
        PlayerPrefs.Save();
    }

    private void LoadPosition()
    {
        if (!PlayerPrefs.HasKey(saveKey + "_x")) return;

        float x = PlayerPrefs.GetFloat(saveKey + "_x");
        float y = PlayerPrefs.GetFloat(saveKey + "_y");
        Vector3 p = new Vector3(x, y, transform.position.z);
        transform.position = ClampToViewport(p);
    }

    void OnApplicationQuit()
    {
        if (rememberPosition) SavePosition();
    }

    public void ResetPosition()
    {
        PlayerPrefs.DeleteKey(saveKey + "_x");
        PlayerPrefs.DeleteKey(saveKey + "_y");
        PlayerPrefs.Save();

        transform.position = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
    }
}
