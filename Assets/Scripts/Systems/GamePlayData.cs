using UnityEngine;

namespace Systems
{
    [CreateAssetMenu(menuName = "GamePlayData")]
    public class GamePlayData:ScriptableObject
    {
        public Vector3 pointSource, pointTarget;
        public int durationMove = 10;
        public float distanceHit = 0.2f;
        public float attackTime = 1f;
        public float[] healths;
        public float[] damages;
    }
}