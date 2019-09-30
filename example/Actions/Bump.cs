using System;

namespace SadConsole.Actions
{
    internal class BumpTile : ActionBase<BasicEntity, Tiles.Tile>
    {
        public BumpTile(BasicEntity source, Tiles.Tile target) : base(source, target) { }

        public override void Run(TimeSpan timeElapsed)
        {
            // Assume that this bump is a failure
            Finish(ActionResult.Failure);

            // Tell the tile to process this bump. The tile may set Finish to success or failure.
            Target.ProcessAction(this);
        }
    }

    internal class BumpGameObject : ActionBase<BasicEntity, BasicEntity>
    {
        public BumpGameObject(BasicEntity source, BasicEntity target) : base(source, target) { }

        public override void Run(TimeSpan timeElapsed)
        {
            // Assume that this bump is a failure
            Finish(ActionResult.Failure);

            // Tell the entity to process this bump. The entity may set Finish to success or failure.
            foreach (Components.GoRogue.IActionProcessor processor in Target.GetComponents<Components.GoRogue.IActionProcessor>())
            {
                processor.ProcessAction(this);
            }
        }
    }
}
