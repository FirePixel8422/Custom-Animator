


/// <summary>
/// An animation datatype acting as a bone, storing bound transform "TransformId", what type of transformations to use "Flags" and "FrameOffset" for loop calculations
/// </summary>
[System.Serializable]
public struct BakedAnimTrack
{
    public int TransformId;
    public int FrameOffset;
    public TransformationFlags Flags;

    public BakedAnimTrack(int transformId, int frameOffset, TransformationFlags flags)
    {
        TransformId = transformId;
        FrameOffset = frameOffset;
        Flags = flags;
    }


    // Split frame offset into offsetPos, offsetRot and OffsetScale to allow the global arrays to be smaller.
    // If bone 1 uses scale but bone 2 doesnt, but bone 3 does, write scale data of bone 3 to slot 2 (next available slot)
}