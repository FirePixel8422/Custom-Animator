using UnityEngine;


[System.Serializable]
public struct BakedAnimClip
{
    // One Track for every Bone/Transform in the animation
    [field: SerializeField] public BakedAnimTrack[] Tracks { get; private set; }
    [field: SerializeField] public int TrackCount { get; private set; }

    // All transformation data for all tracks, packed into one array
    [field: SerializeField] public Vector3[] Positions { get; private set; }
    [field: SerializeField] public Quaternion[] Rotations { get; private set; }
    [field: SerializeField] public Vector3[] Scales { get; private set; }

    [field: SerializeField] public float FrameDuration { get; private set; }
    [field: SerializeField] public int FrameCount { get; private set; }

    [field: SerializeField] public bool DoLerpSmoothing { get; private set; }


    public BakedAnimClip(BakedAnimTrack[] tracks, Vector3[] positions, Quaternion[] rotations, Vector3[] scales, float frameDuration, int frameCount, bool doLerpSmoothing)
    {
        Tracks = tracks;
        TrackCount = tracks.Length;

        Positions = positions;
        Rotations = rotations;
        Scales = scales;

        FrameDuration = frameDuration;
        FrameCount = frameCount;

        DoLerpSmoothing = doLerpSmoothing;
    }
}