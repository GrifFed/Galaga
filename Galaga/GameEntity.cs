namespace Galaga
{
    public abstract class GameEntity
    {
        // Position on board
        public int X { get; set; }
        public int Y { get; set; }

        // Size
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
