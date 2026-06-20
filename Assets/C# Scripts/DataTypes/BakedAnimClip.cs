using Unity.Mathematics;
using UnityEngine;



[System.Serializable]
public class BakedAnimClip
{
    // One Track for every Bone/GameObject in the animation
    public BakedAnimTrack[] Tracks;
    public float FrameDuration;
    public float FrameCount;

    // All transformation data for all tracks, packed into one array
    public float3[] Positions;
    public quaternion[] Rotations;
    public float3[] Scales;
    public int TrackCount => Tracks.Length;



    public BakedAnimClip(int trackCount, int frameCount, float frameDuration)
    {
        Tracks = new BakedAnimTrack[trackCount];
        for (int i = 0; i < trackCount; i++)
        {
            Tracks[i].Flags = (TransformationFlags)255;
            Tracks[i].FrameOffset = frameCount * i;
        }

        int track_X_FrameCount = trackCount * frameCount;

        Positions = new float3[track_X_FrameCount];
        Rotations = new quaternion[track_X_FrameCount];
        Scales = new float3[track_X_FrameCount];

        FrameDuration = frameDuration;
        FrameCount = frameCount;
    }

    /// <summary>
    /// Write transformations from current frameId into baked anim arrays
    /// </summary>
    public void WriteTransformationData(Transform[] transforms, int frameCount, int frameId)
    {
        int trackCount = TrackCount;
        for (int trackId = 0; trackId < trackCount; trackId++)
        {
            int transformationIndex = trackId * frameCount + frameId;

            transforms[trackId].GetLocalPositionAndRotation(out Vector3 pos, out Quaternion rot);
            Positions[transformationIndex] = pos;
            Rotations[transformationIndex] = rot;
            Scales[transformationIndex] = transforms[trackId].localScale;
        }
    }

    /// <summary>
    /// Apply current animation frame transformation data to target transform
    /// </summary>
    public void ApplyToTargetTransforms(Transform[] transforms, float frameId)
    {
        int trackCount = TrackCount;
        for (int i = 0; i < trackCount; i++)
        {
            int transformationIndexA = Tracks[i].FrameOffset + (int)(frameId);
            int transformationIndexB = Tracks[i].FrameOffset + frameId >= FrameCount - 1 ? 0 : (int)math.ceil(frameId);
            float t = frameId - (int)frameId;
            Transform transform = transforms[i];

            switch (Tracks[i].Flags)
            {
                case TransformationFlags.Position:
                    transform.localPosition = math.lerp(Positions[transformationIndexA], Positions[transformationIndexB], t);
                    break;

                case TransformationFlags.Rotation:
                    transform.localRotation = math.slerp(Rotations[transformationIndexA], Rotations[transformationIndexB], t);
                    break;

                case TransformationFlags.Scale:
                    transform.localScale = math.lerp(Scales[transformationIndexA], Scales[transformationIndexB], t);
                    break;

                case TransformationFlags.Position | TransformationFlags.Rotation:
                    transform.SetLocalPositionAndRotation(
                        math.lerp(Positions[transformationIndexA], Positions[transformationIndexB], t),
                        math.slerp(Rotations[transformationIndexA], Rotations[transformationIndexB], t));
                    break;

                case TransformationFlags.Position | TransformationFlags.Scale:
                    transform.localPosition = math.lerp(Positions[transformationIndexA], Positions[transformationIndexB], t);
                    transform.localScale = math.lerp(Scales[transformationIndexA], Scales[transformationIndexB], t);
                    break;

                case TransformationFlags.Rotation | TransformationFlags.Scale:
                    transform.localRotation = math.slerp(Rotations[transformationIndexA], Rotations[transformationIndexB], t);
                    transform.localScale = math.lerp(Scales[transformationIndexA], Scales[transformationIndexB], t);
                    break;

                // TransformationFlags.Position | TransformationFlags.Rotation | TransformationFlags.Scale
                case (TransformationFlags)255:
                    DebugLogger.Log(transformationIndexA + ", " + transformationIndexA);

                    transform.SetLocalPositionAndRotation(
                        math.lerp(Positions[transformationIndexA], Positions[transformationIndexB], t),
                        math.slerp(Rotations[transformationIndexA], Rotations[transformationIndexB], t));
                    transform.localScale = math.lerp(Scales[transformationIndexA], Scales[transformationIndexB], t);
                    break;

                case TransformationFlags.None:
                default:
                    return;
            }
        }
    }
}