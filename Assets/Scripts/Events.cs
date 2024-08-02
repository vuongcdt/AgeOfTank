using Controllers.NewGame;
using UnityEngine;
using Utilities;

namespace Events
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

    public struct ActorAttackPointEvent
    {
        public Vector3 Position;
        public ENUMS.CharacterType Type;

        public ActorAttackPointEvent(Vector3 position, ENUMS.CharacterType type)
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

    public struct AvoidObstacleSystemEvent
    {
        public Collider2D ColliderObstacle;
        public Actor ActorRun;
    }
}