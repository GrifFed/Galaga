using System;
using System.Collections.Generic;
using System.Timers;

namespace Galaga
{
    public class Alien : GameEntity, IMovingEntity, IShootingEntity
    {
        public int Speed { get; set; }
        public Direction MoveDirection { get; set; }

        public Direction DirectionOfProjectile { get; }
        public int ShootingCooldownMiliseconds { get; set; }

        public bool CanShoot { get; set; }
        public Timer shootingCooldownTimer { get; set; }

        public const int SHOOTING_COOLDOWN_LOW_BORDER = 5000;
        public const int SHOOTING_COOLDOWN_UP_BORDER = 25000;

        private static Random random = new Random();

        public void Move()
        {
            X += Speed * (int)MoveDirection;
        }

        public int XAfterMove()
        {
            return X + Speed * (int)MoveDirection;
        }

        public enum Type
        {
            AlienType1,
            AlienType2,
            AlienType3
        }

        public static Dictionary<Type, int> AlienTypeWidth = new Dictionary<Type, int>
        {
            { Type.AlienType1, 24 },
            { Type.AlienType2, 33 },
            { Type.AlienType3, 36 }
        };

        public static int AlienHeight = 24;
        public static int InitialSpeed = 20;

        public Type TypeSize;

        public Alien(int x, int y, Type type)
        {
            X = x;
            Y = y;
            TypeSize = type;
            Width = AlienTypeWidth[TypeSize];
            Height = AlienHeight;
            Speed = InitialSpeed;
            MoveDirection = Direction.Right;

            ShootingCooldownMiliseconds = random.Next(SHOOTING_COOLDOWN_LOW_BORDER, SHOOTING_COOLDOWN_UP_BORDER);

            shootingCooldownTimer = new Timer()
            {
                Interval = ShootingCooldownMiliseconds
            };
            shootingCooldownTimer.Elapsed += OnShootingCooldownTimerElapsed;
            shootingCooldownTimer.Start();
            CanShoot = false;
        }

        public void GetCloserToEarth(int amount)
        {
            Y += amount;
        }

        internal delegate void EventHandler(object sender, NewBulletEventArgs e);
        internal event EventHandler AlienShot;

        public bool Shoot()
        {
            if (!CanShoot)
            {
                return false;
            }

            CanShoot = false;

            shootingCooldownTimer.Stop();
            shootingCooldownTimer.Start();

            AlienShot.Invoke(this, new NewBulletEventArgs(new Bullet(X + Width / 2,
                                                          Y + Height,
                                                          Bullet.Source.Alien
                                                          )));
            return true;
        }

        private void OnShootingCooldownTimerElapsed(object sender, EventArgs e)
        {
            CanShoot = true;
        }
    }
}