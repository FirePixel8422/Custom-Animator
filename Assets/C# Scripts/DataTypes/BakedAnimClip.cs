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



    public BakedAnimClip(BakedAnimTrack[] tracks, Vector3[] positions, Quaternion[] rotations, Vector3[] scales, float frameDuration, int frameCount)
    {
        Tracks = tracks;
        TrackCount = tracks.Length;

        Positions = positions;
        Rotations = rotations;
        Scales = scales;

        FrameDuration = frameDuration;
        FrameCount = frameCount;
    }

    /// <summary>
    /// Apply current animation frame transformation data to target transform
    /// </summary>
    public readonly void ApplyToTargetTransforms(Transform[] transforms, float playbackTime)
    {
        int trackCount = TrackCount;
        int frameId = (int)playbackTime;
        //bool passedLastAnimFrame = playbackTime >= FrameCount - 1;

        //float t = playbackTime - (int)playbackTime;

        Transform transform;

        Vector3 pos = default;
        Quaternion rot = default;

        for (int i = 0; i < trackCount; i++)
        {
            BakedAnimTrack track = Tracks[i];
            transform = transforms[track.TransformId];

            int offset = track.FrameOffset;
            int idxA = frameId + offset;
            //int idxB = passedLastAnimFrame ? offset : idxA + 1;

            TransformationFlags flags = track.Flags;

            bool hasPos = (flags & TransformationFlags.Position) != 0;
            bool hasRot = (flags & TransformationFlags.Rotation) != 0;
            bool hasScale = (flags & TransformationFlags.Scale) != 0;

            if (hasPos)
                //pos = Lerp(positions[idxA], positions[idxB], t);
                pos = Positions[idxA];

            if (hasRot)
                //rot = Lerp(rotations[idxA], rotations[idxB], t);
                rot = Rotations[idxA];

            if (hasScale)
                //transform.localScale = Lerp(scales[idxA], scales[idxB], t);
                transform.localScale = Scales[idxA];

            if (hasPos && hasRot)
            {
                transform.SetLocalPositionAndRotation(pos, rot);
            }
            else if (hasPos)
            {
                transform.localPosition = pos;
            }
            else if (hasRot)
            {
                transform.localRotation = rot;
            }
        }
    }
}