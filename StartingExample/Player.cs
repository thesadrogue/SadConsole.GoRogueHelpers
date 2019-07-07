using GoRogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using System.Collections.Generic;

namespace StartingExample
{
    // Custom class for the player just so we can handle input.  This could be done via a component, or in a main screen, but for simplicity we do it here.
    class Player : BasicEntity
    {
        private static readonly Dictionary<Keys, Direction> _movementDirectionMapping = new Dictionary<Keys, Direction>
        {
            { Keys.NumPad7, Direction.UP_LEFT }, { Keys.NumPad8, Direction.UP }, { Keys.NumPad9, Direction.UP_RIGHT },
            { Keys.NumPad4, Direction.LEFT }, { Keys.NumPad6, Direction.RIGHT },
            { Keys.NumPad1, Direction.DOWN_LEFT }, { Keys.NumPad2, Direction.DOWN }, { Keys.NumPad3, Direction.DOWN_RIGHT }
        };

        public int FOVRadius;

        public Player(Coord position)
            : base(Color.White, Color.Transparent, '@', position, (int)MapLayer.PLAYER, isWalkable: false, isTransparent: true)
        {
            FOVRadius = 10;
        }


        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            Direction moveDirection = Direction.NONE;

            // Simplified way to check if any key we care about is pressed and set movement direction.
            foreach (var key in _movementDirectionMapping.Keys)
                if (info.IsKeyPressed(key))
                {
                    moveDirection = _movementDirectionMapping[key];
                    break;
                }

            Position += moveDirection;

            if (moveDirection != Direction.NONE)
                return true;
            else
                return base.ProcessKeyboard(info);
        }

    }
}
