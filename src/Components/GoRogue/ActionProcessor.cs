using SadConsole.Actions;
using GoRogue.GameFramework;

namespace SadConsole.Components.GoRogue
{
    /// <summary>
    /// Interface for a component that can be attached to any IGameObject that needs to process one or more actions.
    /// </summary>
    public interface IActionProcessor
    {
        /// <summary>
        /// Implements action processing logic.
        /// </summary>
        /// <param name="action">The action to process.</param>
        void ProcessAction(ActionBase action);
    }

    /// <summary>
    /// Component that can be attached to any IGameObject that needs to process one or more actions.
    /// </summary>
    public abstract class ActionProcessor : ActionProcessor<IGameObject> { }

    /// <summary>
    /// Component that can be attached to any IGameObject that needs to process one or more actions and enforces that its parent is of a particular type.
    /// </summary>
    public abstract class ActionProcessor<TParent> : ComponentBase<TParent>, IActionProcessor where TParent : IGameObject
    {
        /// <summary>
        /// Implements action processing logic.
        /// </summary>
        /// <param name="action">The action to process.</param>
        public abstract void ProcessAction(ActionBase action);
    }
}
