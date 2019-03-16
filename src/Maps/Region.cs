using GoRogue;
using System.Collections.Generic;

namespace SadConsole.Maps
{
	// TODO: Perhaps representable by GoRogue MapArea (subclass/composition)?
    /// <summary>
    /// Region of a map.
    /// </summary>
    public class Region
    {
        public bool IsRectangle;
        public Rectangle InnerRect;
        public Rectangle OuterRect;
        public List<Coord> InnerPoints = new List<Coord>();
        public List<Coord> OuterPoints = new List<Coord>();
        public bool IsLit = true;
        public bool IsVisited;
        public List<Coord> Connections = new List<Coord>();
    }
}
