using Fire_Pixel.Utility;
using System.Data.Common;
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

        DebugLogger.Log($">>Baking<< clip length: {clip.length}, frameRate: {frameRate}, tracks: {trackCount}, frames: {frameCount}");

        bakedClip = new BakedAnimClip(trackCount, frameCount, clip.length / frameCount);

        for (int i = 0; i < frameCount; i++)
        {
            float t = (float)i / frameCount * clip.length;

            clip.SampleAnimation(obj, t);

            bakedClip.BakeTransformationData(transforms, frameCount, i);
        }

        // Reset
        clip.SampleAnimation(obj, 0);
    }
}