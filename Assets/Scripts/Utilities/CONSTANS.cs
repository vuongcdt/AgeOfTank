namespace Utilities
{
    public class CONSTANS
    {
        public struct Tag
        {
            public const string Player = nameof(Player);
            public const string Enemy = nameof(Enemy);
            public const string CircleCollider = nameof(CircleCollider);

            public const string HunterCollider = nameof(HunterCollider);
            public const string SameTypeCollider = nameof(SameTypeCollider);
            public const string SameTypeColliderHunter = nameof(SameTypeColliderHunter);
            public const string WarriorColliderPlayer = nameof(WarriorColliderPlayer);
            public const string WarriorColliderEnemy = nameof(WarriorColliderEnemy);
            public const string StartBarPlayer = nameof(StartBarPlayer);
            public const string StartBarEnemy = nameof(StartBarEnemy);
            public const string PlayerBar = nameof(PlayerBar);
        }
        
        public struct LayerMask
        {
            public const string WarriorPlayer = nameof(WarriorPlayer);
            public const string WarriorEnemy = nameof(WarriorEnemy);
            public const string SameType = nameof(SameType);
        }
    }
}