using GoRogue.GameFramework;
using GoRogue.GameFramework.Components;

namespace SadConsole.Components.GoRogue
{
	/// <summary>
	/// Component that can be attached to entities (non-terrain objects) that process game frames.
	/// </summary>
	public abstract class GameFrameProcessor : IGameObjectComponent
	{
		public IGameObject Parent { get; set; }

		public abstract void ProcessGameFrame();
	}
}
