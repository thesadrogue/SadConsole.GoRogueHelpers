using GoRogue.GameFramework;
using GoRogue.GameFramework.Components;

namespace SadConsole.Components.GoRogue
{
	/// <summary>
	/// Component that can be attached to entities (non-terrain objects) that process game frames.
	/// </summary>
	public abstract class GameFrameProcessor : IGameObjectComponent
	{
		private IGameObject _parent;
		public IGameObject Parent
		{
			get => _parent;
			set
			{
				if (value.Layer == 0)
					throw new System.Exception($"Cannot add {nameof(GameFrameProcessor)} component to terrain objects.");

				_parent = value;
			}
		}

		public abstract void ProcessGameFrame();
	}
}
