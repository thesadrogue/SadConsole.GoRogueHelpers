using GoRogue.GameFramework;
using GoRogue.GameFramework.Components;
using System;
using System.Linq;

namespace SadConsole.Components.GoRogue
{
    public class ComponentBase : IGameObjectComponent
    {
        public event EventHandler Added;
        public event EventHandler Removed;

        private IGameObject _parent;
        public virtual IGameObject Parent
        {
            get => _parent;
            set
            {
                if (value == null)
                {
                    _parent = value;
                    Removed?.Invoke(this, EventArgs.Empty);
                }
                else if (_parent != null)
                    throw new Exception($"{nameof(ComponentBase)} components inherit from {nameof(IGameObjectComponent)}, so they can't be attached to multiple objects simultaneously.");

                _parent = value;
                Added?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Add as a handler to <see cref="Added"/> to enforce that this component must be parented to an object that inherits from/implements <typeparamref name="TParent"/>.
        /// </summary>
        /// <typeparam name="TParent">Type of object that must be this component's parent.</typeparam>
        /// <param name="s"/>
        /// <param name="e"/>
        public static void ParentTypeCheck<TParent>(object s, EventArgs e)
        {
            var componentBase = (ComponentBase)s;

            if (componentBase.Parent is TParent)
                return;

            throw new Exception($"{componentBase.GetType().Name} components are marked with a TypeCheck, so can only be attached to something that inherits from/implements {typeof(TParent).Name}.");
        }

        /// <summary>
        /// Add as a handler to <see cref="Added"/> to enforce that this component may not be added to an object that has a component of type <typeparamref name="TComponent"/>.
        /// May also be used to enforce that the component can't have multiple instances of itself attached to the same object by using Added += IncompatibleWith&lt;MyOwnType&gt;;
        /// </summary>
        /// <typeparam name="TComponent">Type of the component this one is incompatible with.</typeparam>
        /// <param name="s"/>
        /// <param name="e"/>
        public void IncompatibleWith<TComponent>(object s, EventArgs e)
        {
            if (Parent.GetComponents<TComponent>().Any(i => !ReferenceEquals(this, i)))
                throw new Exception($"{s.GetType().Name} components are marked as incompatible with {typeof(TComponent).Name} components, so the component couldn't be added.");
        }
    }
}
