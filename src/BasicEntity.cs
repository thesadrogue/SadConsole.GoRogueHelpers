using GoRogue;
using GoRogue.GameFramework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SadConsole
{
	/// <summary>
	/// A GoRogue.GameFramework.GameObject that also inherits from SadConsole.Entities.Entity, that can be used to represent
	/// non-terrain objects on a map.
	/// </summary>
	public class BasicEntity : Entities.Entity, IGameObject
	{
		private IGameObject _backingField;

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
		}

		#endregion Constructors

		// Handle the case where GoRogue's Position property was the one that initiated the move
		private void GoRogue_Moved(object sender, ItemMovedEventArgs<IGameObject> e)
		{
			if (Position != base.Position) // We need to sync entity
				base.Position = Position;

			// SadConsole's Entity position set can't fail so no need to check for success here
		}

		// Handle the case where SadConsole's Position property was the one that initiated the move
		private void SadConsole_Moved(object sender, EntityMovedEventArgs e)
		{
			if (Position != base.Position)
			{
				Position = base.Position;

				// GoRogue wouldn't allow the position set, so set SadConsole's position back to the way it was
				// to keep them in sync.  Since GoRogue's position never changed, this won't infinite loop.
				if (Position != base.Position)
					base.Position = Position;
			}
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
