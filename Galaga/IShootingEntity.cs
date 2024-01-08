using System.Timers;

namespace Galaga
{
    public interface IShootingEntity
    {
        public Direction DirectionOfProjectile { get; }

        public int ShootingCooldownMiliseconds { get; set; }
        public bool CanShoot { get; set; }
        public Timer shootingCooldownTimer { get; set; }

        public bool Shoot();
    }
}
