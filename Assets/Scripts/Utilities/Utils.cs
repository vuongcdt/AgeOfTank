using UnityEngine;

namespace Utilities
{
    public static class Utils
    {
        public static float GetDurationMoveToTarget(float currentX, float startX, float endX, float durationMove)
        {
            return (currentX - endX) / (startX - endX) * durationMove;
        }

        public static float GetDurationMoveToPoint(float currentX, float targetX, float startX, float endX,
            float durationMove)
        {
            return Mathf.Abs((currentX - targetX) / (startX - endX) * durationMove);
        }
    }
}