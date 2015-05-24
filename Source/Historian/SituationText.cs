using UnityEngine;

namespace KSEA.Historian
{
    public class SituationText : Text
    {
        private string m_Default = "";
        private string m_Landed = "";
        private string m_Splashed = "";
        private string m_Prelaunch = "";
        private string m_Flying = "";
        private string m_SubOrbital = "";
        private string m_Orbiting = "";
        private string m_Escaping = "";
        private string m_Docked = "";

        protected override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            m_Default = node.GetString("Default", "");
            m_Landed = node.GetString("Landed", "");
            m_Splashed = node.GetString("Splashed", "");
            m_Prelaunch = node.GetString("Prelaunch", "");
            m_Flying = node.GetString("Flying", "");
            m_SubOrbital = node.GetString("SubOrbital", "");
            m_Orbiting = node.GetString("Orbiting", "");
            m_Escaping = node.GetString("Escaping", "");
            m_Docked = node.GetString("Docked", "");
        }

        protected override void OnDraw(Rect bounds)
        {
            var text = m_Default;
            var vessel = FlightGlobals.ActiveVessel;

            if (vessel != null)
            {
                var situation = vessel.situation;
                text = ResolveText(situation);
            }

            SetText(text);

            base.OnDraw(bounds);
        }

        private string ResolveText(Vessel.Situations situation)
        {
            switch (situation)
            {
            case Vessel.Situations.LANDED:

                return m_Landed;

            case Vessel.Situations.SPLASHED:

                return m_Splashed;

            case Vessel.Situations.PRELAUNCH:

                return m_Prelaunch;

            case Vessel.Situations.FLYING:

                return m_Flying;

            case Vessel.Situations.SUB_ORBITAL:

                return m_SubOrbital;

            case Vessel.Situations.ORBITING:

                return m_Orbiting;

            case Vessel.Situations.ESCAPING:

                return m_Escaping;

            case Vessel.Situations.DOCKED:

                return m_Docked;
            }

            return m_Default;
        }
    }
}