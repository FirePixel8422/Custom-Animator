using System;



[Flags]
public enum TransformationFlags : int
{
    Position = 1,
    Rotation = 2,
    Scale = 4,

    All = Position | Rotation | Scale,
}