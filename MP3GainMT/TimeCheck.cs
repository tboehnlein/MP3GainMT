﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3GainMT
{
    internal class TimeCheck
    {
        public double MinimumTime = .25;
        public DateTime LastTime = DateTime.Now;
        public bool Force = false;

        public TimeCheck(int fps)
        {
            MinimumTime = 1.0 / fps;
        }

        public TimeCheck(double minimum)
        {
            MinimumTime = minimum;
        }

        public TimeCheck() { }

        public bool CheckTime(bool force = false)
        {
            var result = TimeCheck.Check(force, ref LastTime, MinimumTime);

            return result;
        }
        
        public static bool Check(bool force, ref DateTime lastTime, double minTime = .25)
        {
            var current = DateTime.Now;
            var result = (current - lastTime).TotalSeconds > minTime || force;

            // for reference next time
            if (result)
            {
                lastTime = current;
            }

            return result;
        }
    }
}
