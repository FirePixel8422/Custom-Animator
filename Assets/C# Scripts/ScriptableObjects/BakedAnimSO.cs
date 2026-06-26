using UnityEngine;


[CreateAssetMenu(fileName = "BakedAnimClip", menuName = "ScriptableObjects/Custom Animator/BakedAnimSO", order = -1000)]
public class BakedAnimSO : ScriptableObject
{
#if UNITY_EDITOR
    [field: ErrorIf(ComparisonType.IsNull, "Please assign an animation")]
    [field: SerializeField] public AnimationClip TargetClip { get; private set; }


    [field: ShowIf(nameof(TargetClip))]
    [field: Range(2, 60)]
    [field: SerializeField] public int BakeFrameRate { get; private set; } = 2;
#endif

    [Rename("Baked Animation Clip")]
    [EditorReadOnly] public BakedAnimClip Value;
}