namespace HalgarisRPGLoot.Settings.CustomTypes;

public class WeightedObject<TType> where TType : class
{
    public TType Object { get; set; } = default!;
    public int Weight { get; set; } = 1;
}