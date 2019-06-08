using System;
using System.Linq;

namespace SadConsole
{
    /// <summary>
    /// Implements Update logic for implementing "game frames".  The <see cref="Update"/> method should be called
    /// from a SadConosle Update method to process its update logic.
    /// </summary>
    /// <remarks>
    /// Each time Update is called, if <see cref="RunLogicFrame"/> is true, all entities that are _not_ the <see cref="BasicMap.ControlledGameObject"/>
    /// will, if they have any <see cref="Components.GoRogue.GameFrameProcessor"/> components, have those component's ProcessGameFrame function called.
    /// Then, the controlled game object will have its components ProcessGameFrame functions called.  Finally, the <see cref="RunLogicFrame"/> value
    /// is set to false, <see cref="LogicFrameCompleted"/> event is fired.
    /// </remarks>
    public class GameFrameManager : SadConsole.Components.UpdateConsoleComponent
    {
        /// <summary>
        /// Set to true to run a logic frame during the next Update call.
        /// </summary>
        public bool RunLogicFrame;

        /// <summary>
        /// Fires whenever a logic frame completes.
        /// </summary>
        public event EventHandler LogicFrameCompleted;

        /// <summary>
        /// Map that this manager runs game frames for.
        /// </summary>
        public BasicMap Map { get; }

        /// <summary>
        /// Creates a GameFrameManager that runs logic frames for the given map.
        /// </summary>
        /// <param name="map">The map to run logic frames for.</param>
        public GameFrameManager(BasicMap map)
        {
            Map = map;
        }

        /// <summary>
        /// Runs a single logic frame if <see cref="RunLogicFrame"/> is set to true.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="delta"></param>
        public override void Update(Console console, TimeSpan delta)
        {
            // Run logic if valid move made by player
            if (RunLogicFrame)
            {
                foreach (var ent in Map.Entities.Items.Where(e => e.HasComponent<Components.GoRogue.GameFrameProcessor>()))
                {
                    if (ent != Map.ControlledGameObject)
                        foreach (var processor in ent.GetComponents<Components.GoRogue.GameFrameProcessor>())
                            processor.ProcessGameFrame();
                }

                // Process player (though it was proc in the previous loop) to make sure they are last to be processed
                if (Map.ControlledGameObject.HasComponent<Components.GoRogue.GameFrameProcessor>())
                    foreach (var processor in Map.ControlledGameObject.GetComponents<Components.GoRogue.GameFrameProcessor>())
                        processor.ProcessGameFrame();

                RunLogicFrame = false;

                LogicFrameCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
