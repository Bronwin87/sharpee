﻿using DataStore;
using StandardLibrary;
using Common;

namespace MyStory
{
    public static class LocationId
    {
        public const string Kitchen = "kitchen";
        public const string LivingRoom = "livingRoom";
        public const string FrontPorch = "frontPorch";
        public const string FrontYard = "frontYard";
    }

    public class TheHouse : IStory
    {
        public World World { get; private set; }
        public Core Core { get; private set; }
        public Player Player => Core.Player;

        public TheHouse()
        {
            World = new World();
            Core = new Core(World);
        }

        public void InitializeWorld()
        {
            Core
                .CreateLocation(LocationId.Kitchen, "Kitchen", "A clean and well-organized kitchen.")
                .CreateLocation(LocationId.LivingRoom, "Living Room", "A cozy living room with a comfortable couch.")
                .CreateLocation(LocationId.FrontPorch, "Front Porch", "A small front porch with a wooden bench.")
                .CreateLocation(LocationId.FrontYard, "Front Yard", "A well-maintained front yard with a beautiful garden.");
        }
    }

}
