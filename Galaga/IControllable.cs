namespace Galaga
{
    public interface IControllable
    {
        public PlayerController PlayerController { get; }
        public bool ControllingEnabled { get; set; }
        public int XAfterMove(Direction direction);
        public int YAfterMove(Direction direction);
    }
}
