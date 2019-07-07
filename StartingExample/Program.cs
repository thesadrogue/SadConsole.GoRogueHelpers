using GoRogue;
using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using SadConsole;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace StartingExample
{
    class Program
    {

        public const int Width = 80;
        public const int Height = 25;

        public static ExampleMap Map { get; private set; }
        public static ScrollingConsole MapRenderer { get; set; }
        public static Player Player { get; private set; }

        static void Main(string[] args)
        {
            // Setup the engine and create the main window.
            SadConsole.Game.Create(Width, Height);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Start the game.
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Init()
        {
            // Same size as screen, but we set up to center the camera on the player so expanding beyond this should work fine with no other changes.
            Map = new ExampleMap(Width, Height);

            // Generate map via GoRogue, and update the real map with appropriate terrain.
            var tempMap = new ArrayMap<bool>(Map.Width, Map.Height);
            QuickGenerators.GenerateDungeonMazeMap(tempMap, minRooms: 10, maxRooms: 20, roomMinSize: 5, roomMaxSize: 11);
            Map.ApplyTerrainOverlay(tempMap, SpawnTerrain);

            Coord posToSpawn;
            // Spawn a few mock enemies
            for (int i = 0; i < 10; i++)
            {
                posToSpawn = Map.WalkabilityView.RandomPosition(true); // Get a location that is walkable
                var goblin = new BasicEntity(Color.Red, Color.Transparent, 'g', posToSpawn, (int)MapLayer.MONSTERS, isWalkable: false, isTransparent: true);
                Map.AddEntity(goblin);
            }

            // Spawn player
            posToSpawn = Map.WalkabilityView.RandomPosition(true);
            Player = new Player(posToSpawn);
            Map.AddEntity(Player);

            // Get console that renders map and display it
            MapRenderer = Map.CreateRenderer(new XnaRect(0, 0, Width, Height), SadConsole.Global.FontDefault);
            SadConsole.Global.CurrentScreen = MapRenderer;
            Player.IsFocused = true; // Set player to receive input, since in this example the player handles movement

            // Set up to sync FOV and camera on player move
            Player.Moved += Player_Moved;

            // Calculate initial FOV and center camera
            Map.CalculateFOV(Player.Position, Player.FOVRadius, Radius.SQUARE);
            MapRenderer.CenterViewPortOnPoint(Player.Position);
        }

        private static void Player_Moved(object sender, ItemMovedEventArgs<IGameObject> e)
        {
            Map.CalculateFOV(Player.Position, Player.FOVRadius, Radius.SQUARE);
            MapRenderer.CenterViewPortOnPoint(Player.Position);
        }

        private static IGameObject SpawnTerrain(Coord position, bool mapGenValue)
        {
            // Floor or wall.  This could use the Factory system, or instantiate Floor and wall classes, this simplistic if-else is just used for example
            if (mapGenValue) // Floor
                return new BasicTerrain(Color.White, Color.Black, '.', position, isWalkable: true, isTransparent: true);
            else             // Wall
                return new BasicTerrain(Color.White, Color.Black, '#', position, isWalkable: false, isTransparent: false);
        }
    }
}