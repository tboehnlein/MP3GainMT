﻿using System;

namespace MP3GainMT
{
    public class TimeCheck
    {
        public bool Force = false;
        public DateTime LastTime = DateTime.Now;
        public double MinimumTime = .25;

        public TimeCheck(int fps)
        {
            MinimumTime = 1.0 / fps;
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

        public static bool Enough(bool force, ref DateTime lastTime, double minTime = .25)
        {
            var current = DateTime.Now;
            var result = (current - lastTime).TotalSeconds > minTime || force;

            // for reference next time
            lastTime = current;

            return result;
        }

        public bool CheckTime(bool force = false)
        {
            var result = TimeCheck.Check(force, ref LastTime, MinimumTime);

            return result;
        }

        public bool EnoughTime(bool force = false)
        {
            var result = TimeCheck.Enough(force, ref LastTime, MinimumTime);

            return result;
        }
    }
}