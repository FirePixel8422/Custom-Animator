using Fire_Pixel.Utility;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;



public class CustomAnimator : MonoBehaviour
{
    [SerializeField] private BakedAnimSO bakedAnimSO;

    [SerializeField] private GameObject targetObj;


    [SerializeField, EditorReadOnly] private Transform[] targetTransforms;
    [SerializeField, EditorReadOnly] private BakedAnimClip currentClip;

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
    private BakedAnimTrack[] tracks;
    private Vector3[] positions;
    private Quaternion[] rotations;
    private Vector3[] scales;


#if UNITY_EDITOR
    public GameObject TargetObj => targetObj;
    public Transform[] TargetTransforms => targetTransforms;


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
        if (IsPlaying)
        {
            ReloadAnimation();
            Play();
        }

        if (targetObj == null)
        {
            DebugLogger.Log($"Auto assigned {gameObject} as animator target since target was empty", showDebugInfo);
            targetObj = gameObject;
        }
    }
    private void OnDestroy()
    {
        if (IsPlaying) Stop();
    }


    public void ReloadAnimation()
    {
        currentClip = bakedAnimSO.Value;
        oneDivFrameDuration = 1 / currentClip.FrameDuration;

        tracks = currentClip.Tracks;
        positions = currentClip.Positions;
        rotations = currentClip.Rotations;
        scales = currentClip.Scales;
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

        UpdateTransforms(targetTransforms, playbackTime);
    }

    /// <summary>
    /// Apply current animation frame transformation data to target transform
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateTransforms(Transform[] transforms, float playbackTime)
    {
        int trackCount = currentClip.TrackCount;
        int frameId = (int)playbackTime;

        bool passedLastAnimFrame = playbackTime >= currentClip.FrameCount - 1;
        float t = playbackTime - (int)playbackTime;

        BakedAnimTrack track;
        Transform transform;

        for (int i = 0; i < trackCount; i++)
        {
            track = tracks[i];

            //Remove TransformId
            //Remove TransformId
            //Remove TransformId
            //Remove TransformId
            //Remove TransformId
            //Remove TransformId
            //Remove TransformId
            //Remove TransformId
            //Remove TransformId
            //Remove TransformId
            //Remove TransformId
            //Remove TransformId
            //Remove TransformId
            //Remove TransformId
            transform = transforms[track.TransformId];

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