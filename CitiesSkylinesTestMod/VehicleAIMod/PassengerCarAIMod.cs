using UnityEngine;

namespace CitiesSkylinesNoDespawnMod.VehicleAIMod
{
    public class PassengerCarAIMod : PassengerCarAI
    {
        public override void SimulationStep(ushort vehicleID, ref Vehicle data, Vector3 physicsLodRefPos)
        {
            if (!DespawnControl.Despawn)
                data.m_flags &= ~Vehicle.Flags.Congestion;
            base.SimulationStep(vehicleID, ref data, physicsLodRefPos);
        }
    }
}