using GoRogue;
using GoRogue.Factory;

namespace SadConsole.Tiles
{
    public class TileBlueprintConfig : BlueprintConfig
    {
        public readonly Coord Position;

        public TileBlueprintConfig(Coord position) => Position = position;

        public static implicit operator Coord(TileBlueprintConfig config) => config.Position;
        public static implicit operator TileBlueprintConfig(Coord position) => new TileBlueprintConfig(position);
    }
}
