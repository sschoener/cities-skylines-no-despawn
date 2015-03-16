using System;
using System.Collections.Generic;
using System.Reflection;
using CitiesSkylinesNoDespawnMod.VehicleAIMod;
using ColossalFramework.Plugins;
using ICities;
using Object = UnityEngine.Object;

namespace CitiesSkylinesNoDespawnMod
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;
            var mapping = new Dictionary<Type, Type>
            {
                {typeof (CargoTruckAI), typeof (CargoTruckAIMod)},
                {typeof (PassengerCarAI), typeof (PassengerCarAIMod)},
            };
            int num = PrefabCollection<VehicleInfo>.PrefabCount();
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Found " + num + " vehicle infos.");
            for (uint i = 0; i < num; i++)
            {
                var vi = PrefabCollection<VehicleInfo>.GetPrefab(i);
                AdjustVehicleAI(vi, mapping);
            }
        }

        private void PrintFields(object a)
        {
            foreach (var f in a.GetType().GetFields())
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, f.Name + ": " + f.GetValue(a));
        }

        private void AdjustVehicleAI(VehicleInfo vi, Dictionary<Type, Type> componentRemap)
        {
            var oldAI = vi.GetComponent<VehicleAI>();
            if (oldAI == null)
                return;
            var compType = oldAI.GetType();
            Type newCompType;
            if (!componentRemap.TryGetValue(compType, out newCompType))
                return;
            var fields = ExtractFields(oldAI);
            Object.DestroyImmediate(oldAI);
            VehicleAI newAI = vi.gameObject.AddComponent(newCompType) as VehicleAI;
            SetFields(newAI, fields);
            newAI.m_info = vi;
            vi.m_vehicleAI = newAI;
            newAI.InitializeAI();
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Successfully patched vehicle info with " + newCompType);
        }

        private Dictionary<string, object> ExtractFields(object a)
        {
            var fields = a.GetType().GetFields();
            var dict = new Dictionary<string, object>(fields.Length);
            for (int i = 0; i < fields.Length; i++)
            {
                var af = fields[i];
                dict[af.Name] = af.GetValue(a);
            }
            return dict;
        }

        private void SetFields(object b, Dictionary<string, object> fieldValues)
        {
            var bType = b.GetType();
            foreach (var kvp in fieldValues)
            {
                var bf = bType.GetField(kvp.Key);
                if (bf == null)
                {
                    DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Could not find field " + kvp.Key + " in " + b);
                    continue;
                }
                bf.SetValue(b, kvp.Value);
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Set value " + kvp.Key + " to " + kvp.Value);
            }
        }
    }
}