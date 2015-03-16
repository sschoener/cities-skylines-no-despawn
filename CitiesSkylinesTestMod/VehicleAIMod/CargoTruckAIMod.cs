using UnityEngine;

namespace CitiesSkylinesNoDespawnMod.VehicleAIMod
{
    public class CargoTruckAIMod : CargoTruckAI
    {
        public override void SimulationStep(ushort vehicleID, ref Vehicle data, Vector3 physicsLodRefPos)
        {
            data.m_flags &= ~Vehicle.Flags.Congestion;
            base.SimulationStep(vehicleID, ref data, physicsLodRefPos);
        }
    }
}