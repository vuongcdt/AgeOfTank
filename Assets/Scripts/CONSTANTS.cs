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
        public const string HunterColliderPlayer = nameof(HunterColliderPlayer);
        public const string SameTypeCollider = nameof(SameTypeCollider);
        public const string WarriorColliderPlayer = nameof(WarriorColliderPlayer);
        public const string HunterColliderEnemy = nameof(HunterColliderEnemy);
        public const string WarriorColliderEnemy = nameof(WarriorColliderEnemy);
    }

    public enum Layer
    {
        Player = 6,
        Enemy,
    }
}