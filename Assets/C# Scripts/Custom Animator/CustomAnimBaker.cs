using UnityEngine;


/// <summary>
/// MB class responsible for baking <see cref="AnimationClip"/>'s into <see cref="BakedAnimSO"/>'s
/// </summary>
[RequireComponent(typeof(CustomAnimator))]
public class CustomAnimBaker : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField, InlineSO] private BakedAnimSO bakedAnimSO;
    private BakedAnimSO prevBakedAnimSO;

    [SerializeField, EditorReadOnly] private CustomAnimator anim;


    private void Reset()
    {
        anim = GetComponent<CustomAnimator>();
    }
    private void OnValidate()
    {
        if (bakedAnimSO == null) return;

        if (prevBakedAnimSO != bakedAnimSO && TryGetComponent(out CustomAnimator anim))
        {
            anim.BakedAnimSO = bakedAnimSO;
            anim.ReloadAnimation();
        }
        prevBakedAnimSO = bakedAnimSO;
    }

    [InspectorButton("[Bake]")]
    private void Bake()
    {
        if (anim == null || bakedAnimSO == null || bakedAnimSO.TargetClip == null)
        {
            DebugLogger.LogWarning("Cant bake animation without targetAnimSO, targetClip and targetObj, skipping...");
            return;
        }

        AnimBakeUtils.Bake(bakedAnimSO.TargetClip, this.anim.TargetObj, this.anim.TargetTransforms, bakedAnimSO.FrameRate, out bakedAnimSO.Value);
        anim.ReloadAnimation();
    }

    [InspectorButton("[Reload]")]
    private void Reload()
    {
        DebugLogger.Log($"Reloaded Custom Animation Baker on '{name}'");
        Reset();
    }
#endif
}