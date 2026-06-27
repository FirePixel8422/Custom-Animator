using UnityEngine;


[CreateAssetMenu(fileName = "BakedAnimClip", menuName = "ScriptableObjects/Custom Animator/BakedAnimSO", order = -1000)]
public class BakedAnimSO : ScriptableObject
{
#if UNITY_EDITOR
    [field: ErrorIf(ComparisonType.IsNull, "Please assign an animation")]
    [field: SerializeField] public AnimationClip TargetClip { get; private set; }


    [field: ShowIf(nameof(TargetClip))]
    [field: SerializeField] public AnimBakeSettings BakeSettings { get; private set; }
#endif

    [Rename("Baked Animation Clip")]
    [EditorReadOnly] public BakedAnimClip Value;
}