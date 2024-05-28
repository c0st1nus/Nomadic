using System;

public class Player
{
    public Player()
    {
        Strength = 0;
        Intellect = 0;
        Charisma = 0;
        Agility = 0;
        Luck = 0;
        Health = 0;
        Reputation = 0;
    }

    public float Strength;
    public float Intellect;
    public float Charisma;
    public float Agility;
    public float Luck;
    public float Health;
    public float Reputation;

    public double this[playerStats vStat]
    {
        get
        {
            switch (vStat)
            {
                case playerStats.Agility:
                    return Agility;
                case playerStats.Charisma:
                    return Charisma;
                case playerStats.Health:
                    return Health;
                case playerStats.Intellect:
                    return Intellect;
                case playerStats.Reputation:
                    return Reputation;
                case playerStats.Luck:
                    return Luck;
                case playerStats.Strength:
                    return Strength;
                default:
                    throw new Exception("Unknown stat");
            }
        }
    }
}
public enum playerStats {
    Strength,
    Intellect,
    Charisma,
    Agility,
    Luck,
    Health,
    Reputation
};