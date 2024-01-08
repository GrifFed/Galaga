using System;
using System.Timers;

namespace Galaga
{
    public class Spaceship : GameEntity, IMovingEntity, IShootingEntity, IControllable
    {
        public int Speed { get; set; }
        public Direction MoveDirection { get; set; }

        public Direction DirectionOfProjectile { get; }

        public PlayerController PlayerController { get; }

        public int ShootingCooldownMiliseconds { get; set; }
        public bool CanShoot { get; set; }
        public Timer shootingCooldownTimer { get; set; }
        public bool ControllingEnabled { get; set; }

        private const int INITIAL_SPEED = 5;
        private const int SHOOTING_COOLDOWN_MILISECONDS = 500;

        public Spaceship(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Speed = INITIAL_SPEED;
            DirectionOfProjectile = Direction.Up;

            ShootingCooldownMiliseconds = SHOOTING_COOLDOWN_MILISECONDS;

            shootingCooldownTimer = new Timer()
            {
                Interval = ShootingCooldownMiliseconds
            };
            shootingCooldownTimer.Elapsed += OnShootingCooldownTimerElapsed;
            CanShoot = true;

            PlayerController = new PlayerController(this, 0, MainWindow.WINDOW_WIDTH - Width - MainWindow.WEIRD_WINDOW_RIGHT_BORDER_DISPLACEMENT_AMOUNT + 9, MainWindow.WINDOW_HEIGHT - MainWindow.HEIGHT - MainWindow.WEIRD_WINDOW_DOWN_BORDER_DISPLACEMENT_AMOUNT + 9);
        }

        public void Move()
        {
            Y += PlayerController.UpMoveInput();
            X += PlayerController.HandleMoveInput();
        }

        public int XAfterMove(Direction direction)
        {
            return X + Speed * (int)direction;
        }

        public int YAfterMove(Direction direction)
        {
            return Y + Speed * (int)direction;
        }

        public bool Shoot()
        {
            return PlayerController.HandleShootingInput();
        }

        public void OnShootingCooldownTimerElapsed(object sender, EventArgs e)
        {
            CanShoot = true;
        }
    }
}

