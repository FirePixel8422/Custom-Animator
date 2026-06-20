using UnityEngine;



public class CustomAnimator : UpdateMonoBehaviour
{
    [SerializeField] private Transform[] transforms;
    [SerializeField] private BakedAnimClip currentClip;

    [SerializeField] private int frameId;



    [InspectorButton("Play")]
    private void Play()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif

        int clipTrackCount = currentClip.Length;

        DebugLogger.Throw("Transform count isnt equal to baked clip length", clipTrackCount != transforms.Length);

        for (int i = 0; i < clipTrackCount; i++)
        {
            currentClip.ApplyToTargetTransform(transforms[i], i, frameId);
        }
    }
}