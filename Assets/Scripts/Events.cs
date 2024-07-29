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
        public Vector3 Pos;
        public ENUMS.CharacterType Type;

        public ActorAttackPointEvent(Vector3 pos, ENUMS.CharacterType type)
        {
            Pos = pos;
            Type = type;
        }
    }

    public struct MoveToTargetEvent
    {
    }

    public struct AvoidObstacleSystemEvent
    {
        public Collider2D ColliderObstacle;
        public Actor ActorRun;
    }
}