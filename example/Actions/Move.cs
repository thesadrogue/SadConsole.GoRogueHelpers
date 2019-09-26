using System;
using GoRogue;

namespace SadConsole.Actions
{
    internal class Move : ActionBase
    {
        public static Move MoveBy(BasicEntity source, Direction change) => new Move() { Source = source, PositionChange = change };

        //public Maps.MapConsole Map;
        public BasicEntity Source;
        public Direction PositionChange;
        public Coord TargetPosition;

        public override void Run(TimeSpan timeElapsed)
        {
            TargetPosition = Source.Position + PositionChange;

            if (TargetPosition == Source.Position)
            {
                return;
            }

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
                    foreach (BasicEntity item in Source.CurrentMap.GetEntities<BasicEntity>(TargetPosition)) // Something blocked us
                    {
                        BumpGameObject bump = new BumpGameObject(Source, item);
                        BasicTutorial.GameState.Dungeon.ActionProcessor.PushAndRun(bump);
                    }
                }
            }
            else if (Source == ((Tiles.TileMap)Source.CurrentMap).ControlledGameObject) // We are the player
            {
                if (PositionChange == Direction.LEFT)
                {
                    BasicTutorial.GameState.Dungeon.Messages.Print("You move west.", BasicTutorial.MessageConsole.MessageTypes.Status);
                }
                else if (PositionChange == Direction.RIGHT)
                {
                    BasicTutorial.GameState.Dungeon.Messages.Print("You move east.", BasicTutorial.MessageConsole.MessageTypes.Status);
                }
                else if (PositionChange == Direction.UP)
                {
                    BasicTutorial.GameState.Dungeon.Messages.Print("You move north.", BasicTutorial.MessageConsole.MessageTypes.Status);
                }
                else if (PositionChange == Direction.DOWN)
                {
                    BasicTutorial.GameState.Dungeon.Messages.Print("You move south.", BasicTutorial.MessageConsole.MessageTypes.Status);
                }
            }

            Finish(ActionResult.Success);
        }
    }

}
