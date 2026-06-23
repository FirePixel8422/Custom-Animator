using System;



[Flags]
public enum TransformationFlags : byte
{
    Position = 2,
    Rotation = 4,
    Scale = 8,
}