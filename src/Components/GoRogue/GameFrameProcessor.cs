using GoRogue.GameFramework;

namespace SadConsole.Components.GoRogue
{
    /// <summary>
    /// Interface for a component that can be attached to entities (non-terrain objects) that process game frames.
    /// </summary>
    public interface IGameFrameProcessor
    {
        /// <summary>
        /// Implements logic for processing a logic frame.
        /// </summary>
        void ProcessGameFrame();
    }

    /// <summary>
    /// Component that can be attached to entities (non-terrain objects) that process game frames and enforces that its parent is of a particular type.
    /// </summary>
    public abstract class GameFrameProcessor<TParent> : ComponentBase<TParent>, IGameFrameProcessor where TParent : IGameObject
    {
        /// <summary>
        /// The IGameObject that this GameFrameProcessor is attached to.
        /// </summary>
        public override TParent Parent
        {
            get => base.Parent;
            set
            {
                if (value.Layer == 0)
                {
                    throw new System.Exception($"Cannot add {nameof(IGameFrameProcessor)} component to terrain objects.");
                }

                base.Parent = value;
            }
        }

        /// <summary>
        /// Implements logic for processing a logic frame.
        /// </summary>
        public abstract void ProcessGameFrame();
    }

    /// <summary>
    /// Component that can be attached to entities (non-terrain objects) that process game frames
    /// </summary>
    public abstract class GameFrameProcessor : GameFrameProcessor<IGameObject> { }
}
