using UnityEngine;


[CreateAssetMenu(fileName = "BakedAnimClip", menuName = "ScriptableObjects/Custom Animator/BakedAnimSO", order = -1000)]
public class BakedAnimSO : ScriptableObject
{
    public BakedAnimClip Value;



#if UNITY_EDITOR
    public AnimationClip targetClip;
    public GameObject targetObj;
    [Range(1, 120)]
    public int frameRate;


    [InspectorButton("Bake")]
    private void Bake()
    {
        AnimBakeUtils.Bake(targetClip, targetObj, frameRate, out Value);
    }
#endif
}