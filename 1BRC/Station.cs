using System.Collections.Immutable;

public class Station
{
    public string? Name { get; set; }
    public decimal Min { get; private set; } = decimal.MaxValue;
    public decimal Max { get; set; } = decimal.MinValue;
    public decimal Mean => Sum / NumberOfValues;
    private int NumberOfValues { get; set; } = 0;
    private decimal Sum { get; set; } = 0;

    public void AddValue(decimal value)
    {
        AddMin(value);
        AddMax(value);
        Sum += value;
        NumberOfValues++;
    }

    private void AddMin(decimal value)
    {
        if (value < Min)
            Min = value;
    }

    private void AddMax(decimal value)
    {
        if (value > Max)
            Max = value;
    }

    public override string ToString() => $"{Name}={Min}/{Mean}/{Max}";
}