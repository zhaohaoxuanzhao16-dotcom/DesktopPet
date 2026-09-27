using UnityEngine;

[System.Serializable]
public class ClosetItem
{
    public string id;              // 唯一标识，比如 "hat_red"
    public string displayName;     // 显示名字，比如 "红帽子"
    public Sprite icon;            // 图标（UI 里显示）
    public GameObject modelPrefab; // 换装的模型（可选）
}