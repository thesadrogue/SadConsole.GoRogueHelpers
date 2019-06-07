using GoRogue;
using GoRogue.GameFramework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SadConsole
{
	/// <summary>
	/// A GoRogue.GameFramework.GameObject that also inherits from SadConsole.Entities.Entity, that can be used to represent
	/// non-terrain objects on a map rendered by SadConsole.
	/// </summary>
	public class BasicEntity : Entities.Entity, IGameObject
	{
		private IGameObject _backingField;

		#region Constructors

		/// <summary>
		/// Creates a new entity with the given parameters, displayed with the given foreground, background, and glyph.
		/// </summary>
		/// <param name="foreground">Foreground color the entity should use for display.</param>
		/// <param name="background">Background color the entity should use for display.</param>
		/// <param name="glyph">Glyph the entity should use for display.</param>
		/// <param name="position">Position to create the entity at.</param>
		/// <param name="layer">The layer of of a <see cref="BasicEntity"/> the object should be assigned to.</param>
		/// <param name="isWalkable">Whether or not this object collides with other non-walkable objects in the same <see cref="BasicMap"/>.</param>
		/// <param name="isTransparent">Whether or not this object is considered blocking line of sight for the sake of FOV.</param>
		public BasicEntity(Color foreground, Color background, int glyph, Coord position, int layer, bool isWalkable, bool isTransparent)
			: base(foreground, background, glyph)
		{
			Initialize(position, layer, isWalkable, isTransparent);
		}

		/// <summary>
		/// Creates a new, invisible entity using the given parameters and the default font.
		/// </summary>
		/// <param name="position">Position to create the entity at.</param>
		/// <param name="layer">The layer of of a <see cref="BasicEntity"/> the object should be assigned to.</param>
		/// <param name="isWalkable">Whether or not this object collides with other non-walkable objects in the same <see cref="BasicMap"/>.</param>
		/// <param name="isTransparent">Whether or not this object is considered blocking line of sight for the sake of FOV.</param>
		public BasicEntity(Coord position, int layer, bool isWalkable, bool isTransparent)
			: base(1, 1)
		{
			Initialize(position, layer, isWalkable, isTransparent);
		}

		/// <summary>
		/// Creates a new, invisible entity using the given parameters and the specified font.
		/// </summary>
		/// <param name="font">The font to use to display the entity.</param>
		/// <param name="position">Position to create the entity at.</param>
		/// <param name="layer">The layer of of a <see cref="BasicEntity"/> the object should be assigned to.</param>
		/// <param name="isWalkable">Whether or not this object collides with other non-walkable objects in the same <see cref="BasicMap"/>.</param>
		/// <param name="isTransparent">Whether or not this object is considered blocking line of sight for the sake of FOV.</param>
		public BasicEntity(Font font, Coord position, int layer, bool isWalkable, bool isTransparent)
			: base(1, 1, font)
		{
			Initialize(position, layer, isWalkable, isTransparent);
		}

		/// <summary>
		/// Creates a new entity with the given parameters that displays the specified animation.
		/// </summary>
		/// <param name="animation">Animation that the entity should display.</param>
		/// <param name="position">Position to create the entity at.</param>
		/// <param name="layer">The layer of of a <see cref="BasicEntity"/> the object should be assigned to.</param>
		/// <param name="isWalkable">Whether or not this object collides with other non-walkable objects in the same <see cref="BasicMap"/>.</param>
		/// <param name="isTransparent">Whether or not this object is considered blocking line of sight for the sake of FOV.</param>
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

				// In this case, GoRogue wouldn't allow the position set, so set SadConsole's position back to the way it was
				// to keep them in sync.  Since GoRogue's position never changed, this won't infinite loop.
				if (Position != base.Position)
					base.Position = Position;
			}
		}

		#region IGameObject Implementation

		/// <inheritdoc />
		public new event EventHandler<ItemMovedEventArgs<IGameObject>> Moved
		{
			add => _backingField.Moved += value;
			remove => _backingField.Moved -= value;
		}

		/// <inheritdoc />
		Map IGameObject.CurrentMap => _backingField.CurrentMap;

		/// <inheritdoc />
		public BasicMap CurrentMap => (BasicMap)_backingField.CurrentMap;

		/// <inheritdoc />
		public uint ID => _backingField.ID;

		/// <inheritdoc />
		public bool IsStatic => _backingField.IsStatic;

		/// <inheritdoc />
		public bool IsTransparent { get => _backingField.IsTransparent; set => _backingField.IsTransparent = value; }

		/// <inheritdoc />
		public bool IsWalkable { get => _backingField.IsWalkable; set => _backingField.IsWalkable = value; }

		/// <inheritdoc />
		public int Layer => _backingField.Layer;

		/// <inheritdoc />
		public new Coord Position { get => _backingField.Position; set => _backingField.Position = value; }

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
