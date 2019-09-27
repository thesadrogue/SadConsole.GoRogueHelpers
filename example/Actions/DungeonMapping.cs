using System;

namespace SadConsole.Actions
{
    internal class DungeonMapping : ActionBase
    {
        public override void Run(TimeSpan timeElapsed) =>
            //Program.AdventureScreen.MessageScreen.Print("You gain Dungeon Mapping!", Screens.Messages.MessageTypes.Warning);

            //foreach (var tile in Program.AdventureScreen.Map.Tiles)
            //{
            //    tile.SetFlag(Tiles.Flags.Seen);
            //}

            Finish(ActionResult.Success);
    }
}
