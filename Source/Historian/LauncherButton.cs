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

using UnityEngine;

namespace KSEA.Historian
{
    public class LauncherButton
    {
        private ApplicationLauncherButton m_Button = null;
        private Texture m_NormalTexture = null;
        private Texture m_SuppressedTexture = null;
        public delegate void Callback();

        public event Callback OnTrue = delegate { };
        public event Callback OnFalse = delegate { };

        public bool IsRegistered
        {
            get;
            private set;
        }

        public LauncherButton()
        {
            var database = GameDatabase.Instance;

            m_NormalTexture = database.GetTexture("KSEA/Historian/Historian_Launcher", false);
            m_SuppressedTexture = database.GetTexture("KSEA/Historian/Historian_Launcher_Suppressed", false);
        }

        public void Register()
        {
            var launcher = ApplicationLauncher.Instance;
            var scenes = ApplicationLauncher.AppScenes.FLIGHT |
                         ApplicationLauncher.AppScenes.MAPVIEW |
                         ApplicationLauncher.AppScenes.SPACECENTER |
                         ApplicationLauncher.AppScenes.SPH |
                         ApplicationLauncher.AppScenes.TRACKSTATION |
                         ApplicationLauncher.AppScenes.VAB;

            m_Button = launcher.AddModApplication(Button_OnTrue, Button_OnFalse, null, null, null, null, scenes, m_NormalTexture);            

            Update();

            IsRegistered = true;
        }

        public void Unregister()
        {
            IsRegistered = false;
            var launcher = ApplicationLauncher.Instance;
            launcher.RemoveModApplication(m_Button);
            m_Button = null;
        }

        public void Set(bool value, bool call = true)
        {
            if (value)
            {
                m_Button.SetTrue(call);
            }
            else
            {
                m_Button.SetFalse(call);
            }
        }

        public void Update()
        {
            var historian = Historian.Instance;

            if (historian.Suppressed)
            {
                m_Button.SetTexture(m_SuppressedTexture);
            }
            else
            {
                m_Button.SetTexture(m_NormalTexture);
            }
        }

        private void Button_OnTrue()
        {
            OnTrue();
        }

        private void Button_OnFalse()
        {
            OnFalse();
        }
    }
}