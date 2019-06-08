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
		/// <summary>
		/// Creates a terrain object with a white foreground, black background, glyph 0, and no mirror effect.
		/// </summary>
		/// <param name="position">Position of the terrain object.</param>
		/// <param name="isWalkable">Whether or not the terrain object should be considered passable for the sake of collision detection.</param>
		/// <param name="isTransparent">Whether or not the terrain object should be considered transparent for the sake of FOV.</param>
		public BasicTerrain(Coord position, bool isWalkable, bool isTransparent)
			: base()
		{
			InitializeBackingField(position, isWalkable, isTransparent);
		}

		/// <summary>
		/// Creates a terrain object with the specified foreground, black background, glyph 0, and no mirror effect.
		/// </summary>
		/// <param name="foreground">Foreground color.</param>
		/// <param name="position">Position of the terrain object.</param>
		/// <param name="isWalkable">Whether or not the terrain object should be considered passable for the sake of collision detection.</param>
		/// <param name="isTransparent">Whether or not the terrain object should be considered transparent for the sake of FOV.</param>
		public BasicTerrain(Color foreground, Coord position, bool isWalkable, bool isTransparent)
			: base(foreground)
		{
			InitializeBackingField(position, isWalkable, isTransparent);
		}

		/// <summary>
		/// Creates a terrain object with the specified foreground, specified background, glyph 0, and no mirror effect.
		/// </summary>
		/// <param name="foreground">Foreground color.</param>
		/// <param name="background">Background color.</param>
		/// <param name="position">Position of the terrain object.</param>
		/// <param name="isWalkable">Whether or not the terrain object should be considered passable for the sake of collision detection.</param>
		/// <param name="isTransparent">Whether or not the terrain object should be considered transparent for the sake of FOV.</param>
		public BasicTerrain(Color foreground, Color background, Coord position, bool isWalkable, bool isTransparent)
			: base(foreground, background)
		{
			InitializeBackingField(position, isWalkable, isTransparent);
		}

		/// <summary>
		/// Creates a terrain object with the specified foreground, background, and glyph, with no mirror effect.
		/// </summary>
		/// <param name="foreground">Foreground color.</param>
		/// <param name="background">Background color.</param>
		/// <param name="glyph">The glyph index.</param>
		/// <param name="position">Position of the terrain object.</param>
		/// <param name="isWalkable">Whether or not the terrain object should be considered passable for the sake of collision detection.</param>
		/// <param name="isTransparent">Whether or not the terrain object should be considered transparent for the sake of FOV.</param>
		public BasicTerrain(Color foreground, Color background, int glyph, Coord position, bool isWalkable, bool isTransparent)
			: base(foreground, background, glyph)
		{
			InitializeBackingField(position, isWalkable, isTransparent);
		}

		/// <summary>
		/// Creates a terrain object with the specified foreground, background, glyph, and mirror effect.
		/// </summary>
		/// <param name="foreground">Foreground color.</param>
		/// <param name="background">Background color.</param>
		/// <param name="glyph">The glyph index.</param>
		/// <param name="mirror">Mirror effect to use.</param>
		/// <param name="position">Position of the terrain object.</param>
		/// <param name="isWalkable">Whether or not the terrain object should be considered passable for the sake of collision detection.</param>
		/// <param name="isTransparent">Whether or not the terrain object should be considered transparent for the sake of FOV.</param>
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
		/// <inheritdoc />
		public event EventHandler<ItemMovedEventArgs<IGameObject>> Moved
		{
			add => _backingField.Moved += value;
			remove => _backingField.Moved -= value;
		}

		/// <inheritdoc />
		Map IGameObject.CurrentMap => _backingField.CurrentMap;

		/// <summary>
		/// The current map this Entity is added to.
		/// </summary>
		public BasicMap CurrentMap => (BasicMap)_backingField.CurrentMap;

		/// <inheritdoc />
		public uint ID => _backingField.ID;

		/// <inheritdoc />
		public bool IsStatic => _backingField.IsStatic;

		/// <inheritdoc />
		public virtual bool IsTransparent { get => _backingField.IsTransparent; set => _backingField.IsTransparent = value; }

		/// <inheritdoc />
		public virtual bool IsWalkable { get => _backingField.IsWalkable; set => _backingField.IsWalkable = value; }

		/// <inheritdoc />
		public int Layer => _backingField.Layer;

		/// <inheritdoc />
		public Coord Position { get => _backingField.Position; set => _backingField.Position = value; }

		/// <inheritdoc />
		public void AddComponent(object component) => _backingField.AddComponent(component);

		/// <inheritdoc />
		public T GetComponent<T>() => _backingField.GetComponent<T>();

		/// <inheritdoc />
		public IEnumerable<T> GetComponents<T>() => _backingField.GetComponents<T>();

		/// <inheritdoc />
		public bool HasComponent(Type componentType) => _backingField.HasComponent(componentType);

		/// <inheritdoc />
		public bool HasComponent<T>() => _backingField.HasComponent<T>();

		/// <inheritdoc />
		public bool HasComponents(params Type[] componentTypes) => _backingField.HasComponents(componentTypes);

		/// <inheritdoc />
		public bool MoveIn(Direction direction) => _backingField.MoveIn(direction);

		/// <inheritdoc />
		public void OnMapChanged(GoRogue.GameFramework.Map newMap) => _backingField.OnMapChanged(newMap);

		/// <inheritdoc />
		public void RemoveComponent(object component) => _backingField.RemoveComponent(component);

		/// <inheritdoc />
		public void RemoveComponents(params object[] components) => _backingField.RemoveComponents(components);

		#endregion IGameObject Implementation
	}
}
