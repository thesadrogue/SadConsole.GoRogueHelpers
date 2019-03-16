using GoRogue;
using GoRogue.GameFramework;
using Microsoft.Xna.Framework;
using SadConsole.Actions;
using System;
using System.Collections.Generic;

namespace SadConsole
{
// TODO: Action system isn't technically needed to make gorogue integration work... but it seems harmless enough to leave in,
// and can potentially be parsed out later
	public class BasicEntity : Entities.Entity, IGameObject
	{
		private IGameObject _backingField;

		/// <summary>
		/// Gets or sets a friendly short title for the object.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets a longer description for the object.
		/// </summary>
		public string Description { get; set; }

		#region Constructors

		public BasicEntity(Color foreground, Color background, int glyph, Coord position, int layer, bool isWalkable, bool isTransparent)
			: base(foreground, background, glyph)
		{
			Initialize(position, layer, isWalkable, isTransparent);
		}

		public BasicEntity(int width, int height, Coord position, int layer, bool isWalkable, bool isTransparent)
			: base(width, height)
		{
			Initialize(position, layer, isWalkable, isTransparent);
		}

		public BasicEntity(int width, int height, Font font, Coord position, int layer, bool isWalkable, bool isTransparent)
			: base(width, height, font)
		{
			Initialize(position, layer, isWalkable, isTransparent);
		}

		public BasicEntity(AnimatedConsole animation, Coord position, int layer, bool isWalkable, bool isTransparent)
			: base(animation)
		{
			Initialize(position, layer, isWalkable, isTransparent);
		}

		private void Initialize(Coord position, int layer, bool isWalkable, bool isTransparent)
		{
			_backingField = new GameObject(position, layer, this, false, isWalkable, isTransparent);
			base.Position = _backingField.Position;

			base.Moved += SadConsole_Moved;
			_backingField.Moved += GoRogue_Moved;

			Title = "Unknown";
			Description = "Not much is known about this object.";
		}

		#endregion Constructors

		public virtual void ProcessAction(ActionBase command) { }

		public virtual void ProcessGameFrame() { }

		public virtual void OnDestroy() { }

		// Handle the case where GoRogue's position was the one that initiated the move
		private void GoRogue_Moved(object sender, ItemMovedEventArgs<IGameObject> e)
		{
			// Casts are necessary for now due to GoRogue bug #131
			if ((Point)Position != base.Position) // We need to sync entity
				base.Position = Position;

			// SadConsole's Entity position set can't fail so no need to do other checks here
		}

		// Handle the case where you set the position when its casted to Entity
		private void SadConsole_Moved(object sender, EntityMovedEventArgs e)
		{
			// Casts are necessary for now due to GoRogue bug #131
			if ((Point)Position != base.Position)
				Position = base.Position;

			if ((Point)Position != base.Position) // GoRogue wouldn't allow the position set
				base.Position = Position; // Set it back.  This shouldn't infinite loop because Position is still equal to the old base.Position
		}

		#region IGameObject Implementation

		public new event EventHandler<ItemMovedEventArgs<IGameObject>> Moved
		{
			add => _backingField.Moved += value;
			remove => _backingField.Moved -= value;
		}

		Map IGameObject.CurrentMap => _backingField.CurrentMap;
		public BasicMap CurrentMap => (BasicMap)_backingField.CurrentMap;

		public uint ID => _backingField.ID;
		public bool IsStatic => _backingField.IsStatic;

		public bool IsTransparent { get => _backingField.IsTransparent; set => _backingField.IsTransparent = value; }
		public bool IsWalkable { get => _backingField.IsWalkable; set => _backingField.IsWalkable = value; }
		public int Layer => _backingField.Layer;
		public new Coord Position { get => _backingField.Position; set => _backingField.Position = value; }

		public void AddComponent(object component) => _backingField.AddComponent(component);

		public T GetComponent<T>() => _backingField.GetComponent<T>();

		public IEnumerable<T> GetComponents<T>() => _backingField.GetComponents<T>();

		public bool HasComponent(Type componentType) => _backingField.HasComponent(componentType);

		public bool HasComponent<T>() => _backingField.HasComponent<T>();

		public bool HasComponents(params Type[] componentTypes) => _backingField.HasComponents(componentTypes);

		public bool MoveIn(Direction direction) => _backingField.MoveIn(direction);

		public void OnMapChanged(GoRogue.GameFramework.Map newMap) => _backingField.OnMapChanged(newMap);

		public void RemoveComponent(object component) => _backingField.RemoveComponent(component);

		public void RemoveComponents(params object[] components) => _backingField.RemoveComponents(components);

		#endregion IGameObject Implementation
	}
}
