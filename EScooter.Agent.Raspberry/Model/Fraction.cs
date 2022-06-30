namespace EScooter.Agent.Raspberry.Model;

public record struct Fraction : IComparable<Fraction>
{
    public double Base100ValueRounded => Math.Round(Base100Value, 2);

    public double Base100Value { get; }

    public double Base1Value => Base100Value / 100;

    private Fraction(double percentage)
    {
        if (percentage < 0 || percentage > 100)
        {
            throw new ArgumentException("Percentage must be between 0 and 100, inclusive.");
        }
        Base100Value = percentage;
    }

    public static Fraction FromFraction(double fraction)
    {
        if (fraction < 0 || fraction > 1)
        {
            throw new ArgumentException("Fraction must be between 0 and 1, inclusive.");
        }
        return new(fraction * 100);
    }

    public static Fraction FromPercentage(double percentage) => new(percentage);

    public int CompareTo(Fraction other) => Base100Value.CompareTo(other.Base100Value);

    public static bool operator >(Fraction a, Fraction b) => a.CompareTo(b) > 0;

    public static bool operator <(Fraction a, Fraction b) => a.CompareTo(b) < 0;

    public static bool operator >=(Fraction a, Fraction b) => a.CompareTo(b) >= 0;

    public static bool operator <=(Fraction a, Fraction b) => a.CompareTo(b) <= 0;
}
