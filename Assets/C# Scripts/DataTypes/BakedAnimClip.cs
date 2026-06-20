using Unity.Mathematics;
using UnityEngine;



[System.Serializable]
public struct BakedAnimClip
{
    // One Track for every Bone/GameObject in the animation
    public BakedAnimTrack[] Tracks;

    // All transformation data for all tracks, packed into one array
    public float3[] Positions;
    public quaternion[] Rotations;
    public float3[] Scales;    
    public readonly int Length => Tracks.Length;



    public BakedAnimClip(int trackCount, int frameCount)
    {
        Tracks = new BakedAnimTrack[trackCount];

        int track_X_FrameCount = trackCount * frameCount;

        Positions = new float3[track_X_FrameCount];
        Rotations = new quaternion[track_X_FrameCount];
        Scales = new float3[track_X_FrameCount];
    }

    /// <summary>
    /// Apply current animation frame transformation data to target transform
    /// </summary>
    public readonly void ApplyToTargetTransform(Transform t, int trackId, int frameId)
    {
        int transformationIndex = Tracks[trackId].FrameOffset + frameId;

        switch (Tracks[trackId].Flags)
        {
            case TransformationFlags.Position:
                t.position = Positions[transformationIndex];
                break;

            case TransformationFlags.Rotation:
                t.rotation = Rotations[transformationIndex];
                break;

            case TransformationFlags.Scale:
                t.localScale = Scales[transformationIndex];
                break;

            case TransformationFlags.Position | TransformationFlags.Rotation:
                t.SetPositionAndRotation(Positions[transformationIndex], Rotations[transformationIndex]);
                break;

            case TransformationFlags.Position | TransformationFlags.Scale:
                t.position = Positions[transformationIndex];
                t.localScale = Scales[transformationIndex];
                break;

            case TransformationFlags.Rotation | TransformationFlags.Scale:
                t.rotation = Rotations[transformationIndex];
                t.localScale = Scales[transformationIndex];
                break;

            case TransformationFlags.Position | TransformationFlags.Rotation | TransformationFlags.Scale:
                t.SetPositionAndRotation(Positions[transformationIndex], Rotations[transformationIndex]);
                t.localScale = Scales[transformationIndex];
                break;

            case TransformationFlags.None:
            default:
                return;
        }
    }
}