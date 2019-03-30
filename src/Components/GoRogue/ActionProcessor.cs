using GoRogue.GameFramework;
using GoRogue.GameFramework.Components;
using SadConsole.Actions;

namespace SadConsole.Components.GoRogue
{
	/// <summary>
	/// Component that can be attached to any gameobject that needs to processing one or more actions.
	/// </summary>
	public abstract class ActionProcessor : IGameObjectComponent
	{
		public IGameObject Parent { get; set; }

		public abstract void ProcessAction(ActionBase action);
	}
}
