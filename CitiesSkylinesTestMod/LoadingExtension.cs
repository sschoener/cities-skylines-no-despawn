using System;
using System.Collections.Generic;
using System.Reflection;
using CitiesSkylinesNoDespawnMod.VehicleAIMod;
using ColossalFramework.Plugins;
using ICities;
using Object = UnityEngine.Object;
using GameObject = UnityEngine.GameObject;

namespace CitiesSkylinesNoDespawnMod
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static Toggler toggler;
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Loading NoDespawn mod");

            GameObject gameObject = new GameObject("NoDespawnToggler");
            LoadingExtension.toggler = gameObject.AddComponent<Toggler>();

            UpdateVehicleInformation();
        }

        public override void OnLevelUnloading()
        {
            if (LoadingExtension.toggler != null)
            {
                GameObject.Destroy(LoadingExtension.toggler.gameObject);
                LoadingExtension.toggler = null;
            }
        }

        public void UpdateVehicleInformation()
        {
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
            // make sure vehicle AI is set at all
            var oldAI = vi.GetComponent<VehicleAI>();
            if (oldAI == null)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Aborting, AI for vehicle is null");
                return;
            }

            // try to get new AI type for currently set type
            var compType = oldAI.GetType();
            Type newCompType;
            if (!componentRemap.TryGetValue(compType, out newCompType))
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Did not patch vehicle info for type " + compType);
                return;
            }

            // pull fields from old AI and destroy it
            var fields = ExtractFields(oldAI);
            Object.DestroyImmediate(oldAI);

            // create new AI and add it to the game
            VehicleAI newAI = vi.gameObject.AddComponent(newCompType) as VehicleAI;

            // import old AI settings to new AI instance and initialize it
            SetFields(newAI, fields);
            newAI.m_info = vi;
            vi.m_vehicleAI = newAI;
            newAI.InitializeAI();
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Successfully patched vehicle info for type " + compType + " with " + newCompType);
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