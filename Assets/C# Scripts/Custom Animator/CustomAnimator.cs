using Fire_Pixel.Utility;
using System.Diagnostics;
using UnityEngine;



public class CustomAnimator : MonoBehaviour
{
    public BakedAnimSO BakedAnimSO;
    [SerializeField] private GameObject targetObj;

    [SerializeField, EditorReadOnly] private Transform[] targetTransforms;
    [SerializeField, EditorReadOnly] private BakedAnimClip currentClip;

    [SerializeField, EditorReadOnly] private float frameId;
    [field: SerializeField, EditorReadOnly] public bool IsPlaying { get; private set; }


#if UNITY_EDITOR
    public GameObject TargetObj => targetObj;
    public Transform[] TargetTransforms => targetTransforms;

    [SerializeField] private bool showDebugInfo;

    [ShowIf(nameof(showDebugInfo))]
    [SerializeField, EditorReadOnly] private string updateTimeMs;
#endif


    private void OnValidate()
    {
        if (BakedAnimSO == null || targetObj == null) return;

        ReloadAnimation();

        targetTransforms = targetObj.transform.GetChildrenRecursively(true).ToArray();
    }


    private void Awake()
    {
        if (IsPlaying) Play();
    }


    public void ReloadAnimation()
    {
        currentClip = BakedAnimSO.Value;
    }


    #region Play/Stop/Crossfade

    [InspectorButton("[Play]")]
    private void Play()
    {
        CallbackScheduler.RegisterCallback(UpdateAnimation, CallbackType.Update);
        IsPlaying = true;

        DebugLogger.LogError($"Transform count isnt equal to baked clip length, {currentClip.TrackCount}, {targetTransforms.Length}", currentClip.TrackCount != targetTransforms.Length);
    }
    [InspectorButton("[Stop]")]
    private void Stop()
    {
        CallbackScheduler.UnRegisterCallback(UpdateAnimation, CallbackType.Update);
        IsPlaying = false;
        frameId = 0;
    }

    #endregion


    private void UpdateAnimation()
    {
        Stopwatch sw = Stopwatch.StartNew();

        frameId += Time.deltaTime / currentClip.FrameDuration;
        frameId %= currentClip.FrameCount;

        currentClip.ApplyToTargetTransforms(targetTransforms, frameId);

        if (showDebugInfo)
        {
            updateTimeMs = (sw.ElapsedTicks * 0.001f).ToString("N2") + "ms";
        }
    }
}