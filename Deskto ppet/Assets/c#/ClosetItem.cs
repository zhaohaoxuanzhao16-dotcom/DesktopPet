using UnityEngine;

public enum ClosetCategory
{
    Hat,      // 帽子
    Glasses,  // 眼镜
}

[System.Serializable]
public class ClosetItem
{
    public string id;
    public string displayName;
    public Sprite icon;
    public ClosetCategory category;   // ★ 新增：分类
    public GameObject modelPrefab;
}