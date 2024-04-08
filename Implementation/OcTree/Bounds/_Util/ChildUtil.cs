namespace PositionEvents.Implementation.OcTree.Bounds
{
    public static class ChildUtil
    {
        /// <summary>
        /// Determines whether the given <paramref name="value"/> is below or above 0.5 inside the given 
        /// <paramref name="error"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="error"></param>
        /// <returns>0 if within the given <paramref name="error"/> of 0.5. -1 if below and 1 if greater.</returns>
        public static int GetDirection(float value, float error = 1e-2f)
        {
            if (value < 0.5f - error / 2)
            {
                return -1;
            }
            if (value <= 0.5f + error / 2)
            {
                return 0;
            }
            return 1;
        }
    }
}
