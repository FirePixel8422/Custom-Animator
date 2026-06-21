using System;



[Flags]
public enum TransformationFlags : byte
{
    None = 1,
    Position = 2,
    Rotation = 4,
    Scale = 8,
}