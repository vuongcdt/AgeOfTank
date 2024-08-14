using UnityEngine;
using Utilities;

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
public struct MoveToCharacterAttack
{
}

public struct CharacterAttackPointEvent
{
    public Vector3 Position;
    public ENUMS.CharacterType Type;

    public CharacterAttackPointEvent(Vector3 position, ENUMS.CharacterType type)
    {
        Position = position;
        Type = type;
    }
}

public struct MoveToTargetEvent
{
}

public struct AttackEvent
{
}
