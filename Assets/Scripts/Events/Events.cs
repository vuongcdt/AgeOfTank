namespace Events
{
    public class Events
    {
        public struct InitCharacter
        {
            public CONSTANTS.CardCharacterType Type;

            public InitCharacter(CONSTANTS.CardCharacterType type)
            {
                Type = type;
            }
        }
    }
}