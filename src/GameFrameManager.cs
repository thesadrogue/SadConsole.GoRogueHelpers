using System;
using System.Linq;

namespace SadConsole
{
    /// <summary>
    /// Implements Update logic for implementing "game frames".
    /// </summary>
    /// <remarks>
    /// Each time Update is called, if <see cref="RunLogicFrame"/> is true, all entities that are _not_ the <see cref="BasicMap.ControlledGameObject"/>
    /// will, if they have any <see cref="Components.GoRogue.GameFrameProcessor"/> components, have those component's ProcessGameFrame function called.
    /// Then, the controlled game object will have its components ProcessGameFrame functions called (if it has a GameFrameProcessor component).
    /// Finally, the <see cref="RunLogicFrame"/> value is set to false, and the <see cref="LogicFrameCompleted"/> event is fired.
    /// </remarks>
    public class GameFrameManager : SadConsole.Components.UpdateConsoleComponent
    {
        /// <summary>
        /// Set to true to run a logic frame during the next Update call.  Can set to false in the middle of a logic frame to terminate after the current processor exits.
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
        public GameFrameManager(BasicMap map) => Map = map;

        /// <summary>
        /// Runs a single logic frame if <see cref="RunLogicFrame"/> is set to true.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="delta"></param>
        public override void Update(Console console, TimeSpan delta)
        {
            // Run GameFrame logic
            if (RunLogicFrame)
            {
                foreach (GoRogue.GameFramework.IGameObject ent in Map.Entities.Items.Where(e => e.HasComponent<Components.GoRogue.GameFrameProcessor>()))
                {
                    if (ent != Map.ControlledGameObject)
                    {
                        foreach (Components.GoRogue.GameFrameProcessor processor in ent.GetComponents<Components.GoRogue.GameFrameProcessor>())
                        {
                            processor.ProcessGameFrame();
                            if (!RunLogicFrame)
                            {
                                LogicFrameCompleted?.Invoke(this, EventArgs.Empty);
                                return;
                            }
                        }
                    }
                }

                // Process player to make sure they are last to be processed
                foreach (Components.GoRogue.GameFrameProcessor processor in Map.ControlledGameObject.GetGoRogueComponents<Components.GoRogue.GameFrameProcessor>())
                {
                    processor.ProcessGameFrame();
                    if (!RunLogicFrame)
                    {
                        LogicFrameCompleted?.Invoke(this, EventArgs.Empty);
                        return;
                    }
                }

                RunLogicFrame = false;

                LogicFrameCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
