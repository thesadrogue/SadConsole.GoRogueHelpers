using System;
using System.Collections.Generic;
using GoRogue;
using GoRogue.GameFramework;
using Microsoft.Xna.Framework;
using SadConsole.Components;

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
            : base(foreground, background, glyph) => Initialize(position, layer, isWalkable, isTransparent);

        /// <summary>
        /// Creates a new, invisible entity using the given parameters and the default font.
        /// </summary>
        /// <param name="position">Position to create the entity at.</param>
        /// <param name="layer">The layer of of a <see cref="BasicEntity"/> the object should be assigned to.</param>
        /// <param name="isWalkable">Whether or not this object collides with other non-walkable objects in the same <see cref="BasicMap"/>.</param>
        /// <param name="isTransparent">Whether or not this object is considered blocking line of sight for the sake of FOV.</param>
        public BasicEntity(Coord position, int layer, bool isWalkable, bool isTransparent)
            : base(1, 1) => Initialize(position, layer, isWalkable, isTransparent);

        /// <summary>
        /// Creates a new, invisible entity using the given parameters and the specified font.
        /// </summary>
        /// <param name="font">The font to use to display the entity.</param>
        /// <param name="position">Position to create the entity at.</param>
        /// <param name="layer">The layer of of a <see cref="BasicEntity"/> the object should be assigned to.</param>
        /// <param name="isWalkable">Whether or not this object collides with other non-walkable objects in the same <see cref="BasicMap"/>.</param>
        /// <param name="isTransparent">Whether or not this object is considered blocking line of sight for the sake of FOV.</param>
        public BasicEntity(Font font, Coord position, int layer, bool isWalkable, bool isTransparent)
            : base(1, 1, font) => Initialize(position, layer, isWalkable, isTransparent);

        /// <summary>
        /// Creates a new entity with the given parameters that displays the specified animation.
        /// </summary>
        /// <param name="animation">Animation that the entity should display.</param>
        /// <param name="position">Position to create the entity at.</param>
        /// <param name="layer">The layer of of a <see cref="BasicEntity"/> the object should be assigned to.</param>
        /// <param name="isWalkable">Whether or not this object collides with other non-walkable objects in the same <see cref="BasicMap"/>.</param>
        /// <param name="isTransparent">Whether or not this object is considered blocking line of sight for the sake of FOV.</param>
        public BasicEntity(AnimatedConsole animation, Coord position, int layer, bool isWalkable, bool isTransparent)
            : base(animation) => Initialize(position, layer, isWalkable, isTransparent);

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

        #region Component Forwarder Functions
        /// <summary>
        /// Gets the first SadConsole Console component of the specified type.
        /// </summary>
        /// <typeparam name="TComponent">THe component to find</typeparam>
        /// <returns>The component if found, otherwise null.</returns>
        public IConsoleComponent GetConsoleComponent<TComponent>() where TComponent : IConsoleComponent => GetComponent<TComponent>();

        /// <summary>
        /// Gets SadConosle Console components of the specified types.
        /// </summary>
        /// <typeparam name="TComponent">The component to find</typeparam>
        /// <returns>The components found.</returns>
        public IEnumerable<IConsoleComponent> GetConsoleComponents<TComponent>() where TComponent : IConsoleComponent => GetComponents<TComponent>();

        /// <summary>
		/// Adds the given object as a GoRogue component.  Throws ArgumentException if the specific instance has already been added.
		/// </summary>
		/// <param name="component">Component to add.</param>
        public void AddGoRogueComponent(object component) => ((IGameObject)this).AddComponent(component);

        /// <summary>
		/// Gets the first GoRogue component of type T that was added, or default(T) if no component of that type has
		/// been added.
		/// </summary>
		/// <typeparam name="T">Type of component to retrieve.</typeparam>
		/// <returns>The first GoRogue component of Type T that was attached, or default(T) if no components of the given type
		/// have been attached.</returns>
		public T GetGoRogueComponent<T>() => ((IGameObject)this).GetComponent<T>();

        /// <summary>
		/// Gets all GoRogue components of type T that are added.
		/// </summary>
		/// <typeparam name="T">Type of components to retrieve.</typeparam>
		/// <returns>All GoRogue components of Type T that are attached.</returns>
		public IEnumerable<T> GetGoRogueComponents<T>() => ((IGameObject)this).GetComponents<T>();

        /// <summary>
		/// Returns whether or not there is at least one GoRogue component of the specified type attached.  Type may be specified
		/// by using typeof(MyComponentType).
		/// </summary>
		/// <param name="componentType">The type of component to check for.</param>
		/// <returns>True if the implementer has at least one GoRogue component of the specified type, false otherwise.</returns>
		public bool HasGoRogueComponent(Type componentType) => ((IGameObject)this).HasComponent(componentType);

        /// <summary>
		/// Returns whether or not there is at least one GoRogue component of type T attached.
		/// </summary>
		/// <typeparam name="T">Type of component to check for.</typeparam>
		/// <returns>True if the implemented has at least one GoRogue component of the specified type attached, false otherwise.</returns>
		public bool HasGoRogueComponent<T>() => ((IGameObject)this).HasComponent<T>();

        /// <summary>
		/// Returns whether or not the implementer has at least one of all of the given types of GoRogue components attached.  Types may be specified by
		/// using typeof(MyComponentType)
		/// </summary>
		/// <param name="componentTypes">One or more component types to check for.</param>
		/// <returns>True if the implementer has at least one GoRogue component of each specified type, false otherwise.</returns>
		public bool HasGoRogueComponents(params Type[] componentTypes) => ((IGameObject)this).HasComponents(componentTypes);

        /// <summary>
		/// Removes the given GoRogue component.  Throws an exception if the component isn't attached to the GoRogue GameObject component system.
		/// </summary>
		/// <param name="component">GoRogue component to remove.</param>
		public void RemoveGoRogueComponent(object component) => ((IGameObject)this).RemoveComponent(component);

        /// <summary>
		/// Removes the given GoRogue component(s).  Throws an exception if a component given isn't attached to the GoRogue GameObject component system.
		/// </summary>
		/// <param name="components">One or more GoRogue component instances to remove.</param>
		public void RemoveGoRogueComponents(params object[] components) => ((IGameObject)this).RemoveComponents(components);
        #endregion

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
        void IHasComponents.AddComponent(object component) => _backingField.AddComponent(component);

        /// <inheritdoc />
        T IHasComponents.GetComponent<T>() => _backingField.GetComponent<T>();

        /// <inheritdoc />
        IEnumerable<T> IHasComponents.GetComponents<T>() => _backingField.GetComponents<T>();

        /// <inheritdoc />
        bool IHasComponents.HasComponent(Type componentType) => _backingField.HasComponent(componentType);

        /// <inheritdoc />
        bool IHasComponents.HasComponent<T>() => _backingField.HasComponent<T>();

        /// <inheritdoc />
        bool IHasComponents.HasComponents(params Type[] componentTypes) => _backingField.HasComponents(componentTypes);

        /// <inheritdoc />
        public bool MoveIn(Direction direction) => _backingField.MoveIn(direction);

        /// <inheritdoc />
        public void OnMapChanged(GoRogue.GameFramework.Map newMap) => _backingField.OnMapChanged(newMap);

        /// <inheritdoc />
        void IHasComponents.RemoveComponent(object component) => _backingField.RemoveComponent(component);

        /// <inheritdoc />
        void IHasComponents.RemoveComponents(params object[] components) => _backingField.RemoveComponents(components);

        #endregion IGameObject Implementation
    }
}
