using UnityEngine;


[CreateAssetMenu(fileName = "BakedAnimClip", menuName = "ScriptableObjects/Custom Animator/BakedAnimSO", order = -1000)]
public class BakedAnimSO : ScriptableObject
{
#if UNITY_EDITOR
    [field: SerializeField] public AnimationClip TargetClip { get; private set; }
    [field: SerializeField] public int FrameRate { get; private set; }
#endif

    [EditorReadOnly] public BakedAnimClip Value;
}