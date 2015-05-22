using UnityEngine;

namespace KSEA.Historian
{
    public class ToolbarButton
    {
        private IButton m_Button = null;
        private bool m_State = false;
        public delegate void Callback();

        public event Callback OnTrue = delegate { };
        public event Callback OnFalse = delegate { };

        public bool IsRegistered
        {
            get;
            private set;
        }

        public void SetState(bool state)
        {
            m_State = state;
        }

        public void Register()
        {
            if (ToolbarManager.ToolbarAvailable)
            {
                var toolbar = ToolbarManager.Instance;

                m_Button = toolbar.add("KSEA_Historian", "Button");

                m_Button.TexturePath = "KSEA/Historian/Historian_Toolbar";
                m_Button.OnClick += Button_OnClick;

                IsRegistered = true;
            }
        }

        public void Unregister()
        {
            IsRegistered = false;

            if (m_Button != null)
            {
                m_Button.Destroy();
            }
        }

        public void Update()
        {
            var historian = Historian.Instance;

            if (historian.Suppressed)
            {
                m_Button.TexturePath = "KSEA/Historian/Historian_Toolbar_Suppressed";
            }
            else
            {
                m_Button.TexturePath = "KSEA/Historian/Historian_Toolbar";
            }
        }

        public void Button_OnClick(ClickEvent e)
        {
            m_State = !m_State;

            if (m_State)
            {
                OnTrue();
            }
            else
            {
                OnFalse();
            }
        }
    }
}