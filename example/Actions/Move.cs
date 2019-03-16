using GoRogue;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Entities;

namespace SadConsole.Actions
{
    class Move : ActionBase
    {
        public static Move MoveBy(ActionBasedEntity source, Direction change)
        {
            return new Move() { Source = source, PositionChange = change };
        }

        //public Maps.MapConsole Map;
        public ActionBasedEntity Source;
        public Direction PositionChange;
        public Coord TargetPosition;

        public override void Run(TimeSpan timeElapsed)
        {
            TargetPosition = Source.Position + PositionChange;

            if (TargetPosition == Source.Position)
                return;

			bool moved = Source.MoveIn(PositionChange);
			if (!moved)
			{
				Tiles.Tile tile = Source.CurrentMap.GetTerrain<Tiles.Tile>(TargetPosition);
				if (!tile.IsWalkable)
				{
					BumpTile bump = new BumpTile(Source, tile);
					BasicTutorial.GameState.Dungeon.ActionProcessor.PushAndRun(bump);
				}
				else
				{
					foreach (var item in Source.CurrentMap.GetEntities<ActionBasedEntity>(TargetPosition)) // Something blocked us
					{
						BumpGameObject bump = new BumpGameObject(Source, item);
						BasicTutorial.GameState.Dungeon.ActionProcessor.PushAndRun(bump);
					}
				}
			}
			else if (Source == ((Tiles.TileMap)Source.CurrentMap).ControlledGameObject) // We are the player
			{
				if (PositionChange == Direction.LEFT)
					BasicTutorial.GameState.Dungeon.Messages.Print("You move west.", BasicTutorial.MessageConsole.MessageTypes.Status);
				else if (PositionChange == Direction.RIGHT)
					BasicTutorial.GameState.Dungeon.Messages.Print("You move east.", BasicTutorial.MessageConsole.MessageTypes.Status);
				else if (PositionChange == Direction.UP)
					BasicTutorial.GameState.Dungeon.Messages.Print("You move north.", BasicTutorial.MessageConsole.MessageTypes.Status);
				else if (PositionChange == Direction.DOWN)
					BasicTutorial.GameState.Dungeon.Messages.Print("You move south.", BasicTutorial.MessageConsole.MessageTypes.Status);
			}

            Finish(ActionResult.Success);
        }
    }

}
