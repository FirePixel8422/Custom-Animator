


/// <summary>
/// An animation datatype acting as a bone, storing
/// </summary>
[System.Serializable]
public struct BakedAnimTrack
{
    public TransformationFlags Flags;
    public int FrameOffset;


    // Split frame offset into offsetPos, offsetRot and OffsetScale to allow the global arrays to be smaller.
    // If bone 1 uses scale but bone 2 doesnt, but bone 3 does, write scale data of bone 3 to slot 2 (next available slot)
}