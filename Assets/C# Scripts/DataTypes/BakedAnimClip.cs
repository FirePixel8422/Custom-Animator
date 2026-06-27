using System.Runtime.CompilerServices;
using UnityEngine;


[System.Serializable]
public struct BakedAnimClip
{
    // One Track for every Bone/Transform in the animation
    [SerializeField] private BakedAnimTrack[] tracks;
    public readonly int TrackCount => tracks.Length;

    // All transformation data for all tracks, packed into one array
    [SerializeField] private Vector3[] positions;
    [SerializeField] private Quaternion[] rotations;
    [SerializeField] private Vector3[] scales;

    [field: SerializeField] public float FrameDuration { get; private set; }
    [field: SerializeField] public int FrameCount { get; private set; }



    public BakedAnimClip(BakedAnimTrack[] tracks, Vector3[] positions, Quaternion[] rotations, Vector3[] scales, float frameDuration, int frameCount)
    {
        this.tracks = tracks;
        this.positions = positions;
        this.rotations = rotations;
        this.scales = scales;

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
        bool passedLastAnimFrame = playbackTime >= FrameCount - 1;

        float t = playbackTime - (int)playbackTime;

        Transform transform;

        Vector3 pos = default;
        Quaternion rot = default;

        for (int i = 0; i < trackCount; i++)
        {
            BakedAnimTrack track = tracks[i];
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
                pos = positions[idxA];

            if (hasRot)
                //rot = Lerp(rotations[idxA], rotations[idxB], t);
                rot = rotations[idxA];

            if (hasScale)
                //transform.localScale = Lerp(scales[idxA], scales[idxB], t);
                transform.localScale = scales[idxA];

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