using System.Runtime.CompilerServices;
using UnityEngine;



/// <summary>
/// Static class responsible for baking 
/// </summary>
public static class AnimBakeUtils
{
    public static void Bake(AnimationClip clip, GameObject obj, int frameRate, out BakedAnimClip bakedClip)
    {
        int trackCount = GetRecursiveChildCount(obj.transform);
        int frameCount = Mathf.FloorToInt(clip.length / frameRate);

        bakedClip = new BakedAnimClip(trackCount, frameCount);

        for (int i = 0; i < frameCount; i++)
        {
            float t = i / frameCount;
            clip.SampleAnimation(obj, t);
        }

    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //
    //
    //
    //
    //
    //
    private static int GetRecursiveChildCount(Transform root)
    {
        int count = 0;

        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);

            count += 1;
            count += GetRecursiveChildCount(child);
        }

        return count;
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //
    //
    //
    //
    //
    //
    private static void StoreTransformData(GameObject obj)
    {

    }
}