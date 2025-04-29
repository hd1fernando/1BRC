public class Station
{
    public string?  Name { get; set; }
    public List<float> Values = new();
    public float Mean() => Values.Average();

    public float Min() => Values.Min();

    public float Max() => Values.Max();

    public override string ToString()=> $"{Name}={Min()}/{Mean()}/{Max()}";
}