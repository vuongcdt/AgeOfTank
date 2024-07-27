namespace Events
{
    public class Events
    {
        public struct InitCharacter
        {
            public ENUMS.CharacterTypeClass TypeClass;

            public InitCharacter(ENUMS.CharacterTypeClass typeClass)
            {
                TypeClass = typeClass;
            }
        }
        public struct InitConfigSystemEvent
        {
        }
    }
}