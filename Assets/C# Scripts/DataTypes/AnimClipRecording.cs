using System.Collections.Generic;
using System.Linq;
using UnityEngine;



[System.Serializable]
public class AnimClipRecording
{
    public List<BakedAnimTrack> Tracks;
    public int TrackCount => Tracks.Count;

    public List<Vector3> Positions;
    public List<Quaternion> Rotations;
    public List<Vector3> Scales;

    [field: SerializeField] public float FrameDuration { get; private set; }
    [field: SerializeField] public int FrameCount { get; private set; }


    public AnimClipRecording(int trackCount, int frameCount, float frameDuration)
    {
        Tracks = new List<BakedAnimTrack>(trackCount);
        for (int i = 0; i < trackCount; i++)
        {
            Tracks.Add(new BakedAnimTrack(i, TransformationFlags.All, frameCount * i));
        }

        int maxArrayLength = trackCount * frameCount;

        Positions = new Vector3[maxArrayLength].ToList();
        Rotations = new Quaternion[maxArrayLength].ToList();
        Scales = new Vector3[maxArrayLength].ToList();

        FrameDuration = frameDuration;
        FrameCount = frameCount;
    }

    /// <summary>
    /// Record (Bake/Write) transformations from current frameId into baked anim arrays
    /// </summary>
    public void RecordTransformationData(Transform[] transforms, int frameCount, int frameId)
    {
        int trackCount = TrackCount;
        for (int i = 0; i < trackCount; i++)
        {
            int transformationIndex = i * frameCount + frameId;

            transforms[i].GetLocalPositionAndRotation(out Vector3 pos, out Quaternion rot);
            Positions[transformationIndex] = pos;
            Rotations[transformationIndex] = rot;
            Scales[transformationIndex] = transforms[i].localScale;
        }
    }
}