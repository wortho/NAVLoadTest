using System;

namespace Microsoft.Dynamics.Nav.TestUtilities
{
    /// <summary>
    /// Simple thread safe random number generator
    /// </summary>
    public class SafeRandom
    {
        private static readonly Object RandLock = new object();
        private static readonly Random Random = new Random();

        public static int GetRandomNext(int maxValue)
        {
            lock (RandLock)
            {
                return Random.Next(maxValue);
            }
        }

        public static int GetRandomNext(int minValue, int maxValue)
        {
            lock (RandLock)
            {
                return Random.Next(minValue, maxValue);
            }
        }
    }
}