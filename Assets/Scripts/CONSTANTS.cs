public class CONSTANTS
{
    public enum CardCharacterType
    {
        Fighter,
        Hunter,
        Warrior,
        FighterEnemy,
        HunterEnemy,
        WarriorEnemy,
    }

    public class Tag
    {
        public const string Player = nameof(Player);
        public const string Enemy = nameof(Enemy);
        public const string CircleCollider = nameof(CircleCollider);
    }

    public enum Layer
    {
        Player = 6,
        Enemy,
    }
}