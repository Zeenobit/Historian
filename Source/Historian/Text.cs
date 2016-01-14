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
using System.Linq;
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
        private string m_pilotColor, m_engineerColor, m_scientistColor, m_touristColor;
        private int m_baseYear;
        private string m_dateFormat;
        private bool m_isKerbincalendar;

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

            m_pilotColor = node.GetString("PilotColor", "clear");
            m_engineerColor = node.GetString("EngineerColor", "clear");
            m_scientistColor = node.GetString("ScientistColor", "clear");
            m_touristColor = node.GetString("TouristColor", "clear");

            m_isKerbincalendar = GameSettings.KERBIN_TIME;

            m_baseYear = node.GetInteger("BaseYear", m_isKerbincalendar ? 1 : 1940 );
            m_dateFormat = node.GetString("DateFormat", CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern);
        }

        protected string Parse(string text)
        {
            var ut = Planetarium.GetUniversalTime();
            int[] time;

            if (m_isKerbincalendar) 
            {
                time = KSPUtil.GetKerbinDateFromUT((int)ut);
            }
            else 
            {
                time = KSPUtil.GetEarthDateFromUT((int)ut);
            }

            var vessel = FlightGlobals.ActiveVessel;

            if (text.Contains("<N>"))
            {
                text = text.Replace("<N>", Environment.NewLine);
            }

            if (text.Contains("<Date>")) {
                if (m_isKerbincalendar)
                {
                    // use custom date formatter for Kerbin time
                    text = text.Replace("<Date>", time.FormattedDate(m_dateFormat, m_baseYear));
                }
                else
                {
                    // create date object including time in case user wants to specify time format as well as date
                    var dt = new DateTime(time[4] + m_baseYear, 1, 1, time[2], time[1], time[0]).AddDays(time[3]);
                    text = text.Replace("<Date>", dt.ToString(m_dateFormat));
                }
            }

            

            if (text.Contains("<UT>"))
            {
                var value = string.Format("Y{0}, D{1:D2}, {2}:{3:D2}:{4:D2}", time[4] + m_baseYear, time[3] + 1, time[2], time[1], time[0]);

                text = text.Replace("<UT>", value);
            }

            if (text.Contains("<Year>"))
            {
                text = text.Replace("<Year>", (time[4] + m_baseYear).ToString());
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
                    value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(vessel.situation.ToString().Replace("_","-").ToLower());
                }
                text = text.Replace("<Situation>", value);
            }

            if (text.Contains("<Biome>"))
            {
                var value = "";

                if (vessel != null)
                {
                    var biome = ScienceUtil.GetExperimentBiome(vessel.mainBody, vessel.latitude, vessel.longitude);
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
                    double altitude;
                    string unit;
                    ShortenDistance(vessel.altitude, out altitude, out unit);

                    value = string.Format("{0:F1} {1}", altitude, unit);
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

            if (text.Contains("<Heading>"))
            {
                text = text.Replace("<Heading>", FlightGlobals.ship_heading.ToString("F1"));
            }

            if (text.Contains("<LandingZone>"))
            {
                var value = "";

                if (vessel != null)
                {
                    value = vessel.landedAt;

                    if (string.IsNullOrEmpty(value))
                    {
                        value = ScienceUtil.GetExperimentBiome(vessel.mainBody, vessel.latitude, vessel.longitude);
                    }
                    else
                    {
                        // http://forum.kerbalspaceprogram.com/threads/123896-Human-Friendly-Landing-Zone-Title
                        value = Vessel.GetLandedAtString(vessel.landedAt);
                    }

                    value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
                }

                text = text.Replace("<LandingZone>", value);
            }

            if (text.Contains("<Speed>"))
            {
                var value = "";

                if (vessel != null)
                {
                    double speed;
                    string unit;
                    ShortenDistance(vessel.srfSpeed, out speed, out unit);

                    value = string.Format("{0:F1} {1}/s", speed, unit);
                }

                text = text.Replace("<Speed>", value);
            }

            if (text.Contains("<Crew>"))
            {
                var value = "";

                if (vessel != null && !vessel.isEVA)
                {
                    if (vessel.GetCrewCount() > 0)
                    {
                        value = string.Join(", ", vessel.GetVesselCrew().Select(item => TraitColor(item.trait) + item.name + "</color>").ToArray());
                    }
                    else {
                        if (vessel.isCommandable)
                        {
                            value = "Unmanned";
                        }
                        else {
                            value = "N/A";
                        }
                    }
                }

                text = text.Replace("<Crew>", value);
            }

            if (text.Contains("<CrewShort>"))
            {
                var value = "";

                if (vessel != null && !vessel.isEVA)
                {
                    if (vessel.GetCrewCount() > 0)
                    {
                        value = string.Join(", ", vessel.GetVesselCrew().Select(item => TraitColor(item.trait) + item.name.Replace(" Kerman", "") + "</color>").ToArray()) + " Kerman";
                    }
                    else {
                        if (vessel.isCommandable)
                        {
                            value = "Unmanned";
                        }
                        else {
                            value = "N/A";
                        }
                    }
                }

                text = text.Replace("<CrewShort>", value);
            }

            if (text.Contains("<Pilots>"))
                text = text.Replace("<Pilots>", TraitColor("Pilot") + CrewByTrait(vessel, "Pilot", false, false) + "</color>");

            if (text.Contains("<Engineers>"))
                text = text.Replace("<Engineers>", TraitColor("Engineer") + CrewByTrait(vessel, "Engineer", false, false) + "</color>");

            if (text.Contains("<Scientists>"))
                text = text.Replace("<Scientists>", TraitColor("Scientist") + CrewByTrait(vessel, "Scientist", false, false) + "</color>");

            if (text.Contains("<Tourists>"))
                text = text.Replace("<Tourists>", TraitColor("Tourist") + CrewByTrait(vessel, "Tourist", false, false) + "</color>");

            if (text.Contains("<PilotsList>"))
                text = text.Replace("<PilotsList>", TraitColor("Pilot") + CrewByTrait(vessel, "Pilot", false, true) + "</color>");

            if (text.Contains("<EngineersList>"))
                text = text.Replace("<EngineersList>", TraitColor("Engineer") + CrewByTrait(vessel, "Engineer", false, true) + "</color>");

            if (text.Contains("<ScientistsList>"))
                text = text.Replace("<ScientistsList>", TraitColor("Scientist") + CrewByTrait(vessel, "Scientist", false, true) + "</color>");

            if (text.Contains("<TouristsList>"))
                text = text.Replace("<TouristsList>", TraitColor("Tourist") + CrewByTrait(vessel, "Tourist", false, true) + "</color>");


            if (text.Contains("<Custom>"))
            {
                var value = Historian.Instance.GetConfiguration().CustomText;

                // No infinite recursion for you.
                value = value.Replace("<Custom>", "");
                value = Parse(value);

                text = text.Replace("<Custom>", value);
            }

            if (text.Contains("<Ap>"))
            {
                var value = "";

                if (vessel != null)
                {
                    var orbit = vessel.GetOrbit();
                    double ap;
                    string unit;
                    ShortenDistance(orbit.ApA, out ap, out unit);

                    value = string.Format("{0:F1} {1}", ap, unit);
                }

                text = text.Replace("<Ap>", value);
            }

            if (text.Contains("<Pe>"))
            {
                var value = "";

                if (vessel != null)
                {
                    var orbit = vessel.GetOrbit();
                    double pe;
                    string unit;
                    ShortenDistance(orbit.PeA, out pe, out unit);

                    value = string.Format("{0:F1} {1}", pe, unit);
                }

                text = text.Replace("<Pe>", value);
            }

            if (text.Contains("<Inc>"))
            {
                var value = "";

                if (vessel != null)
                {
                    var orbit = vessel.GetOrbit();
                    double inc = orbit.inclination;

                    value = string.Format("{0:F1}°", inc);
                }

                text = text.Replace("<Inc>", value);
            }

            if (text.Contains("<LAN>"))
            {
                var value = "";
                if (vessel != null)
                {
                    var orbit = vessel.GetOrbit();
                    double lan = orbit.LAN;

                    value = string.Format("{0:F1}°", lan);
                }

                text = text.Replace("<LAN>", value);
            }

            if (text.Contains("<ArgPe>"))
            {
                var value = "";
                if (vessel != null)
                {
                    var orbit = vessel.GetOrbit();
                    double argPe = orbit.argumentOfPeriapsis;

                    value = string.Format("{0:F1}°", argPe);
                }

                text = text.Replace("<ArgPe>", value);
            }

            if (text.Contains("<Ecc>"))
            {
                var value = "";
                if (vessel != null)
                {
                    var orbit = vessel.GetOrbit();
                    double ecc = orbit.eccentricity;

                    value = string.Format("{0:F3}", ecc);
                }

                text = text.Replace("<Ecc>", value);
            }

            if (text.Contains("<Period>"))
            {
                var value = "";
                if (vessel != null)
                {
                    var orbit = vessel.GetOrbit();
                    var period = orbit.period;
                    int[] t; 

                    if (m_isKerbincalendar)
                    {
                        t = KSPUtil.GetKerbinDateFromUT((int)period);
                    }
                    else
                    {
                        t = KSPUtil.GetEarthDateFromUT((int)period);
                    }
                    if (t[4] > 0)
                    {
                        value = string.Format("{0}y, {1}d, {2:D2}:{3:D2}:{4:D2}", t[4] + 1, t[3] + 1, t[2], t[1], t[0]);
                    }
                    else if (t[3] > 0)
                    {
                        value = string.Format("{1}d, {2:D2}:{3:D2}:{4:D2}", t[4] + 1, t[3] + 1, t[2], t[1], t[0]);
                    }
                    else
                    {
                        value = string.Format("{2:D2}:{3:D2}:{4:D2}", t[4] + 1, t[3] + 1, t[2], t[1], t[0]);
                    }
                }

                text = text.Replace("<Period>", value);
            }


            if (text.Contains("<Orbit>"))
            {
                var value = "";

                if (vessel != null)
                {
                    var orbit = vessel.GetOrbit();
                    double ap;
                    double pe;
                    string unitAp, unitPe;
                    ShortenDistance(orbit.ApA, out ap, out unitAp);
                    ShortenDistance(orbit.PeA, out pe, out unitPe);

                    value = string.Format("{0:F1} {1} x {2:F1} {3}", ap, unitAp, pe, unitPe);
                }

                text = text.Replace("<Orbit>", value);
            }

            return text;
        }

        protected string CrewByTrait(Vessel vessel, string trait, bool isShort, bool isList)
        {
            var value = "";

            if (vessel != null && !vessel.isEVA)
            {

                var crewMembers = vessel.GetVesselCrew()
                    .Where(member => member.trait == trait)
                    .Select(member => (isShort) ? member.name.Replace(" Kerman", "") : member.name)
                    .ToArray();

                if (crewMembers.Length > 0)
                {
                    if (isList)
                    {
                        value = "• " + string.Join(Environment.NewLine + "• ", crewMembers);
                    }
                    else
                    {
                        value = string.Join(", ", crewMembers) + (isShort ? " Kerman" : "");
                    }
                }
                else {
                    if (vessel.isCommandable)
                    {
                        value = "None";
                    }
                    else {
                        value = "N/A";
                    }
                }
            }

            return value;
        }

        protected string TraitColor(string trait)
        {
            switch (trait)
            {
                case "Pilot":
                    return "<color=" + m_pilotColor + ">";
                case "Engineer":
                    return "<color=" + m_engineerColor + ">";
                case "Scientist":
                    return "<color=" + m_scientistColor + ">";
                case "Tourist":
                    return "<color=" + m_touristColor + ">";
                default:
                    return "<color=clear>";
            }
        }

        static readonly string[] m_units = { "m", "km", "Mm", "Gm", "Tm", "Pm" };

        protected static void ShortenDistance(double meters, out double result, out string unit)
        {
            double d = meters;
            int i = 0;

            while (d > 1000.0)
            {
                d /= 1000.0f;
                ++i;
            }

            result = d;
            unit = m_units[i];
        }
    }
}