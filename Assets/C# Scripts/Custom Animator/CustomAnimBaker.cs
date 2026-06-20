using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// MB class responsible for baking <see cref="AnimationClip"/>'s into <see cref="BakedAnimSO"/>'s
/// </summary>
public class CustomAnimBaker : MonoBehaviour
{
    [SerializeField] private BakedAnimSO targetAnimSO;
    [SerializeField] private AnimationClip targetClip;
    [SerializeField] private GameObject targetObj;
    [Range(1, 120)]
    [SerializeField] private int frameRate;

    [SerializeField, EditorReadOnly] private Transform[] targetTransforms;


    private void OnValidate()
    {
        List<Transform> childList = new List<Transform>
        {
            targetObj.transform
        };
        List<Transform> checkList = new List<Transform>
        {
            targetObj.transform
        };

        for (int i = 0; i < checkList.Count; i++)
        {
            Transform current = checkList[i];

            int childCount = current.childCount;
            for (int c = 0; c < childCount; c++)
            {
                Transform child = current.GetChild(c);

                checkList.Add(child);
                childList.Add(child);
            }
        }

        targetTransforms = childList.ToArray();
    }

    [InspectorButton("Bake")]
    private void Bake()
    {
        if (targetAnimSO == null || targetClip == null || targetObj == null)
        {
            DebugLogger.LogWarning("Cant bake animation without targetAnimSO, targetClip and targetObj, skipping...");
            return;
        }

        AnimBakeUtils.Bake(targetClip, targetObj, targetTransforms, frameRate, out targetAnimSO.Value);
    }
}