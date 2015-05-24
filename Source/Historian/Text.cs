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
using System.Globalization;
using UnityEngine;

namespace KSEA.Historian
{
    public class Text : Element
    {
        private Color m_Color = Color.white;
        private string m_Text = "";
        private TextAnchor m_TextAnchor = TextAnchor.MiddleCenter;
        private int m_FontSize = 10;
        private FontStyle m_FontStyle = FontStyle.Normal;

        protected void SetText(string text)
        {
            m_Text = text;
        }

        protected override void OnDraw(Rect bounds)
        {
            var style = new GUIStyle(GUI.skin.label);

            style.alignment = m_TextAnchor;
            style.normal.textColor = m_Color;
            style.fontSize = m_FontSize;
            style.fontStyle = m_FontStyle;
            style.richText = true;

            var content = new GUIContent();
            content.text = Parse(m_Text);

            GUI.Label(bounds, content, style);
        }

        protected override void OnLoad(ConfigNode node)
        {
            m_Color = node.GetColor("Color", Color.white);
            m_Text = node.GetString("Text", "");
            m_TextAnchor = node.GetEnum("TextAnchor", TextAnchor.MiddleCenter);
            m_FontSize = node.GetInteger("FontSize", 10);
            m_FontStyle = node.GetEnum("FontStyle", FontStyle.Normal);
        }

        protected static string Parse(string text)
        {
            var ut = Planetarium.GetUniversalTime();
            var time = KSPUtil.GetKerbinDateFromUT((int) ut);
            var vessel = FlightGlobals.ActiveVessel;

            if (text.Contains("<N>"))
            {
                text = text.Replace("<N>", Environment.NewLine);
            }

            if (text.Contains("<UT>"))
            {
                var value = string.Format("Y{0}, D{1:D2}, {2}:{3:D2}:{4:D2}", time[4] + 1, time[3] + 1, time[2], time[1], time[0]);

                text = text.Replace("<UT>", value);
            }

            if (text.Contains("<Year>"))
            {
                text = text.Replace("<Year>", time[4].ToString());
            }

            if (text.Contains("<Day>"))
            {
                text = text.Replace("<Day>", time[3].ToString());
            }

            if (text.Contains("<Hour>"))
            {
                text = text.Replace("<Hour>", time[2].ToString());
            }

            if (text.Contains("<Minute>"))
            {
                text = text.Replace("<Minute>", time[1].ToString());
            }

            if (text.Contains("<Second>"))
            {
                text = text.Replace("<Second>", time[0].ToString());
            }

            if (text.Contains("<T+>"))
            {
                var value = "";

                if (vessel != null)
                {
                    var t = KSPUtil.GetKerbinDateFromUT((int) vessel.missionTime);

                    if (t[4] > 0)
                    {
                        value = string.Format("T+ {0}y, {1}d, {2:D2}:{3:D2}:{4:D2}", t[4] + 1, t[3] + 1, t[2], t[1], t[0]);
                    }
                    else if (t[3] > 0)
                    {
                        value = string.Format("T+ {1}d, {2:D2}:{3:D2}:{4:D2}", t[4] + 1, t[3] + 1, t[2], t[1], t[0]);
                    }
                    else
                    {
                        value = string.Format("T+ {2:D2}:{3:D2}:{4:D2}", t[4] + 1, t[3] + 1, t[2], t[1], t[0]);
                    }
                }

                text = text.Replace("<T+>", value);
            }

            if (text.Contains("<Vessel>"))
            {
                var value = "";

                if (vessel != null)
                {
                    value = vessel.vesselName;
                }

                text = text.Replace("<Vessel>", value);
            }

            if (text.Contains("<Body>"))
            {
                var value = "";

                if (vessel != null)
                {
                    value = Planetarium.fetch.CurrentMainBody.bodyName;
                }

                text = text.Replace("<Body>", value);
            }

            if (text.Contains("<Situation>"))
            {
                var value = "";

                if (vessel != null)
                {
                    switch (vessel.situation)
                    {
                    case Vessel.Situations.SUB_ORBITAL:
                        value = "Sub-orbital";
                        break;

                    default:
                        value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(vessel.situation.ToString().ToLower());
                        break;
                    }
                }

                text = text.Replace("<Situation>", value);
            }

            if (text.Contains("<Biome>"))
            {
                var value = "";

                if (vessel != null)
                {
                    var biome = ScienceUtil.GetExperimentBiome(FlightGlobals.ActiveVessel.mainBody, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude);
                    value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(biome.ToLower());
                }

                text = text.Replace("<Biome>", value);
            }

            if (text.Contains("<Latitude>"))
            {
                var value = "";

                if (vessel != null)
                {
                    value = vessel.latitude.ToString("F3");
                }

                text = text.Replace("<Latitude>", value);
            }

            if (text.Contains("<Longitude>"))
            {
                var value = "";

                if (vessel != null)
                {
                    value = vessel.longitude.ToString("F3");
                }

                text = text.Replace("<Longitude>", value);
            }

            if (text.Contains("<Altitude>"))
            {
                var value = "";

                if (vessel != null)
                {
                    value = vessel.altitude.ToString("F1");
                }

                text = text.Replace("<Altitude>", value);
            }

            if (text.Contains("<Mach>"))
            {
                var value = "";

                if (vessel != null)
                {
                    value = vessel.mach.ToString("F1");
                }

                text = text.Replace("<Mach>", value);
            }

            if (text.Contains("<LandingZone>"))
            {
                var value = "";

                if (vessel != null)
                {
                    value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(vessel.landedAt.ToLower());
                }

                text = text.Replace("<LandingZone>", value);
            }

            if (text.Contains("<Speed>"))
            {
                var value = "";

                if (vessel != null)
                {
                    value = vessel.srfSpeed.ToString("F1");
                }

                text = text.Replace("<Speed>", value);
            }

            if (text.Contains("<Custom>"))
            {
                var value = Historian.Instance.GetConfiguration().CustomText;

                // No infinite recursion for you.
                value = value.Replace("<Custom>", "");
                value = Parse(value);

                text = text.Replace("<Custom>", value);
            }

            return text;
        }
    }
}