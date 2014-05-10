using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ZiZhuJY.Helpers
{
    public class ThreadHelper
    {
        private static readonly TimeSpan defaultRetryInterval = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan defaultTimeout = TimeSpan.FromMinutes(5);

        public delegate bool PollCondition();

        public static bool PollWait(PollCondition conditionDelegate)
        {
            return PollWait(conditionDelegate, defaultRetryInterval);
        }

        public static bool PollWait(PollCondition conditionDelegate, TimeSpan retryInterval)
        {
            return PollWait(conditionDelegate, retryInterval, defaultTimeout);
        }

        public static bool PollWait(PollCondition conditionDelegate, TimeSpan retryInterval, TimeSpan timeout)
        {
            if (conditionDelegate())
            {
                return true;
            }
            DateTime waitStartTime = DateTime.Now;
            while (DateTime.Now.Subtract(waitStartTime) < timeout)
            {
                Thread.Sleep(retryInterval);
                if (conditionDelegate())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
