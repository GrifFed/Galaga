using System;

namespace Galaga
{
    public class NewBulletEventArgs : EventArgs
    {
        public Bullet NewBullet { get; set; }

        public NewBulletEventArgs(Bullet newBullet)
        {
            NewBullet = newBullet;
        }
    }
}
