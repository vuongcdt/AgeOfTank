namespace Utilities
{
    public static class Utils
    {
        public static float GetDurationMoveToTarget(float currentPos,float start,float end,float durationMove)
        {
            return (currentPos - end) / (start - end) * durationMove;
        }
    }
}