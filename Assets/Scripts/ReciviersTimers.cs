namespace Assets.Scripts
{
    /// <summary>
    /// Data class used to save info for each line of times
    /// </summary>
    public class RecieversTime
    {
        public float FirstTime { get; }
        public float SecondTime { get; }
        public float ThirdTime { get; }

        public RecieversTime(float firstTime, float secondTime, float thirdTime)
        {
            FirstTime = firstTime;
            SecondTime = secondTime;
            ThirdTime = thirdTime;
        }
    }
}
