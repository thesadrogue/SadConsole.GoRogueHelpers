using System;

namespace SadConsole.Actions
{
    internal class DungeonSight : ActionBase
    {
        public override void Run(TimeSpan timeElapsed) =>
            //Program.AdventureScreen.MessageScreen.Print("You gain Dungeon Sight!", Screens.Messages.MessageTypes.Warning);

            //foreach (var tile in Program.AdventureScreen.Map.Tiles)
            //{
            //    tile.SetFlag(Tiles.Flags.Seen | Tiles.Flags.PermaLight | Tiles.Flags.PermaInLOS);
            //}

            Finish(ActionResult.Success);
    }
}
