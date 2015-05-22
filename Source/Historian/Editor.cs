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
    public class Editor
    {
        private bool m_Open = false;
        private LauncherButton m_LauncherButton = null;
        private ToolbarButton m_ToolbarButton = null;
        private Rect m_Position;
        private Texture m_NextButtonTexture = null;
        private Texture m_PreviousButtonTexture = null;
        private bool m_EnableLauncherButton = true;
        private bool m_EnableToolberButton = true;

        public Editor(Configuration configuration)
        {
            m_LauncherButton = new LauncherButton();
            m_ToolbarButton = new ToolbarButton();

            m_Position = new Rect(0.5f * Screen.width - 200.0f, 0.5f * Screen.height - 130.0f, 400.0f, 260.0f);

            m_NextButtonTexture = GameDatabase.Instance.GetTexture("KSEA/Historian/Historian_Button_Next", false);
            m_PreviousButtonTexture = GameDatabase.Instance.GetTexture("KSEA/Historian/Historian_Button_Previous", false);

            m_EnableLauncherButton = configuration.EnableLauncherButton;
            m_EnableToolberButton = configuration.EnableToolbarButton;

            if (m_EnableLauncherButton)
            {
                m_LauncherButton.OnTrue += Button_OnTrue;
                m_LauncherButton.OnFalse += Button_OnFalse;

                m_LauncherButton.Register();
            }

            if (m_EnableToolberButton)
            {
                m_ToolbarButton.OnTrue += Button_OnTrue;
                m_ToolbarButton.OnFalse += Button_OnFalse;

                m_ToolbarButton.Register();
            }
        }

        public void Open()
        {
            m_Open = true;
        }

        public void Close()
        {
            m_Open = false;
        }

        public void Draw()
        {
            if (m_Open)
            {
                m_Position = GUI.Window(0, m_Position, OnWindowGUI, "Historian", HighLogic.Skin.window);

                if (m_EnableLauncherButton)
                {
                    m_LauncherButton.Update();
                }

                if (m_EnableToolberButton)
                {
                    m_ToolbarButton.Update();
                }
            }
        }

        private void OnWindowGUI(int id)
        {
            GUI.skin = HighLogic.Skin;
            var historian = Historian.Instance;

            GUILayout.BeginVertical();

            historian.Suppressed = GUILayout.Toggle(historian.Suppressed, "Suppressed");
            historian.AlwaysActive = GUILayout.Toggle(historian.AlwaysActive, "Always Active");

            m_EnableLauncherButton = GUILayout.Toggle(m_EnableLauncherButton, "Use Stock Launcher");
            m_EnableToolberButton = GUILayout.Toggle(m_EnableToolberButton, "Use Blizzy's Toolbar");

            if (m_EnableLauncherButton && !m_LauncherButton.IsRegistered)
            {
                m_LauncherButton.OnTrue += Button_OnTrue;
                m_LauncherButton.OnFalse += Button_OnFalse;

                m_LauncherButton.Register();
            }
            else if (!m_EnableLauncherButton && m_LauncherButton.IsRegistered)
            {
                m_LauncherButton.OnTrue -= Button_OnTrue;
                m_LauncherButton.OnFalse -= Button_OnFalse;

                m_LauncherButton.Unregister();
            }

            if (m_EnableToolberButton && !m_ToolbarButton.IsRegistered)
            {
                m_ToolbarButton.OnTrue += Button_OnTrue;
                m_ToolbarButton.OnFalse += Button_OnFalse;

                m_ToolbarButton.SetState(m_Open);

                m_ToolbarButton.Register();
            }
            else if (!m_EnableToolberButton && m_ToolbarButton.IsRegistered)
            {
                m_ToolbarButton.OnTrue -= Button_OnTrue;
                m_ToolbarButton.OnFalse -= Button_OnFalse;

                m_ToolbarButton.SetState(m_Open);

                m_ToolbarButton.Unregister();
            }

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Layout");
            GUILayout.Space(20);

            var layouts = historian.GetLayoutNames();

            if (GUILayout.Button(m_PreviousButtonTexture, GUILayout.Width(20), GUILayout.Height(GUI.skin.label.lineHeight)))
            {
                historian.CurrentLayoutIndex = Mathf.Clamp(historian.CurrentLayoutIndex - 1, 0, layouts.Length - 1);
            }
            else if (GUILayout.Button(m_NextButtonTexture, GUILayout.Width(20), GUILayout.Height(GUI.skin.label.lineHeight)))
            {
                historian.CurrentLayoutIndex = Mathf.Clamp(historian.CurrentLayoutIndex + 1, 0, layouts.Length - 1);
            }

            GUILayout.Space(5);
            GUILayout.Label(historian.GetCurrentLayoutName(), GUI.skin.textField);

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Load", GUILayout.Width(100.0f)))
            {
                historian.Reload();
            }

            if (GUILayout.Button("Save", GUILayout.Width(100.0f)))
            {
                var configuration = historian.GetConfiguration();

                configuration.Layout = historian.GetCurrentLayoutName();
                configuration.EnableLauncherButton = m_EnableLauncherButton;
                configuration.EnableToolbarButton = m_EnableToolberButton;

                historian.SetConfiguration(configuration);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        private void Button_OnTrue()
        {
            Open();
        }

        private void Button_OnFalse()
        {
            Close();
        }
    }
}