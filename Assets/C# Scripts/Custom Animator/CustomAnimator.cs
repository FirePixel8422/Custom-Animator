using System.Collections.Generic;
using UnityEngine;



public class CustomAnimator : UpdateMonoBehaviour
{
    [SerializeField] private GameObject targetObj;
    [SerializeField, EditorReadOnly] private Transform[] transforms;
    [SerializeField, EditorReadOnly] private BakedAnimClip currentClip;

    [SerializeField] private float frameId;
    [SerializeField] private bool isPlaying;


    [SerializeField] private BakedAnimSO bakedAnimSO;


    private void OnValidate()
    {
        if (bakedAnimSO == null || targetObj == null) return;

        currentClip = bakedAnimSO.Value;

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

        transforms = childList.ToArray();
    }

    [InspectorButton("Play")]
    private void Play()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        isPlaying = true;
    }
    [InspectorButton("Stop")]
    private void Stop()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        isPlaying = false;
        //frameId = 0;
    }

    protected override void OnUpdate()
    {
        if (!isPlaying) return;

        DebugLogger.Throw($"Transform count isnt equal to baked clip length, {currentClip.TrackCount}, {transforms.Length}", currentClip.TrackCount != transforms.Length);

        frameId += Time.deltaTime / currentClip.FrameDuration;

        if (frameId >= currentClip.FrameCount)
        {
            frameId -= currentClip.FrameCount;
        }

        currentClip.ApplyToTargetTransforms(transforms, frameId);
    }
}