using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SadConsole.Entities;
using SadConsole.Maps;
using SadConsole;
using SadConsole.Actions;
using SadConsole.Tiles;
using GoRogue;

namespace BasicTutorial.GameObjects
{
    class Player : GameObjects.LivingCharacter
    {
        public Player(TileMap map, Coord position): base(map, position, Color.Green, Color.Black, 1) { }
    }
}
