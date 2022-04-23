using System.Collections.Generic;

public class Config
{
    public static int PrefferedHeight = 768;
    public static int PrefferedWidth = 1024;
    public static string TitleGame = "River Ride - Mikołaj Godzicki";
    public static int BGCount = 2;
    public static float BGMovementSpeed = 4f;
    public static string ContentRootDirectory = "Content";
    public static float PlaneMovementSpeed = 5f;
    public static float ProjectileSpeed = 6f;
    public static float EnemyMovementSpeed = 5f;
    public static int MinimumObjectPos = 200;
    public static int MaximumObjectPos = 750;
    public static float FuelSpeed = 0.1f;
    public static float FuelBarrelSpeed = 4f;
    public static int levelUpScore = 2500;

    public static List<int> Points = new List<int> { 50, 100, 150, 250};
    public static List<int> Fuel = new List<int> { 10, 15, 20, 25};
}