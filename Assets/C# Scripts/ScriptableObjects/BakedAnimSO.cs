using UnityEngine;


[CreateAssetMenu(fileName = "BakedAnimClip", menuName = "ScriptableObjects/Custom Animator/BakedAnimSO", order = -1000)]
public class BakedAnimSO : ScriptableObject
{
#if UNITY_EDITOR
    [field: ErrorIf(ComparisonType.IsNull, "Please assign an animation")]
    [field: SerializeField] public AnimationClip TargetClip { get; private set; }


    [field: WarningIf(ComparisonType.LessThanOrEqual, 0, "FrameRate has to be higher then 0")]
    [field: SerializeField] public int FrameRate { get; private set; }
#endif

    [EditorReadOnly] public BakedAnimClip Value;
}