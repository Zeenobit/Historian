/**
 * This file is part of Historian.
 * 
 * Historian is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Historian is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Historian. If not, see <http://www.gnu.org/licenses/>.
 **/

using System;
using UnityEngine;

namespace KSEA.Historian
{
    public static class ConfigNodeExtension
    {
        public static Color GetColor(this ConfigNode self, string name, Color fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);
                    return ConfigNode.ParseColor(value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static Vector2 GetVector2(this ConfigNode self, string name, Vector2 fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);
                    return ConfigNode.ParseVector2(value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static string GetString(this ConfigNode self, string name, string fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    return self.GetValue(name);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static bool GetBoolean(this ConfigNode self, string name, bool fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);

                    return bool.Parse(value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static float GetFloat(this ConfigNode self, string name, float fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);
                    return float.Parse(value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static int GetInteger(this ConfigNode self, string name, int fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);
                    return int.Parse(value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static Version GetVersion(this ConfigNode self, string name, Version fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);
                    return new Version(value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static T GetEnum<T>(this ConfigNode self, string name, T fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);
                    return (T) (object) ConfigNode.ParseEnum(typeof(T), value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }
    }
}