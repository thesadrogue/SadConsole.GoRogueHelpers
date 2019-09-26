using SadConsole.Actions;

namespace SadConsole.Components.GoRogue
{
    /// <summary>
    /// Component that can be attached to any IGameObject that needs to process one or more actions.
    /// </summary>
    public abstract class ActionProcessor : ComponentBase
    {
        /// <summary>
        /// Implements action processing logic.
        /// </summary>
        /// <param name="action">The action to process.</param>
        public abstract void ProcessAction(ActionBase action);
    }
}
