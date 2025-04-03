// Copyright (c) 2025 Thomas Boehnlein
// 
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
// 
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using System;

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