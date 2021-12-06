using System;
using UnityEngine;

namespace SpicaSDK.Editor.Editor_Toolbox.Editor.Internal
{
    internal class GuiBackground : IDisposable
    {
        private Color newBackgroundColor;
        private Color oldBackgroundColor;

        public GuiBackground(Color newBackgroundColor)
        {
            this.newBackgroundColor = newBackgroundColor;
            Prepare();
        }

        public void Prepare()
        {
            oldBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = newBackgroundColor;
        }

        public void Dispose()
        {
            GUI.backgroundColor = oldBackgroundColor;
        }
    }
}