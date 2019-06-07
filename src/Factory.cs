using GoRogue;
using GoRogue.MapViews;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SadConsole
{
    /// <summary>
    /// A factory that produces an object based on a blueprint.
    /// </summary>
    /// <typeparam name="TBlueprint">A settings object that will create an object defined by <typeparamref name="TProduced"/>.</typeparam>
    /// <typeparam name="TProduced">An object this factory creates.</typeparam>
    public abstract class Factory<TBlueprint, TProduced> : IEnumerable<TBlueprint>
        where TBlueprint : IFactoryBlueprint<TProduced>
        /*where TProduced : IFactoryObject*/
    {
        private readonly Dictionary<string, TBlueprint> _blueprints = new Dictionary<string, TBlueprint>();

        /// <summary>
        /// Creates a <typeparamref name="TProduced"/> object based on the blueprint factory id.
        /// </summary>
        /// <param name="name">The factory id of a blueprint.</param>
        /// <returns>A new object.</returns>
        public TProduced Create(string name)
        {
            if (!_blueprints.ContainsKey(name))
                throw new ItemNotDefinedException(name);

            return _blueprints[name].Create();
        }

        /// <summary>
        /// Adds a blueprint to the factory.
        /// </summary>
        /// <param name="item">The blueprint.</param>
        public void Add(TBlueprint item)
        {
            if (_blueprints.ContainsKey(item.Id))
                _blueprints.Remove(item.Id);

            _blueprints.Add(item.Id, item);
        }

        /// <summary>
        /// Checks if a blueprint exists.
        /// </summary>
        /// <param name="factoryId">The blueprint to check for.</param>
        /// <returns>Returns true when the specified <paramref name="factoryId"/> exists; otherwise false.</returns>
        public bool BlueprintExists(string factoryId)
        {
            return _blueprints.ContainsKey(factoryId);
        }

        /// <summary>
        /// Gets a blueprint by identifier.
        /// </summary>
        /// <param name="factoryId">The blueprint identifier to get.</param>
        /// <returns>The blueprint of the object.</returns>
        /// <exception cref="ItemNotDefinedException">Thrown if the factory identifier does not exist.</exception>
        public TBlueprint GetBlueprint(string factoryId)
        {
            if (!_blueprints.ContainsKey(factoryId))
                throw new ItemNotDefinedException(factoryId);

            return _blueprints[factoryId];
        }

		/// <summary>
		/// Gets an enumerator of all of the blueprints in the factory.
		/// </summary>
		/// <returns>An enumeration of the blueprints.</returns>
		public IEnumerator<TBlueprint> GetEnumerator()
        {
            return _blueprints.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator of all of the blueprints in the factory.
        /// </summary>
        /// <returns>An enumeration of the blueprints.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _blueprints.Values.GetEnumerator();
        }

        [Serializable]
        public class ItemNotDefinedException : Exception
        {
            public ItemNotDefinedException(string factoryId)
                : base($"The item '{factoryId}' has not been added to this factory")
            {
            }
        }
    }

	public static class FactoryExtensions
	{
		/// <summary>
		/// Gets a map view that translates a map view exposing type <typeparamref name="TBase"/> to a map view that exposes type
		/// <typeparamref name="TProduced"/>, by using the factory's <see cref="Factory{TBlueprint, TProduced}.Create(string)"/>
		/// method and the provided function to translate <typeparamref name="TBase"/> values to factory keys.  It does not cache values,
		/// so if you need to keep these values, you'll need to copy them to another map view.
		/// </summary>
		/// <remarks>
		/// Every time a location is accessed via the resulting map view, the <see cref="Create(string, Coord)"/> function is called.
		/// If you want to access locations multiple times and get the same instance, you'll want to cache these values into another
		/// map view by using a map view's <see cref="IMapView{T}.ApplyOverlay"/> function.
		/// </remarks>
		/// <typeparam name="TBase">Type that the base map view is exposing.</typeparam>
		/// <param name="baseMap">Map view to translate.</param>
		/// <param name="keyTranslator">Function that translates values of type <typeparamref name="TBase"/> to factory keys.</param>
		/// <returns>A map view that, each time locations are accessed, calls the <see cref="Factory{TBlueprint, TProduced}.Create(string)"/>
		/// method with the appropriate key.</returns>
		public static IMapView<TProduced> GetObjectCreationMap<TBlueprint, TProduced, TBase>(this Factory<TBlueprint, TProduced> factory, IMapView<TBase> baseMap, Func<TBase, string> keyTranslator)
			where TBlueprint : IFactoryBlueprint<TProduced>
			where TProduced : GoRogue.GameFramework.IGameObject
		{
			return new LambdaTranslationMap<TBase, TProduced>(baseMap,
			(pos, baseVal) =>
			{
				var val = factory.Create(keyTranslator(baseVal));
				val.Position = pos;

				return val;
			});
		}
	}
}
