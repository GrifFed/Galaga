namespace Galaga
{
    public interface IMovingEntity
    {
        public int Speed { get; set; }

        public Direction MoveDirection { get; set; }

        public void Move();
    }
}
