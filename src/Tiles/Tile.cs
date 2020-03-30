using System;
using GoRogue;
using GoRogue.GameFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Tiles
{
    /// <summary>
    /// A terrain object that implements a flags-based system for recording Tile information.
    /// </summary>
    public partial class Tile : BasicTerrain
    {
        private int _tileState;
        private int _flags;

        private Cell _appearanceNormal;
        private Cell _appearanceDim;

        /// <summary>
        /// Appearance set as the never-seen appearance for cells.  Defaults to black.
        /// </summary>
        public static Cell AppearanceNeverSeen = new Cell(Color.Black, Color.Black, '.');

        /// <summary>
        /// The amount multiplied to the color value of the normal appearance to obtain the unseen appearance, if no unseen appearance is specified.
        /// </summary>
        public static float DimAmount = 0.4f;

        /// <summary>
        /// Fires whenever tile flags, state, etc change.
        /// </summary>
        public event EventHandler TileChanged;

        /// <summary>
        /// The map containining the Tile.
        /// </summary>
        public new TileMap CurrentMap => (TileMap)(base.CurrentMap);

        /// <summary>
        /// Arbitrary title for the Tile.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Arbitrary description for the Tile.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Called automatically when the Tile's <see cref="TileState"/> changes.
        /// </summary>
        public Action<Tile, int> OnTileStateChanged { get; set; }

        /// <summary>
        /// Called automatically when the Tile's <see cref="Flags"/> are changed.
        /// </summary>
        public Action<Tile, int> OnTileFlagsChanged { get; set; }

        /// <summary>
        /// Called automatically when the <see cref="ProcessAction(Actions.ActionBase)"/> function is invoked.
        /// </summary>
        public Action<Tile, Actions.ActionBase> OnProcessAction { get; set; }

        /// <summary>
        /// ID of the blueprint that created this Tile.
        /// </summary>
        public string DefinitionId { get; set; }

        /// <summary>
        /// The type of tile represented.
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// Flags for the tile such as blocks LOS.
        /// </summary>
        public int Flags
        {
            get => _flags;
            set
            {
                if (_flags == value) return;

                int oldFlags = _flags;
                _flags = value;
                OnTileFlagsChanged?.Invoke(this, oldFlags);
                UpdateAppearance();
                TileChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The state of the tile.
        /// </summary>
        public int TileState
        {
            get => _tileState;
            set
            {
                if (_tileState == value) return;

                int oldState = _tileState;
                _tileState = value;
                OnTileStateChanged?.Invoke(this, oldState);
                UpdateAppearance();
                TileChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // OK to do tile flag checks in here because base class sets the backing field, not the properties directly,
        // so no undefined behavior.
        /// <summary>
        /// Whether or not the tile is walkable for the sake of pathing.
        /// </summary>
        public override bool IsWalkable
        {
            get => base.IsWalkable;
            set
            {
                base.IsWalkable = value;
                if (base.IsWalkable != (!Helpers.HasFlag(Flags, (int)TileFlags.BlockMove))) // Sync flag to gorogue setting
                {
                    if (base.IsWalkable)
                        UnsetFlag(TileFlags.BlockMove);
                    else
                        SetFlag(TileFlags.BlockMove);
                }
            }
        }

        /// <summary>
        /// Whether or not the Tile is considered transparent for the sake of FOV.
        /// </summary>
        public override bool IsTransparent
        {
            get => base.IsTransparent;
            set
            {
                base.IsTransparent = value;
                if (base.IsTransparent != (!Helpers.HasFlag(Flags, (int)TileFlags.BlockLOS))) // Sync flag to gorogue setting
                {
                    if (base.IsTransparent)
                        UnsetFlag(TileFlags.BlockLOS);
                    else
                    {
                        SetFlag(TileFlags.BlockLOS);
                    }
                }
            }
        }

        #region Constructors
        public Tile(Coord position, bool isWalkable, bool isTransparent)
            : base(position, isWalkable, isTransparent) => Initialize(isWalkable, isTransparent);

        public Tile(Color foreground, Coord position, bool isWalkable, bool isTransparent)
            : base(foreground, position, isWalkable, isTransparent) => Initialize(isWalkable, isTransparent);

        public Tile(Color foreground, Color background, Coord position, bool isWalkable, bool isTransparent)
            : base(foreground, background, position, isWalkable, isTransparent) => Initialize(isWalkable, isTransparent);

        public Tile(Color foreground, Color background, int glyph, Coord position, bool isWalkable, bool isTransparent)
            : base(foreground, background, glyph, position, isWalkable, isTransparent) => Initialize(isWalkable, isTransparent);

        public Tile(Color foreground, Color background, int glyph, SpriteEffects mirror, Coord position, bool isWalkable, bool isTransparent)
            : base(foreground, background, glyph, mirror, position, isWalkable, isTransparent) => Initialize(isWalkable, isTransparent);

        private void Initialize(bool isWalkable, bool isTransparent)
        {
            if (!isWalkable)
            {
                SetFlag(TileFlags.BlockMove);
            }

            if (!isTransparent)
            {
                SetFlag(TileFlags.BlockLOS);
            }

            Color dimFore = Foreground * DimAmount;
            Color dimBack = Background * DimAmount;
            dimFore.A = 255;
            dimBack.A = 255;

            _appearanceDim = new Cell(dimFore, dimBack, Glyph);
            _appearanceNormal = new Cell(Foreground, Background, Glyph);

            AppearanceNeverSeen.CopyAppearanceTo(this);

            TileChanged += OnSelfChanged;
        }

        #endregion Constructors

        /// <summary>
        /// Modify the appearance of the cell as specified.  The dim version of the cell will be identical, but with the colors dimmed by a factor of <see cref="DimAmount"/>.
        /// </summary>
        /// <param name="normal">Appearance to set.</param>
        public void ChangeAppearance(Cell normal)
        {
            Color dimFore = normal.Foreground * DimAmount;
            Color dimBack = normal.Background * DimAmount;
            dimFore.A = 255;
            dimBack.A = 255;

            ChangeAppearance(normal, new Cell(dimFore, dimBack, normal.Glyph));
        }

        /// <summary>
        /// Modify normal and dim appearances as specified.
        /// </summary>
        /// <param name="normal">Appearance to set as normal appearance.</param>
        /// <param name="dim">Appearance to set as dim appearance.</param>
        public void ChangeAppearance(Cell normal, Cell dim)
        {
            _appearanceNormal = normal;
            _appearanceDim = dim;

            UpdateAppearance();
        }

        /// <summary>
        /// Change glyph of both the normal and dim versions of the Tile to the specified one.
        /// </summary>
        /// <param name="glyph">Glyph to change to.</param>
        public void ChangeGlyph(int glyph)
        {
            _appearanceNormal.Glyph = glyph;
            _appearanceDim.Glyph = glyph;

            UpdateAppearance();
        }

        /// <summary>
        /// Adds the specified flags to the <see cref="Flags"/> property.
        /// </summary>
        /// <param name="flags">The flags to set.</param>
        public void SetFlag(params TileFlags[] flags)
        {
            int total = 0;

            foreach (TileFlags flag in flags)
                total |= (int)flag;

            Flags = Helpers.SetFlag(_flags, total);
        }
        /// <summary>
        /// Removes the specified flags to the <see cref="Flags"/> property.
        /// </summary>
        /// <param name="flags">The flags to remove.</param>
        public void UnsetFlag(params TileFlags[] flags)
        {
            int total = 0;

            foreach (TileFlags flag in flags)
                total |= (int)flag;

            Flags = Helpers.UnsetFlag(_flags, total);
        }

        /// <summary>
        /// Callto cause the Tile to process an action.
        /// </summary>
        /// <param name="action">Action to process.</param>
        public virtual void ProcessAction(Actions.ActionBase action) => OnProcessAction?.Invoke(this, action);

        /// <summary>
        /// Updates the cells appearance based on lighting/explored/seen flags.
        /// </summary>
        protected virtual void UpdateAppearance()
        {
            if (!Helpers.HasFlag(in _flags, (int)TileFlags.Seen))
                AppearanceNeverSeen.CopyAppearanceTo(this);
            else if ((Helpers.HasFlag(in _flags, (int)TileFlags.InLOS) || Helpers.HasFlag(in _flags, (int)TileFlags.PermaInLOS))
                     && Helpers.HasFlag(in _flags, (int)TileFlags.Lighted) || Helpers.HasFlag(in _flags, (int)TileFlags.PermaLight) || Helpers.HasFlag(in _flags, (int)TileFlags.RegionLighted))
                _appearanceNormal.CopyAppearanceTo(this);
            else // Seen but not lighted/los
                _appearanceDim.CopyAppearanceTo(this);

            TileChanged?.Invoke(this, EventArgs.Empty);
        }

        // Handler for OnTileChanged to sync GoRogue IGameObject properties to flags, in the cases where the
        // change originated from a Tile.SetFlag call
        private void OnSelfChanged(object s, EventArgs e)
        {
            IGameObject thisAsGameObject = this;
            // Interesting that these masking functions exist, GoRogue has similar functionality but calls it LayerMasker, is it was designed as a bitmask
            // to deal with map layers specifically.  Might be able to generalize gorogue's...
            thisAsGameObject.IsWalkable = !Helpers.HasFlag(Flags, (int)TileFlags.BlockMove);
            thisAsGameObject.IsTransparent = !Helpers.HasFlag(Flags, (int)TileFlags.BlockLOS);
        }
    }
}
