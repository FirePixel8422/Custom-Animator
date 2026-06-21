using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;



public class CustomAnimator : UpdateMonoBehaviour
{
    [SerializeField] private BakedAnimSO bakedAnimSO;
    [SerializeField] private GameObject targetObj;

    [SerializeField, EditorReadOnly] private Transform[] transforms;
    [SerializeField, EditorReadOnly] private BakedAnimClip currentClip;

    [SerializeField, EditorReadOnly] private float frameId;
    [SerializeField, EditorReadOnly] private bool isPlaying;


    private void OnValidate()
    {
        if (bakedAnimSO == null || targetObj == null) return;

        ReloadAnimation();

        transforms = targetObj.transform.GetChildrenRecursively(true).ToArray();
    }
    public void ReloadAnimation()
    {
        currentClip = bakedAnimSO.Value;
    }

    [InspectorButton("Play")]
    private void Play()
    {
        isPlaying = true;
    }
    [InspectorButton("Stop")]
    private void Stop()
    {
        isPlaying = false;
        //frameId = 0;
    }

    protected override void OnUpdate()
    {
        if (!isPlaying) return;

        //DebugLogger.Throw($"Transform count isnt equal to baked clip length, {currentClip.TrackCount}, {transforms.Length}", currentClip.TrackCount != transforms.Length);

        frameId += Time.deltaTime / currentClip.FrameDuration;
        frameId %= currentClip.FrameCount;

        currentClip.ApplyToTargetTransforms(transforms, frameId);
    }
}