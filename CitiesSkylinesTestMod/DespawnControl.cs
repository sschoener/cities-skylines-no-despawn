using System.Collections;
using ColossalFramework.Plugins;
using UnityEngine;

namespace CitiesSkylinesNoDespawnMod
{
    class DespawnControl : MonoBehaviour
    {
        private static bool _despawn;

        private bool CtrlCmdDown
        {
            get
            {
                return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
                       Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
            }
        }

        private bool ShiftDown
        {
            get
            {
                return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
        }

        private void Update()
        {
            if (CtrlCmdDown && ShiftDown && Input.GetKeyDown(KeyCode.D))
            {
                _despawn = !_despawn;
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Toggled despawning to " + (_despawn ? "enabled" : "disabled"));
            }
        }

        public static bool Despawn
        {
            get { return _despawn; }
        }
    }
}