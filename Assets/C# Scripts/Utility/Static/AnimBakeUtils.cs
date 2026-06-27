using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;


/// <summary>
/// Static class responsible for baking an <see cref="AnimationClip"/> to into a <see cref="BakedAnimClip"/> in a hyper optimized wat for usage in the <see cref="CustomAnimator"/>
/// </summary>
public static class AnimBakeUtils
{
    public static void Bake(AnimationClip clip, GameObject obj, Transform[] transforms, AnimBakeSettings bakeSettings, out BakedAnimClip bakedClip)
    {
        int trackCount = transforms.Length;
        int frameCount = Mathf.CeilToInt(clip.length * bakeSettings.FrameRate);

        Stopwatch sw = Stopwatch.StartNew();
        DebugLogger.Log($">>Baking<< Clip length:" +
            $" {clip.length}s," +
            $" total tracks (transforms): {trackCount}," +
            $" frameRate: {bakeSettings.FrameRate}," +
            $" generated frames: {frameCount}");

        AnimClipRecording clipRecording = new AnimClipRecording(trackCount, frameCount, clip.length / frameCount);

        // Play animation and record it
        for (int i = 0; i < frameCount; i++)
        {
            float t = (float)i / frameCount * clip.length;

            clip.SampleAnimation(obj, t);

            clipRecording.RecordTransformationData(transforms, frameCount, i);
        }
        // Reset
        clip.SampleAnimation(obj, 0);

        // Optimise recorded clip and bake it into a BakedAnimClip.
        OptimizeAndBakeAnimation(in clipRecording, bakeSettings, out bakedClip);

        DebugLogger.Log($"Bake Finished," +
            $" took: {sw.ElapsedTicks} ticks ({sw.ElapsedMilliseconds} ms)");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void OptimizeAndBakeAnimation(in AnimClipRecording clipRecording, AnimBakeSettings bakeSettings, out BakedAnimClip bakedClip)
    {
        DebugLogger.Log($">>Optimizing baked clip<<");

        int trackCount = clipRecording.TrackCount;
        int frameCount = clipRecording.FrameCount;

        // Early exit if optimise is disabled
        if (!bakeSettings.OptimizeClipData)
        {
            bakedClip = new BakedAnimClip(
                clipRecording.Tracks.ToArray(),
                clipRecording.Positions.ToArray(),
                clipRecording.Rotations.ToArray(),
                clipRecording.Scales.ToArray(),
                clipRecording.FrameDuration,
                clipRecording.FrameCount,
                bakeSettings.DoLerpSmoothing);

            DebugLogger.Log($"Optimise Finished," +
                $" tracks left (transforms): {clipRecording.TrackCount}/{trackCount}," +
                $" positions left: {trackCount}/{trackCount}," +
                $" rotations left: {trackCount}/{trackCount}," +
                $" scales left: {trackCount}/{trackCount},");

            return;
        }

        int skippedEmptyPositions = 0;
        int skippedEmptyRotations = 0;
        int skippedEmptyScales = 0;

        for (int i = trackCount - 1; i >= 0; i--)
        {
            int startId = i * frameCount;

            ValueChangeMeter<Vector3> posChangeMeter = new ValueChangeMeter<Vector3>(clipRecording.Positions[startId], frameCount);
            ValueChangeMeter<Quaternion> rotChangeMeter = new ValueChangeMeter<Quaternion>(clipRecording.Rotations[startId], frameCount);
            ValueChangeMeter<Vector3> scaleChangeMeter = new ValueChangeMeter<Vector3>(clipRecording.Scales[startId], frameCount);

            float dist;

            // Check transformation changes in the recorded frames.
            for (int i2 = 0; i2 < frameCount; i2++)
            {
                int targetId = i * frameCount + i2;

                dist = Vector3.Distance(posChangeMeter.LastNewValue, clipRecording.Positions[targetId]);
                if (dist > bakeSettings.Epsilons.Position)
                {
                    posChangeMeter.RecordChange(i2, dist);
                }

                dist = Quaternion.Angle(rotChangeMeter.LastNewValue, clipRecording.Rotations[targetId]);
                if (dist > bakeSettings.Epsilons.Rotation)
                {
                    rotChangeMeter.RecordChange(i2, dist);
                }

                dist = Vector3.Distance(scaleChangeMeter.LastNewValue, clipRecording.Scales[targetId]);
                if (dist > bakeSettings.Epsilons.Scale)
                {
                    scaleChangeMeter.RecordChange(i2, dist);
                }
            }

            // Check if target track is completely empty, if so remove it from the animation
            if (posChangeMeter.HasNoChanges && rotChangeMeter.HasNoChanges && scaleChangeMeter.HasNoChanges)
            {
                clipRecording.RemoveTrackSwapBack(i);

                skippedEmptyPositions += 1;
                skippedEmptyRotations += 1;
                skippedEmptyScales += 1;

                continue;
            }

            BakedAnimTrack track = clipRecording.Tracks[i];

            // Check if target transformation (pos, rot, scale) contributes to animation, if not remove it from the animation
            if (posChangeMeter.HasNoChanges)
            {
                track.Flags &= ~TransformationFlags.Position;
                skippedEmptyPositions += 1;
            }
            if (rotChangeMeter.HasNoChanges)
            {
                track.Flags &= ~TransformationFlags.Rotation;
                skippedEmptyRotations += 1;
            }
            if (scaleChangeMeter.HasNoChanges)
            {
                track.Flags &= ~TransformationFlags.Scale;
                skippedEmptyScales += 1;
            }

            clipRecording.Tracks[i] = track;
        }

        // Check if target transformation (pos, rot, scale) has ANY contributers to the animation, if not remove entire channel from the animation
        if (skippedEmptyPositions == trackCount)
        {
            clipRecording.Positions.Clear();
        }
        if (skippedEmptyRotations == trackCount)
        {
            clipRecording.Rotations.Clear();
        }
        if (skippedEmptyScales == trackCount)
        {
            clipRecording.Scales.Clear();
        }


        bakedClip = new BakedAnimClip(
            clipRecording.Tracks.ToArray(),
            clipRecording.Positions.ToArray(),
            clipRecording.Rotations.ToArray(),
            clipRecording.Scales.ToArray(),
            clipRecording.FrameDuration,
            clipRecording.FrameCount,
            bakeSettings.DoLerpSmoothing);

        DebugLogger.Log($"Optimise Finished," +
            $" tracks left (transforms): {clipRecording.TrackCount}/{trackCount}," +
            $" positions left: {trackCount - skippedEmptyPositions}/{trackCount}," +
            $" rotations left: {trackCount - skippedEmptyRotations}/{trackCount}," +
            $" scales left: {trackCount - skippedEmptyScales}/{trackCount},");
    }

    public struct ValueChangeMeter<T>
    {
        public T LastNewValue;
        public float[] Changes;
        public float TotalChange;

        public readonly bool HasNoChanges => TotalChange == 0;

        public ValueChangeMeter(T startValue, int frameCount)
        {
            LastNewValue = startValue;
            Changes = new float[frameCount];
            TotalChange = 0;
        }

        public void RecordChange(int frameId, float change)
        {
            TotalChange += change;
            Changes[frameId] = change;
        }
    }
}