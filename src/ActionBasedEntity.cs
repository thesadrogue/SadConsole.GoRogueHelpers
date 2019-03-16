using GoRogue;
using Microsoft.Xna.Framework;
using SadConsole.Actions;

namespace SadConsole
{
	public class ActionBasedEntity : BasicEntity
	{
		/// <summary>
		/// Gets or sets a friendly short title for the object.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets a longer description for the object.
		/// </summary>
		public string Description { get; set; }
		#region Constructors
		public ActionBasedEntity(Color foreground, Color background, int glyph, Coord position, int layer, bool isWalkable, bool isTransparent)
			: base(foreground, background, glyph, position, layer, isWalkable, isTransparent)
		{
			Initialize();
		}

		public ActionBasedEntity(int width, int height, Coord position, int layer, bool isWalkable, bool isTransparent)
			: base(width, height, position, layer, isWalkable, isTransparent)
		{
			Initialize();
		}

		public ActionBasedEntity(int width, int height, Font font, Coord position, int layer, bool isWalkable, bool isTransparent)
			: base(width, height, font, position, layer, isWalkable, isTransparent)
		{
			Initialize();
		}

		public ActionBasedEntity(AnimatedConsole animation, Coord position, int layer, bool isWalkable, bool isTransparent)
			: base(animation, position, layer, isWalkable, isTransparent)
		{
			Initialize();
		}

		public virtual void ProcessAction(ActionBase command) { }

		public virtual void ProcessGameFrame() { }

		public virtual void OnDestroy() { }


		private void Initialize()
		{
			Title = "Unknown";
			Description = "Not much is known about this object.";
		}
		#endregion
	}
}
