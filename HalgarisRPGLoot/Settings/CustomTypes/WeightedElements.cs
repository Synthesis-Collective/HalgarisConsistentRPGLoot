namespace HalgarisRPGLoot.Settings.CustomTypes;

public class WeightedElements<TType> where TType : class
{
    
    public TType Element { get; set; } = default!;
    public int Weight { get; set; } = 1;
}