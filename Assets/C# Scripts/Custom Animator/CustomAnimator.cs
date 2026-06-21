using UnityEngine;



public class CustomAnimator : UpdateMonoBehaviour
{
    [SerializeField] private BakedAnimSO bakedAnimSO;
    [SerializeField] private GameObject targetObj;

    [SerializeField, EditorReadOnly] private Transform[] targetTransforms;
    [SerializeField, EditorReadOnly] private BakedAnimClip currentClip;

    [SerializeField, EditorReadOnly] private float frameId;
    [SerializeField, EditorReadOnly] private bool isPlaying;

#if UNITY_EDITOR
    public GameObject TargetObj => targetObj;
    public Transform[] TargetTransforms => targetTransforms;
#endif


    private void OnValidate()
    {
        if (bakedAnimSO == null || targetObj == null) return;

        if (TryGetComponent(out CustomAnimBaker animBaker))
        {
            bakedAnimSO = animBaker.BakedAnimSO;
        }

        ReloadAnimation();

        targetTransforms = targetObj.transform.GetChildrenRecursively(true).ToArray();
    }
    public void ReloadAnimation()
    {
        currentClip = bakedAnimSO.Value;
    }

    [InspectorButton("[Play]")]
    private void Play()
    {
        isPlaying = true;
    }
    [InspectorButton("[Stop]")]
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

        currentClip.ApplyToTargetTransforms(targetTransforms, frameId);
    }
}