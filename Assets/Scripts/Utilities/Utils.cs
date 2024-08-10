
namespace Utilities
{
    public static class Utils
    {
        public static float GetDurationMoveToTarget(float currentX, float startX, float endX, float durationMove)
        {
            return (currentX - endX) / (startX - endX) * durationMove;
        }
    }
}