using System;



[Flags]
public enum TransformationFlags : byte
{
    None = 0,
    Position = 1,
    Rotation = 2,
    Scale = 4,
}