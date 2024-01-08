using System.Windows.Input;

namespace Galaga
{
    public class PlayerController
    {
        Spaceship player;
        int rightBorder;
        int leftBorder;
        int bottomBorder;


        public PlayerController(Spaceship spaceship, int leftBorder, int rightBorder, int bottomBorder)
        {
            player = spaceship;
            this.rightBorder = rightBorder;
            this.leftBorder = leftBorder;
            this.bottomBorder = bottomBorder;

        }

        public int UpMoveInput()
        {
            if (!player.ControllingEnabled)
            {
                return 0;
            }

            player.MoveDirection = Direction.None;

            if (Keyboard.IsKeyDown(Key.Up))
            {
                player.MoveDirection += (int)Direction.Up;
            }

            if (Keyboard.IsKeyDown(Key.Down) && !(player.YAfterMove(Direction.Down) > bottomBorder))
            {
                player.MoveDirection += (int)Direction.Down;
            }

            return (int)player.MoveDirection * player.Speed;
        }

        public int HandleMoveInput()
        {
            if (!player.ControllingEnabled)
            {
                return 0;
            }

            player.MoveDirection = Direction.None;

            if (Keyboard.IsKeyDown(Key.Left) && !(player.XAfterMove(Direction.Left) < leftBorder))
            {
                player.MoveDirection += (int)Direction.Left;
            }

            if (Keyboard.IsKeyDown(Key.Right) && !(player.XAfterMove(Direction.Right) > rightBorder))
            {
                player.MoveDirection += (int)Direction.Right;
            }

            return (int)player.MoveDirection * player.Speed;
        }


        public delegate void EventHandler(object sender, NewBulletEventArgs e);
        public event EventHandler PlayerShot;

        public bool HandleShootingInput()
        {
            if (!player.ControllingEnabled)
            {
                return false; ;
            }

            if (player.CanShoot)
            {
                player.CanShoot = false;

                player.shootingCooldownTimer.Stop();
                player.shootingCooldownTimer.Start();

                PlayerShot.Invoke(this, new NewBulletEventArgs(new Bullet(player.X + player.Width / 2,
                                                               player.Y,
                                                               Bullet.Source.Spaceship
                                                               )));
                return true;
            }

            return false;
        }
    }
}
