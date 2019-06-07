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
		public bool RunLogicFrame;

		public event EventHandler LogicFrameCompleted;

		public BasicMap Map { get; }

		public GameFrameManager(BasicMap map)
		{
			Map = map;
		}

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
