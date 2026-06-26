using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;


[System.Serializable]
public struct BakedAnimClip
{
    // One Track for every Bone/GameObject in the animation
    public BakedAnimTrack[] Tracks;
    public float FrameDuration;
    public int FrameCount;

    // All transformation data for all tracks, packed into one array
    public Vector3[] Positions;
    public Quaternion[] Rotations;
    public Vector3[] Scales;
    public readonly int TrackCount => Tracks.Length;



    public BakedAnimClip(int trackCount, int frameCount, float frameDuration)
    {
        Tracks = new BakedAnimTrack[trackCount];
        for (int i = 0; i < trackCount; i++)
        {
            Tracks[i].Flags = TransformationFlags.Position | TransformationFlags.Rotation | TransformationFlags.Scale;
            Tracks[i].FrameOffset = frameCount * i;
        }

        int maxArrayLength = trackCount * frameCount;

        Positions = new Vector3[maxArrayLength];
        Rotations = new Quaternion[maxArrayLength];
        Scales = new Vector3[maxArrayLength];

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
            Positions[transformationIndex] = (Vector3)pos;
            Rotations[transformationIndex] = new Quaternion(rot.x, rot.y, rot.z, rot.w);
            Scales[transformationIndex] = (Vector3)transforms[i].localScale;
        }
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

        Vector3 pos = default;
        Quaternion rot = default;
        Vector3 scale = default;

        for (int i = 0; i < trackCount; i++)
        {
            Transform transform = transforms[i];
            BakedAnimTrack track = Tracks[i];

            int offset = track.FrameOffset;
            int idxA = frameId + offset;
            int idxB = passedLastAnimFrame ? offset : idxA + 1;

            TransformationFlags flags = track.Flags;

            bool hasPos = (flags & TransformationFlags.Position) != 0;
            bool hasRot = (flags & TransformationFlags.Rotation) != 0;
            bool hasScale = (flags & TransformationFlags.Scale) != 0;

            if (hasPos)
                pos = Lerp(Positions[idxA], Positions[idxB], t);

            if (hasRot)
                rot = Lerp(Rotations[idxA], Rotations[idxB], t);

            if (hasScale)
                scale = Lerp(Scales[idxA], Scales[idxB], t);

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

            if (hasScale)
            {
                transform.localScale = scale;
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