using System.Threading;

namespace Microsoft.Dynamics.Nav.TestUtilities
{
    public class DelayTiming
    {
        public const int OpenFormDelay = 500;

        public const int EntryDelay = 400;

        public const int ThinkDelay = 1000;

        public static void SleepDelay(int millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout);
        }
    }
}
