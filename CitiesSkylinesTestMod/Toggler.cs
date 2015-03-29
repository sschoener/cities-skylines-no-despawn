using ColossalFramework;
using ColossalFramework.Plugins;
using UnityEngine;

namespace CitiesSkylinesNoDespawnMod
{
    class Toggler : MonoBehaviour
    {

        private static bool despawn = false;

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
            if (this.CtrlCmdDown && this.ShiftDown && Input.GetKeyDown(KeyCode.D))
            {
                despawn = !despawn;
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Toggled despawning to " + (despawn ? "enabled" : "disabled"));
            }
        }

        public static bool Despawn
        {
            get { return despawn; }
        }
    }
}