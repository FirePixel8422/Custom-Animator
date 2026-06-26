using System.Diagnostics;
using UnityEngine;


/// <summary>
/// Static class responsible for baking 
/// </summary>
public static class AnimBakeUtils
{
    public static void Bake(AnimationClip clip, GameObject obj, Transform[] transforms, int frameRate, out BakedAnimClip bakedClip)
    {
        int trackCount = transforms.Length;
        int frameCount = Mathf.CeilToInt(clip.length * frameRate);

        Stopwatch sw = Stopwatch.StartNew();
        DebugLogger.Log($">>Baking<< Clip length: {clip.length}s, tracks (transforms): {trackCount}, frameRate: {frameRate}, frames: {frameCount}");

        bakedClip = new BakedAnimClip(trackCount, frameCount, clip.length / frameCount);

        // Play animation and record it
        for (int i = 0; i < frameCount; i++)
        {
            float t = (float)i / frameCount * clip.length;

            clip.SampleAnimation(obj, t);

            bakedClip.RecordTransformationData(transforms, frameCount, i);
        }

        // Reset
        clip.SampleAnimation(obj, 0);

        DebugLogger.Log($"Bake Finished, took: {sw.ElapsedTicks} ticks ({sw.ElapsedMilliseconds} ms)");
    }

    private static void OptimizeBakedAnimation(int frameCount)
    {
        DebugLogger.Log($">>Optimizing baked clip<<");


    }
}