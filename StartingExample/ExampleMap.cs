using GoRogue;
using Microsoft.Xna.Framework;
using SadConsole;
using System;

namespace StartingExample
{
    enum MapLayer
    {
        TERRAIN,
        ITEMS,
        MONSTERS,
        PLAYER
    }

    class ExampleMap : BasicMap
    {
        // Handles the changing of tile/entity visiblity as appropriate based on Map.FOV.
        private FOVVisibilityHandler _fovVisibilityHandler;

        public new Player ControlledGameObject
        {
            get => (Player)base.ControlledGameObject;
            set => base.ControlledGameObject = value;
        }

        public ExampleMap(int width, int height)
            // Allow multiple items on the same location only on the items layer.  This example uses 8-way movement, so Chebyshev distance is selected.
            : base(width, height, Enum.GetNames(typeof(MapLayer)).Length - 1, Distance.CHEBYSHEV, entityLayersSupportingMultipleItems: LayerMasker.DEFAULT.Mask((int)MapLayer.ITEMS))
        {
            _fovVisibilityHandler = new DefaultFOVVisibilityHandler(this, ColorAnsi.BlackBright);
        }
    }
}
