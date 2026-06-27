using Fire_Pixel.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;



public class CustomAnimator : MonoBehaviour
{
    [SerializeField] private BakedAnimSO bakedAnimSO;

    [SerializeField] private GameObject targetObj;


    [SerializeField, EditorReadOnly] private Transform[] fullTransformTree;
    [SerializeField, EditorReadOnly] private BakedAnimClip currentClip;

    [SerializeField, EditorReadOnly] private List<Transform> targetTransforms;

#if UNITY_EDITOR
    [SerializeField] private bool showDebugInfo;
#else
    // non editor version for build errors fix
    private bool showDebugInfo => false;
#endif

    [ShowIf("showDebugInfo")]
    [SerializeField, EditorReadOnly] private float playbackTime;

    [field: ShowIf(nameof(showDebugInfo))]
    [field: SerializeField, EditorReadOnly] public bool IsPlaying { get; private set; }


    private float oneDivFrameDuration;
    private bool doLerpSmoothing;

    private BakedAnimTrack[] tracks;
    private int trackCount;

    private Vector3[] positions;
    private Quaternion[] rotations;
    private Vector3[] scales;


#if UNITY_EDITOR
    public GameObject TargetObj => targetObj;
    public Transform[] FullTransformTree => fullTransformTree;


    private void OnValidate()
    {
        if (bakedAnimSO == null || targetObj == null) return;

        ReloadAnimation();

        fullTransformTree = targetObj.transform.GetChildrenRecursively(true).ToArray();
    }

    public void SetBakedAnimSO(BakedAnimSO newSO)
    {
        bakedAnimSO = newSO;
        ReloadAnimation();
    }
#endif


    #region Init/Cleanup

    private void Awake()
    {
        if (targetObj == null)
        {
            DebugLogger.Log($"Auto assigned {gameObject} as animator target since target was empty", showDebugInfo);
            targetObj = gameObject;
            fullTransformTree = targetObj.transform.GetChildrenRecursively(true).ToArray();
        }

        targetTransforms = new List<Transform>(fullTransformTree.Length);

        if (IsPlaying)
        {
            ReloadAnimation();
            Play();
        }
    }
    private void OnDestroy()
    {
        if (IsPlaying) Stop();
    }
    #endregion


    public void ReloadAnimation()
    {
        currentClip = bakedAnimSO.Value;

        oneDivFrameDuration = 1 / currentClip.FrameDuration;
        doLerpSmoothing = currentClip.DoLerpSmoothing;

        tracks = currentClip.Tracks;
        positions = currentClip.Positions;
        rotations = currentClip.Rotations;
        scales = currentClip.Scales;

        trackCount = currentClip.TrackCount;
        targetTransforms.Clear();

        for (int i = 0; i < trackCount; i++)
        {
            targetTransforms.Add(fullTransformTree[tracks[i].TransformId]);
        }
    }


    #region Play/Stop/Crossfade

    [InspectorButton("[Play]")]
    private void Play()
    {
        CallbackScheduler.RegisterCallback(UpdateAnimation, CallbackType.Update);
        IsPlaying = true;
    }
    [InspectorButton("[Stop]")]
    private void Stop()
    {
        CallbackScheduler.UnRegisterCallback(UpdateAnimation, CallbackType.Update);
        IsPlaying = false;
        playbackTime = 0;
    }

    #endregion


    private void UpdateAnimation()
    {
        playbackTime += Time.deltaTime * oneDivFrameDuration;
        while (playbackTime > currentClip.FrameCount)
        {
            playbackTime -= currentClip.FrameCount;
        }

        if (doLerpSmoothing)
        {
            UpdateTransformsSmooth(targetTransforms, playbackTime);
        }
        else
        {
            UpdateTransforms(targetTransforms, (int)playbackTime);
        }
    }

    /// <summary>
    /// Apply smoothed value between previous and current animation frame transformation data to target transform
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateTransforms(List<Transform> transforms, int frameId)
    {
        BakedAnimTrack track;
        Transform transform;

        for (int i = 0; i < trackCount; i++)
        {
            track = tracks[i];
            transform = transforms[i];

            int idx = frameId + track.FrameOffset;

            TransformationFlags flags = track.Flags;

            if ((flags & TransformationFlags.Position) != 0)
            {
                transform.localPosition = positions[idx];
            }
            if ((flags & TransformationFlags.Rotation) != 0)
            {
                transform.localRotation = rotations[idx];
            }
            if ((flags & TransformationFlags.Scale) != 0)
            {
                transform.localScale = scales[idx];
            }
        }
    }
    /// <summary>
    /// Apply smoothed value between previous and current animation frame transformation data to target transform
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateTransformsSmooth(List<Transform> transforms, float playbackTime)
    {
        int frameId = (int)playbackTime;

        bool passedLastAnimFrame = playbackTime >= currentClip.FrameCount - 1;
        float t = playbackTime - (int)playbackTime;

        BakedAnimTrack track;
        Transform transform;

        for (int i = 0; i < trackCount; i++)
        {
            track = tracks[i];
            transform = transforms[i];

            int idx = frameId + track.FrameOffset;
            int idxB = passedLastAnimFrame ? track.FrameOffset : idx + 1;

            TransformationFlags flags = track.Flags;

            if ((flags & TransformationFlags.Position) != 0)
            {
                transform.localPosition = Lerp(positions[idx], positions[idxB], t);
            }
            if ((flags & TransformationFlags.Rotation) != 0)
            {
                transform.localRotation = Lerp(rotations[idx], rotations[idxB], t);
            }
            if ((flags & TransformationFlags.Scale) != 0)
            {
                transform.localScale = Lerp(scales[idx], scales[idxB], t);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateTransformsCrossfade()
    {

    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return a + (b - a) * t;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Quaternion Lerp(Quaternion a, Quaternion b, float t)
    {
        return new Quaternion(
            a.x + (b.x - a.x) * t,
            a.y + (b.y - a.y) * t,
            a.z + (b.z - a.z) * t,
            a.w + (b.w - a.w) * t
        );
    }
}