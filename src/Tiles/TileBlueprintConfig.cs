using GoRogue;
using GoRogue.Factory;

namespace SadConsole.Tiles
{
    /// <summary>
    /// Config object for creating Tiles.  Implicitly convertible to/from a Coord.
    /// </summary>
    public class TileBlueprintConfig : BlueprintConfig
    {
        /// <summary>
        /// The position to create the Tile object at.
        /// </summary>
        public readonly Coord Position;

        /// <summary>
        /// Creates a configuration that will create a Tile at the given position..
        /// </summary>
        /// <param name="position">The position to create the Tile object at.</param>
        public TileBlueprintConfig(Coord position) => Position = position;

        /// <summary>
        /// Implicitly converts to a Coord instance.
        /// </summary>
        /// <param name="config"/>
        public static implicit operator Coord(TileBlueprintConfig config) => config.Position;

        /// <summary>
        /// Implicitly converts a Coord to a TileBlueprintConfig.
        /// </summary>
        /// <param name="position"/>
        public static implicit operator TileBlueprintConfig(Coord position) => new TileBlueprintConfig(position);
    }
}
