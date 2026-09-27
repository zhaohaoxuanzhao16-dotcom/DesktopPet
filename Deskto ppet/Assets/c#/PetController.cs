using UnityEngine;
using System.Collections;

public class PetClickHandler : MonoBehaviour
{
    private Animator animator;

    [Header("鼠标行为")]
    public bool enableMouseClick = true;

    [Header("键盘触发（左右交替）")]
    public bool enableKeyboard = true;

    [Header("状态名")]
    public string idleStateName = "idle";
    public string leftStateName = "PlayLeft";
    public string rightStateName = "PlayRight";

    [Header("触发间隔")]
    public float minInterval = 0.2f;

    private bool nextIsLeft = true;
    private bool isPlayingAction = false;
    private float lastActionTime = -999f;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("未找到 Animator 组件");
    }

    void OnMouseDown()
    {
        if (!enableMouseClick) return;
        if (animator == null) return;

        if (Input.GetMouseButtonDown(0))
            PlayAction(leftStateName);
        else if (Input.GetMouseButtonDown(1))
            PlayAction(rightStateName);
    }

    void Update()
    {
        if (!enableKeyboard) return;
        if (animator == null) return;

        if (Input.anyKeyDown
            && !Input.GetMouseButtonDown(0)
            && !Input.GetMouseButtonDown(1)
            && !Input.GetMouseButtonDown(2)
            && Input.mouseScrollDelta == Vector2.zero)
        {
            string state = nextIsLeft ? leftStateName : rightStateName;

            bool played = PlayAction(state);
            if (played)
                nextIsLeft = !nextIsLeft;
        }
    }

    bool PlayAction(string stateName)
    {
        if (animator == null) return false;

        float elapsed = Time.time - lastActionTime;
        if (elapsed < minInterval)
        {
            Debug.Log($"动作间隔太短（{elapsed:F2}s < {minInterval}s），忽略");
            return false;
        }

        lastActionTime = Time.time;
        StopAllCoroutines();
        animator.Play(stateName, 0, 0f);
        isPlayingAction = true;

        Debug.Log($"播放动作: {stateName}");
        StartCoroutine(ReturnToIdle(stateName));
        return true;
    }

    IEnumerator ReturnToIdle(string stateName)
    {
        yield return null;
        float length = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);

        if (isPlayingAction)
        {
            animator.Play(idleStateName, 0, 0f);
            isPlayingAction = false;
            Debug.Log("回到 idle");
        }
    }
}