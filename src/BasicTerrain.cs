using GoRogue;
using GoRogue.GameFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SadConsole
{
	/// <summary>
	/// A GoRogue.GameFramework.GameObject that also inherits from SadConsole.Cell, that can be used to represent terrain
	/// on a map.
	/// </summary>
	public class BasicTerrain : Cell, IGameObject
	{
		private IGameObject _backingField;

		#region Constructors
		public BasicTerrain(Coord position, bool isWalkable, bool isTransparent)
			: base()
		{
			InitializeBackingField(position, isWalkable, isTransparent);
		}

		public BasicTerrain(Color foreground, Coord position, bool isWalkable, bool isTransparent)
			: base(foreground)
		{
			InitializeBackingField(position, isWalkable, isTransparent);
		}

		public BasicTerrain(Color foreground, Color background, Coord position, bool isWalkable, bool isTransparent)
			: base(foreground, background)
		{
			InitializeBackingField(position, isWalkable, isTransparent);
		}

		public BasicTerrain(Color foreground, Color background, int glyph, Coord position, bool isWalkable, bool isTransparent)
			: base(foreground, background, glyph)
		{
			InitializeBackingField(position, isWalkable, isTransparent);
		}

		public BasicTerrain(Color foreground, Color background, int glyph, SpriteEffects mirror, Coord position, bool isWalkable, bool isTransparent)
			: base(foreground, background, glyph, mirror)
		{
			InitializeBackingField(position, isWalkable, isTransparent);
		}

		private void InitializeBackingField(Coord position, bool isWalkable, bool isTransparent)
		{
			_backingField = new GameObject(position, 0, this, true, isWalkable, isTransparent);
		}
		#endregion Constructors

		#region IGameObject Implementation

		public event EventHandler<ItemMovedEventArgs<IGameObject>> Moved
		{
			add => _backingField.Moved += value;
			remove => _backingField.Moved -= value;
		}

		Map IGameObject.CurrentMap => _backingField.CurrentMap;
		public BasicMap CurrentMap => (BasicMap)_backingField.CurrentMap;

		public uint ID => _backingField.ID;
		public bool IsStatic => _backingField.IsStatic;

		public virtual bool IsTransparent { get => _backingField.IsTransparent; set => _backingField.IsTransparent = value; }
		public virtual bool IsWalkable { get => _backingField.IsWalkable; set => _backingField.IsWalkable = value; }
		public int Layer => _backingField.Layer;
		public Coord Position { get => _backingField.Position; set => _backingField.Position = value; }

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
