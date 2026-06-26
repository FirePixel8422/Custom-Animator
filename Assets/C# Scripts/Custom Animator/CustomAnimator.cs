using Fire_Pixel.Utility;
using System.Diagnostics;
using UnityEngine;



public class CustomAnimator : MonoBehaviour
{
    [SerializeField] private BakedAnimSO bakedAnimSO;

    [SerializeField] private GameObject targetObj;


    [SerializeField, EditorReadOnly] private Transform[] targetTransforms;
    [SerializeField, EditorReadOnly] private BakedAnimClip currentClip;

#if UNITY_EDITOR
    [SerializeField] private bool showDebugInfo;
#endif

    [ShowIf("showDebugInfo")]
    [SerializeField, EditorReadOnly] private float playbackTime;

    [field: ShowIf("showDebugInfo")]
    [field: SerializeField, EditorReadOnly] public bool IsPlaying { get; private set; }


#if UNITY_EDITOR
    public GameObject TargetObj => targetObj;
    public Transform[] TargetTransforms => targetTransforms;

    [ShowIf("showDebugInfo")]
    [SerializeField, EditorReadOnly] private string updateTimeMs;
    [ShowIf("showDebugInfo")]
    [SerializeField, EditorReadOnly] private string updateTimeAvg;

    private long totalUpdateTimeTicks;
    private int totalElapsedFrames;
    private Stopwatch sw;


    private void OnValidate()
    {
        if (bakedAnimSO == null || targetObj == null) return;

        ReloadAnimation();

        targetTransforms = targetObj.transform.GetChildrenRecursively(true).ToArray();
    }

    public void SetBakedAnimSO(BakedAnimSO newSO)
    {
        bakedAnimSO = newSO;
        ReloadAnimation();
    }
#endif


    private void Awake()
    {
        if (IsPlaying) Play();
    }


    public void ReloadAnimation()
    {
        currentClip = bakedAnimSO.Value;
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
        playbackTime = 0;

#if UNITY_EDITOR
        totalUpdateTimeTicks = 0;
        totalElapsedFrames = 0;
#endif
    }

    #endregion


    private void UpdateAnimation()
    {
#if UNITY_EDITOR
        if (showDebugInfo)
        {
            sw = Stopwatch.StartNew();
        }
#endif

        playbackTime += Time.deltaTime / currentClip.FrameDuration;
        playbackTime %= currentClip.FrameCount;

        currentClip.ApplyToTargetTransforms(targetTransforms, playbackTime);

#if UNITY_EDITOR
        if (showDebugInfo)
        {
            totalUpdateTimeTicks += sw.ElapsedTicks;
            totalElapsedFrames += 1;

            updateTimeMs = (sw.ElapsedTicks * 0.001f).ToString("N2") + "ms";
            updateTimeAvg = ((float)totalUpdateTimeTicks / totalElapsedFrames * 0.001f).ToString("N2") + "ms";
        }
#endif
    }
}