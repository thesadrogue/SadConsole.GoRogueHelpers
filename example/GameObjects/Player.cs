using GoRogue;
using Microsoft.Xna.Framework;
using SadConsole.Tiles;

namespace BasicTutorial.GameObjects
{
    internal class Player : GameObjects.LivingCharacter
    {
        public Player(TileMap map, Coord position) : base(map, position, Color.Green, Color.Black, 1) { }
    }
}
