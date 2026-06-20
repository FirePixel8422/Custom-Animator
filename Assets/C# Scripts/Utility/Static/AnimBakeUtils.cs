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

        for (int frameId = 0; frameId < frameCount; frameId++)
        {
            float t = (float)frameId / frameCount * clip.length;
            clip.SampleAnimation(obj, t);

            bakedClip.WriteTransformationData(transforms, frameCount, frameId);
        }

        // Reset
        clip.SampleAnimation(obj, 0);
    }
}