using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ZiZhuJY.Helpers
{
    public class TextHelper
    {
        /// <summary>
        /// Calculate word count
        /// </summary>
        /// <returns></returns>
        public static int CalculateWordCount(string article)
        {
            var sec = Regex.Split(article, @"\s");
            int count = 0;
            foreach (var si in sec)
            {
                int ci = Regex.Matches(si, @"[\u0000-\u00ff]+").Count;
                foreach (var c in si)
                    if ((int)c > 0x00FF) ci++;

                count += ci;
            }

            return count;
        }
    }
}
