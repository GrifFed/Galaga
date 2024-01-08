namespace Galaga
{ 
    public class Bullet : GameEntity, IMovingEntity
    {
        public int Speed { get; set; }
        public Direction MoveDirection { get; set; }

        public enum Source
        {
            Spaceship,
            Alien
        }

        public Source BulletSource;

        public const int SPACESHIP_BULLET_WIDTH = 9;
        public const int SPACESHIP_BULLET_HEIGHT = 22;
        public const int SPACESHIP_BULLET_SPEED = 15;
        public const Direction SPACESHIP_BULLET_MOVE_DIRECTION = Direction.Up;

        public const int ALIEN_BULLET_WIDTH = 9;
        public const int ALIEN_BULLET_HEIGHT = 22;
        public const int ALIEN_BULLET_SPEED = 5;
        public const Direction ALIEN_BULLET_MOVE_DIRECTION = Direction.Down;

        public Bullet(int x, int y, Source source)
        {
            X = x;
            Y = y;
            BulletSource = source;

            if (source == Source.Alien)
            {
                Width = ALIEN_BULLET_WIDTH;
                Height = ALIEN_BULLET_HEIGHT;
                Speed = ALIEN_BULLET_SPEED;
                MoveDirection = ALIEN_BULLET_MOVE_DIRECTION;
            }

            if (source == Source.Spaceship)
            {
                Width = SPACESHIP_BULLET_WIDTH;
                Height = SPACESHIP_BULLET_HEIGHT;
                Speed = SPACESHIP_BULLET_SPEED;
                MoveDirection = SPACESHIP_BULLET_MOVE_DIRECTION;
            }
        }

        public void Move()
        {
            Y += Speed * (int)MoveDirection;
        }
    }
}
