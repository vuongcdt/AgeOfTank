namespace Utilities
{
    public class ENUMS
    {
        public enum CharacterTypeClass
        {
            Fighter = 0,
            Hunter,
            Warrior,
            FighterEnemy,
            HunterEnemy,
            WarriorEnemy,
        }

        public enum CharacterType
        {
            Player,
            Enemy,
        }
        public enum Layer
        {
            Player = 6,
            Enemy,
            WarriorPlayer,
            WarriorEnemy,
            SameType
        }
    }
}

