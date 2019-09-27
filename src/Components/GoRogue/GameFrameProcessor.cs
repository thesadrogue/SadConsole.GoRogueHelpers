using GoRogue.GameFramework;

namespace SadConsole.Components.GoRogue
{
    /// <summary>
    /// Component that can be attached to entities (non-terrain objects) that process game frames.
    /// </summary>
    public abstract class GameFrameProcessor : ComponentBase
    {
        /// <summary>
        /// The IGameObject that this GameFrameProcessor is attached to.
        /// </summary>
        public override IGameObject Parent
        {
            get => base.Parent;
            set
            {
                if (value.Layer == 0)
                {
                    throw new System.Exception($"Cannot add {nameof(GameFrameProcessor)} component to terrain objects.");
                }

                base.Parent = value;
            }
        }

        /// <summary>
        /// Implements logic for processing a logic frame.
        /// </summary>
        public abstract void ProcessGameFrame();
    }
}
