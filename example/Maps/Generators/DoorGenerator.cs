using System.Collections.Generic;
using SadConsole.Maps;

namespace BasicTutorial.Maps.Generators
{
    public static class DoorGenerator
    {
        public static void Generate(SadConsole.Tiles.TileMap map, IEnumerable<Region> rooms, string doorBlueprint, int leaveFloorAloneChance = 20)
        {
            bool PercentageCheck(int outOfHundred) => outOfHundred != 0 && GoRogue.Random.SingletonRandom.DefaultRNG.Next(101) < outOfHundred;

            foreach (Region room in rooms)
                foreach (GoRogue.Coord point in room.Connections)
                    if (!PercentageCheck(leaveFloorAloneChance))
                        map.SetTerrain(SadConsole.Tiles.Tile.Factory.Create(doorBlueprint, point));
        }
    }
}
