using ICities;

namespace CitiesSkylinesNoDespawnMod
{
    // http://www.reddit.com/r/CitiesSkylinesModding/comments/2ypcl5/guide_using_visual_studio_2013_to_develop_mods/
    public class NoDespawnMod : IUserMod
    {
        public string Name { get { return "Cope's No Despawn"; } }
        public string Description { get { return "Vehicles won't despawn any more when they are stuck in traffic"; } }
    }
}
