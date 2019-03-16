using GoRogue;
using GoRogue.GameFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SadConsole.Tiles
{
	public partial class Tile : BasicTerrain
	{
		protected int tileState;
		protected int tileType;
		protected int flags;

		protected Cell AppearanceNormal;
		protected Cell AppearanceDim;

		public static Cell AppearanceNeverSeen = new Cell(Color.Black, Color.Black, '.');
		public static float DimAmount = 0.4f;

		public event EventHandler TileChanged;

		public new TileMap CurrentMap => (TileMap)(base.CurrentMap);

		public string Title { get; set; }
		public string Description { get; set; }

		public Action<Tile, int> OnTileStateChanged { get; set; }
		public Action<Tile, int> OnTileFlagsChanged { get; set; }
		public Action<Tile, Actions.ActionBase> OnProcessAction { get; set; }


		public string DefinitionId { get; protected set; }

		/// <summary>
		/// The type of tile represented.
		/// </summary>
		public int Type => tileType;

		/// <summary>
		/// Flags for the tile such as blocks LOS.
		/// </summary>
		public int Flags
		{
			get => flags;
			set
			{
				if (flags == value) return;

				var oldFlags = flags;
				flags = value;
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
			get => tileState;
			set
			{
				if (tileState == value) return;

				var oldState = tileState;
				tileState = value;
				OnTileStateChanged?.Invoke(this, oldState);
				UpdateAppearance();
				TileChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		// OK to do tile flag checks in here because base class sets the backing field, not the properties directly,
		// so no undefined behavior.
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
						SetFlag(TileFlags.BlockLOS);
				}
			}
		}

		#region Constructors
		public Tile(Coord position, bool isWalkable, bool isTransparent)
			: base(position, isWalkable, isTransparent)
		{
			Initialize(isWalkable, isTransparent);
		}

		public Tile(Color foreground, Coord position, bool isWalkable, bool isTransparent)
			: base(foreground, position, isWalkable, isTransparent)
		{
			Initialize(isWalkable, isTransparent);
		}

		public Tile(Color foreground, Color background, Coord position, bool isWalkable, bool isTransparent)
			: base(foreground, background, position, isWalkable, isTransparent)
		{
			Initialize(isWalkable, isTransparent);
		}

		public Tile(Color foreground, Color background, int glyph, Coord position, bool isWalkable, bool isTransparent)
			: base(foreground, background, glyph, position, isWalkable, isTransparent)
		{
			Initialize(isWalkable, isTransparent);
		}

		public Tile(Color foreground, Color background, int glyph, SpriteEffects mirror, Coord position, bool isWalkable, bool isTransparent)
			: base(foreground, background, glyph, mirror, position, isWalkable, isTransparent)
		{
			Initialize(isWalkable, isTransparent);
		}

		private void Initialize(bool isWalkable, bool isTransparent)
		{
			if (!isWalkable)
				SetFlag(TileFlags.BlockMove);

			if (!isTransparent)
				SetFlag(TileFlags.BlockLOS);

			TileChanged += OnSelfChanged;
		}

		#endregion Constructors


		public void ChangeAppearance(Cell normal)
		{
			Color dimFore = normal.Foreground * DimAmount;
			Color dimBack = normal.Background * DimAmount;
			dimFore.A = 255;
			dimBack.A = 255;

			ChangeAppearance(normal, new Cell(dimFore, dimBack, normal.Glyph));
		}

		public void ChangeAppearance(Cell normal, Cell dim)
		{
			AppearanceNormal = normal;
			AppearanceDim = dim;

			UpdateAppearance();
		}

		public void ChangeGlyph(int glyph)
		{
			AppearanceNormal.Glyph = glyph;
			AppearanceDim.Glyph = glyph;

			UpdateAppearance();
		}

		/// <summary>
		/// Adds the specified flags to the <see cref="flags"/> property.
		/// </summary>
		/// <param name="flags">The flags to set.</param>
		public void SetFlag(params TileFlags[] flags)
		{
			var total = 0;

			foreach (var flag in flags)
				total = total | (int)flag;

			Flags = Helpers.SetFlag(this.flags, total);
		}
		/// <summary>
		/// Removes the specified flags to the <see cref="flags"/> property.
		/// </summary>
		/// <param name="flags">The flags to remove.</param>
		public void UnsetFlag(params TileFlags[] flags)
		{
			var total = 0;

			foreach (var flag in flags)
				total = total | (int)flag;

			Flags = Helpers.UnsetFlag(this.flags, total);
		}

		public virtual void ProcessAction(Actions.ActionBase action) => OnProcessAction?.Invoke(this, action);

		protected virtual void UpdateAppearance()
		{
			if (!Helpers.HasFlag(in flags, (int)TileFlags.Seen))
			{
				AppearanceNeverSeen.CopyAppearanceTo(this);
			}
			else if ((Helpers.HasFlag(in flags, (int)TileFlags.InLOS) || Helpers.HasFlag(in flags, (int)TileFlags.PermaInLOS))
					 && Helpers.HasFlag(in flags, (int)TileFlags.Lighted) || Helpers.HasFlag(in flags, (int)TileFlags.PermaLight) || Helpers.HasFlag(in flags, (int)TileFlags.RegionLighted))
			{
				AppearanceNormal.CopyAppearanceTo(this);
			}
			else // Seen but not lighted/los
			{
				AppearanceDim.CopyAppearanceTo(this);
			}

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
