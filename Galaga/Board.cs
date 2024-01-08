using System.Collections.Generic;

namespace Galaga
{ 
    public class Board
    {
        public List<List<Alien>> Aliens { get; set; }
        public Spaceship Spaceship { get; set; }
        public List<Bullet> SpaceshipBullets { get; set; }
        public List<Bullet> AlienBullets { get; set; }

        public void Setup()
        {
            Spaceship.PlayerController.PlayerShot += NewBullet;

            foreach (List<Alien> alienList in Aliens)
            {
                foreach (Alien alien in alienList)
                {
                    alien.AlienShot += NewBullet;
                }
            }
        }

        public delegate void EventHandler(object sender, NewBulletEventArgs e);
        public event EventHandler BulletAddedToSpaceInvadersBoard;

        public void NewBullet(object sender, NewBulletEventArgs e)
        {
            if (e.NewBullet.BulletSource == Bullet.Source.Spaceship)
            {
                SpaceshipBullets.Add(e.NewBullet);
            }
            else
            {
                AlienBullets.Add(e.NewBullet);
            }

            BulletAddedToSpaceInvadersBoard.Invoke(this, e);
        }
    }
}
