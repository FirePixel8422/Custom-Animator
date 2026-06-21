using UnityEngine;


/// <summary>
/// MB class responsible for baking <see cref="AnimationClip"/>'s into <see cref="BakedAnimSO"/>'s
/// </summary>
public class CustomAnimBaker : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private BakedAnimSO bakedAnimSO;
    [SerializeField] private AnimationClip targetClip;
    [SerializeField] private GameObject targetObj;
    [Range(1, 120)]
    [SerializeField] private int frameRate;

    [SerializeField, EditorReadOnly] private Transform[] targetTransforms;


    private void OnValidate()
    {
        if (targetObj == null) return;

        targetTransforms = targetObj.transform.GetChildrenRecursively(true).ToArray();
    }

    [InspectorButton("Bake")]
    private void Bake()
    {
        if (bakedAnimSO == null || targetClip == null || targetObj == null)
        {
            DebugLogger.LogWarning("Cant bake animation without targetAnimSO, targetClip and targetObj, skipping...");
            return;
        }

        AnimBakeUtils.Bake(targetClip, targetObj, targetTransforms, frameRate, out bakedAnimSO.Value);

        if (TryGetComponent(out CustomAnimator anim))
        {
            anim.ReloadAnimation();
        }
    }
#endif
}