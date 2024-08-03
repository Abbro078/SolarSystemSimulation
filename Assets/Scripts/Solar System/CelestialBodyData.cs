using System.Collections.Generic;

public static class CelestialBodyData
{
    public static readonly Dictionary<string, float> RotationalPeriods = new Dictionary<string, float>()
    {
        {"Sun", 25.0f * 24 * 60 * 60 / 2000},
        {"Mercury", 58.6f * 24 * 60 * 60 / 2000},
        {"Venus", -243.0f * 24 * 60 * 60 / 2000},
        {"Earth", 24.0f * 60 * 60 / 2000},
        {"Mars", 24.6f * 60 * 60 / 2000},
        {"Jupiter", 9.9f * 60 * 60 / 2000},
        {"Saturn", 10.7f * 60 * 60 / 2000},
        {"Uranus", -17.2f * 60 * 60 / 2000},
        {"Neptune", 16.1f * 60 * 60 / 2000},
        {"Moon", 27.3f * 24 * 60 * 60 / 2000}
    };
}