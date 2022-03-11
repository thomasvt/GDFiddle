namespace GDFiddle.Ecs
{
    public struct Time
    {
        /// <summary>
        /// Elapsed time in seconds since previous frame.
        /// </summary>
        public float DeltaTime;
        /// <summary>
        /// Time in seconds since start.
        /// </summary>
        public float TotalTime;

        public Time(float totalTime, float deltaTime)
        {
            TotalTime = totalTime;
            DeltaTime = deltaTime;
        }
    }
}
