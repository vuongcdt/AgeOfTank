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

    public struct AvoidObstacleSystemEvent
    {
        public Collider2D ColliderObstacle;
        public Actor ActorRun;
    }
}