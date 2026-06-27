using UnityEngine;



[System.Serializable]
public class AnimBakeSettings
{
    [field: SerializeField] public int FrameRate { get; private set; } = 2;

    [field: Tooltip("While enabled, smoothly blend from previous animation frame to the next. Comes with a small performance cost.")]
    [field: SerializeField] public bool DoLerpSmoothing { get; private set; } = true;

    [field: Tooltip("Skip baking tracks and their transform channels that dont contribute to the animation (pos, rot, scale)")]
    [field: WarningIf(ComparisonType.False, "It is highly recommended to turn OptimizeClipData on")]
    [field: SerializeField] public bool OptimizeClipData { get; private set; } = true;


    [field: Tooltip("What margin of values are considered 0 (distance, angle, distance)")]
    [field: ShowIf(nameof(OptimizeClipData))]
    [field: SerializeField] public Epsilons Epsilons { get; private set; } = new Epsilons(0.01f, 0.15f, 0.01f);

}

[System.Serializable]
public struct Epsilons
{
    public float Position;
    public float Rotation;
    public float Scale;

    public Epsilons(float position, float rotation, float scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}