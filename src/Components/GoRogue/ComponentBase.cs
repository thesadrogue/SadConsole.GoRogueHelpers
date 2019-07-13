using GoRogue.GameFramework;
using GoRogue.GameFramework.Components;
using System;

namespace SadConsole.Components.GoRogue
{
    public class ComponentBase : IGameObjectComponent
    {
        public event EventHandler OnAdded;
        public event EventHandler OnRemoved;

        private IGameObject _parent;
        public virtual IGameObject Parent
        {
            get => _parent;
            set
            {
                if (value == null)
                {
                    _parent = value;
                    OnRemoved?.Invoke(this, EventArgs.Empty);
                }
                else if (_parent != null)
                    throw new Exception($"{nameof(ComponentBase)} components inherit from {nameof(IGameObjectComponent)}, so they can't be attached to multiple objects simultaneously.");

                _parent = value;
                OnAdded?.Invoke(this, EventArgs.Empty);
            }
        }

        public static void TypeCheck<TParent>(object s, EventArgs e)
        {
            var componentBase = (ComponentBase)s;

            if (componentBase.Parent is TParent)
                return;

            throw new Exception($"{typeof(ComponentBase).Name} components have a TypeCheck OnAdded handler, so can only be attached to something that inherits from/implements {typeof(TParent).Name}.");
        }
    }
}
