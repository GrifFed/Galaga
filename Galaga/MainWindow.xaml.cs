using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Galaga
{
    public partial class MainWindow : Window
    {
        public const int WINDOW_WIDTH = 650;
        public const int WINDOW_HEIGHT = 700;
        public const int HEIGHT = 120;

        public const int ALIEN_ADVANCEMENT_RATE = 10;
        public const int ALIEN_POST_DEATH_EXPLOSION_TIME = 200;
        public const int TIME_BETWEEN_WAVES = 500;
        public const double WAVE_ALIEN_FPS_MULTIPLIER = 1.5;

        public int Score = 0;

        private DispatcherTimer gameTimerAliens;
        private const double INITIAL_ALIENS_UPDATES_PER_SECOND = 0.8;

        private DispatcherTimer gameTimerStandard;
        private const int STANDARD_UPDATES_PER_SECOND = 300;

        private DispatcherTimer alienDeathTimer;

        private int currentWaveIndex = 0;
        private double currentAliensUpdatesPerSecond = INITIAL_ALIENS_UPDATES_PER_SECOND;
        private double aliensUpdatesPerSecondBonus = 0.1;

        private List<Alien> leftmostAliens;
        private List<Alien> rightmostAliens;

        private bool previousTickAliensWentDown;

        public const int WEIRD_WINDOW_RIGHT_BORDER_DISPLACEMENT_AMOUNT = 24;
        public const int WEIRD_WINDOW_DOWN_BORDER_DISPLACEMENT_AMOUNT = 24;
        public const int UPPER_BORDER_Y = 10;
        public const int LOWER_BORDER_Y = 600;

        public const int ALIEN_EXPLOSION_WIDTH = 36;

        public int SpaceshipLivesLeft = 3;

        internal readonly Dictionary<Alien.Type, int> ALIEN_SCORES = new Dictionary<Alien.Type, int>()
        {
            { Alien.Type.AlienType1, 30 },
            { Alien.Type.AlienType2, 20 },
            { Alien.Type.AlienType3, 10 }
        };


        internal GameSetup SpaceInvaders;

        internal static Dictionary<Alien.Type, ImageBrush> AliensImages { get; set; } = new Dictionary<Alien.Type, ImageBrush>()
        {
            { 
                Alien.Type.AlienType3, new ImageBrush(){ImageSource = new BitmapImage(new Uri("../../../res/img/AlienType3.png", UriKind.Relative))} 
            },
            {
                Alien.Type.AlienType2, new ImageBrush(){ImageSource = new BitmapImage(new Uri("../../../res/img/AlienType2.png", UriKind.Relative))}
            },
            {
                Alien.Type.AlienType1, new ImageBrush() { ImageSource = new BitmapImage(new Uri("../../../res/img/AlienType1.png", UriKind.Relative))}
            },
        };
        internal static ImageBrush SpaceshipImage = new ImageBrush() { ImageSource = new BitmapImage(new Uri("../../../res/img/Spaceship.png", UriKind.Relative)) };
        internal static ImageBrush SpaceshipBulletImage = new ImageBrush() { ImageSource = new BitmapImage(new Uri("../../../res/img/SpaceshipBullet.png", UriKind.Relative)) };
        internal static ImageBrush AlienBulletImage = new ImageBrush() { ImageSource = new BitmapImage(new Uri("../../../res/img/AlienBullet.png", UriKind.Relative)) };
        internal static ImageBrush AlienExplosionImage = new ImageBrush() { ImageSource = new BitmapImage(new Uri("../../../res/img/AlienExplosion.png", UriKind.Relative)) };

        public List<List<Rectangle>> AliensRectangles = new List<List<Rectangle>>(GameSetup.ALIENS_IN_COLUMN_COUNT);
        public Rectangle SpaceshipRectangle;
        public List<Rectangle> SpaceshipBulletsRectangles = new List<Rectangle>();
        public List<Rectangle> AliensBulletsRectangles = new List<Rectangle>();

        public MainWindow()
        {
            InitializeComponent();

            Height = WINDOW_HEIGHT;
            Width = WINDOW_WIDTH;

            SpaceInvaders = new GameSetup(new Board());

            SpaceInvaders.Board.BulletAddedToSpaceInvadersBoard += AddBulletToBoard;

            Setup();
        }

        public void Setup()
        {
            SpaceInvaders.Setup();

            ClearBoard();
            WaveLabel.Content = "Wave: " + (currentWaveIndex + 1);

            AliensRectangles = new List<List<Rectangle>>(GameSetup.ALIENS_IN_COLUMN_COUNT);
            AliensBulletsRectangles = new List<Rectangle>();
            SpaceshipBulletsRectangles = new List<Rectangle>();

            for (int y = 0; y < GameSetup.ALIENS_IN_COLUMN_COUNT; ++y)
            {
                AliensRectangles.Add(new List<Rectangle>(GameSetup.ALIENS_IN_ROW_COUNT));

                for (int x = 0; x < GameSetup.ALIENS_IN_ROW_COUNT; ++x)
                {
                    AliensRectangles[y].Add(new Rectangle
                    {
                        Height = SpaceInvaders.Board.Aliens[y][x].Height,
                        Width = SpaceInvaders.Board.Aliens[y][x].Width,
                        Fill = AliensImages[SpaceInvaders.Board.Aliens[y][x].TypeSize]
                    });

                    Canvas.SetTop(AliensRectangles[y][x], SpaceInvaders.Board.Aliens[y][x].Y);
                    Canvas.SetLeft(AliensRectangles[y][x], SpaceInvaders.Board.Aliens[y][x].X);

                    BoardPanel.Children.Add(AliensRectangles[y][x]);
                }
            }

            GetAliensWithRightmostPositions();
            GetAliensWithLeftmostPositions();

            SpaceshipRectangle = new Rectangle
            {
                Height = SpaceInvaders.Board.Spaceship.Height,
                Width = SpaceInvaders.Board.Spaceship.Width,
                Fill = SpaceshipImage
            };

            Canvas.SetTop(SpaceshipRectangle, SpaceInvaders.Board.Spaceship.Y);
            Canvas.SetLeft(SpaceshipRectangle, SpaceInvaders.Board.Spaceship.X);

            BoardPanel.Children.Add(SpaceshipRectangle);

            previousTickAliensWentDown = false;

            Canvas.SetLeft(GameOverLabel, 120);
            Canvas.SetTop(GameOverLabel, 120);

            currentAliensUpdatesPerSecond = INITIAL_ALIENS_UPDATES_PER_SECOND + (currentWaveIndex * WAVE_ALIEN_FPS_MULTIPLIER);

            gameTimerAliens = new DispatcherTimer();
            gameTimerAliens.Interval = TimeSpan.FromMilliseconds(1000.0 / currentAliensUpdatesPerSecond);
            gameTimerAliens.Tick += GameTimerAliensTick;

            gameTimerStandard = new DispatcherTimer();
            gameTimerStandard.Interval = TimeSpan.FromMilliseconds(1000.0 / STANDARD_UPDATES_PER_SECOND);
            gameTimerStandard.Tick += GameTimerStandardTick;

            gameTimerAliens.Start();
            gameTimerStandard.Start();
        }

        private void GameTimerStandardTick(object sender, EventArgs e)
        {
            Spaceship spaceship = SpaceInvaders.Board.Spaceship;

            spaceship.Move();

            Canvas.SetTop(SpaceshipRectangle, spaceship.Y);
            Canvas.SetLeft(SpaceshipRectangle, spaceship.X);

            spaceship.Shoot();

            for (int i = 0; i < SpaceInvaders.Board.SpaceshipBullets.Count; ++i)
            {
                Bullet currentBullet = SpaceInvaders.Board.SpaceshipBullets[i];
                currentBullet.Move();

                if (currentBullet.Y <= UPPER_BORDER_Y)
                {
                    RemoveSpaceshipBullet(i);

                    continue;
                }

                Canvas.SetTop(SpaceshipBulletsRectangles[i], currentBullet.Y);
                Canvas.SetLeft(SpaceshipBulletsRectangles[i], currentBullet.X);

                Rect bulletHitBox = new Rect(Canvas.GetLeft(SpaceshipBulletsRectangles[i]), Canvas.GetTop(SpaceshipBulletsRectangles[i]), SpaceshipBulletsRectangles[i].Width, SpaceshipBulletsRectangles[i].Height);
                bool bulletHit = false;

                for (int y = 0; y < SpaceInvaders.Board.Aliens.Count && !bulletHit; ++y)
                {
                    for (int x = 0; x < SpaceInvaders.Board.Aliens[y].Count; ++x)
                    {
                        Rectangle currAlienRectangle = AliensRectangles[y][x];

                        Rect alienHitBox = new Rect(Canvas.GetLeft(currAlienRectangle), Canvas.GetTop(currAlienRectangle), currAlienRectangle.Width, currAlienRectangle.Height);

                        if (alienHitBox.IntersectsWith(bulletHitBox))
                        {
                            bulletHit = true;
                            RemoveSpaceshipBullet(i);
                            AlienDeath(y, x);

                            break;
                        }
                    }
                }
            }

            for (int y = 0; y < SpaceInvaders.Board.Aliens.Count; ++y)
            {
                for (int x = 0; x < SpaceInvaders.Board.Aliens[y].Count; ++x)
                {
                    Alien currentAlien = SpaceInvaders.Board.Aliens[y][x];

                    currentAlien.Shoot();
                }
            }

            Rect spaceshipHitBox = new Rect(Canvas.GetLeft(SpaceshipRectangle), Canvas.GetTop(SpaceshipRectangle), SpaceshipRectangle.Width, SpaceshipRectangle.Height);

            for (int i = 0; i < SpaceInvaders.Board.AlienBullets.Count; ++i)
            {
                Bullet currentBullet = SpaceInvaders.Board.AlienBullets[i];
                currentBullet.Move();

                // Bullet hit the ground
                if (currentBullet.Y >= LOWER_BORDER_Y)
                {
                    RemoveAlienBullet(i);

                    continue;
                }

                Canvas.SetTop(AliensBulletsRectangles[i], currentBullet.Y);
                Canvas.SetLeft(AliensBulletsRectangles[i], currentBullet.X);

                // Collisions with spaceship
                Rect bulletHitBox = new Rect(Canvas.GetLeft(AliensBulletsRectangles[i]), Canvas.GetTop(AliensBulletsRectangles[i]), AliensBulletsRectangles[i].Width, AliensBulletsRectangles[i].Height);

                if (bulletHitBox.IntersectsWith(spaceshipHitBox))
                {
                    RemoveAlienBullet(i);
                    SpaceshipHit();
                }
            }
        }

        private void SpaceshipHit()
        {
            --SpaceshipLivesLeft;
            LivesLabel.Content = "Lives: " + SpaceshipLivesLeft;

            if (SpaceshipLivesLeft <= 0)
            {
                GameOver();
            }
        }

        private void AlienDeath(int y, int x)
        {
            currentAliensUpdatesPerSecond += aliensUpdatesPerSecondBonus;

            Rectangle deadAlienRectangle = AliensRectangles[y][x];
            Alien deadAlien = SpaceInvaders.Board.Aliens[y][x];

            Score += ALIEN_SCORES[deadAlien.TypeSize];
            ScoreLabel.Content = "Score: " + Score;

            SpaceInvaders.Board.Aliens[y].RemoveAt(x);
            AliensRectangles[y].RemoveAt(x);

            if (SpaceInvaders.Board.Aliens[y].Count == 0)
            {
                SpaceInvaders.Board.Aliens.RemoveAt(y);
                AliensRectangles.RemoveAt(y);
            }

            RemoveAlienFromListsWithUtmostPositions(deadAlien);

            deadAlienRectangle.Width = ALIEN_EXPLOSION_WIDTH;
            deadAlienRectangle.Fill = AlienExplosionImage;
            alienDeathTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(ALIEN_POST_DEATH_EXPLOSION_TIME)
            };
            alienDeathTimer.Tick += (sender, e) => { RemoveAlienRectangle(sender, e, deadAlienRectangle); };
            alienDeathTimer.Start();

            if (SpaceInvaders.Board.Aliens.Count == 0)
            {
                gameTimerAliens.Stop();
                gameTimerStandard.Stop();

                ++currentWaveIndex;

                DispatcherTimer waveEndedTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(TIME_BETWEEN_WAVES)
                };
                waveEndedTimer.Tick += (sender, e) => { (sender as DispatcherTimer).Stop(); Setup(); };
                waveEndedTimer.Start();
            }
        }

        private void RemoveAlienRectangle(object sender, EventArgs e, Rectangle deadAlienRectangle)
        {
            BoardPanel.Children.Remove(deadAlienRectangle);
            (sender as DispatcherTimer).Stop();
        }

        private void RemoveSpaceshipBullet(int index)
        {
            SpaceInvaders.Board.SpaceshipBullets.RemoveAt(index);
            BoardPanel.Children.Remove(SpaceshipBulletsRectangles[index]);
            SpaceshipBulletsRectangles.RemoveAt(index);
        }

        private void RemoveAlienBullet(int index)
        {
            SpaceInvaders.Board.AlienBullets.RemoveAt(index);
            BoardPanel.Children.Remove(AliensBulletsRectangles[index]);
            AliensBulletsRectangles.RemoveAt(index);
        }

        private void GameTimerAliensTick(object sender, EventArgs e)
        {

            gameTimerAliens.Interval = TimeSpan.FromMilliseconds(1000.0 / currentAliensUpdatesPerSecond);

            bool thisTickAliensGoDown =
                !previousTickAliensWentDown &&
                (rightmostAliens[0].XAfterMove() + rightmostAliens[0].Width + WEIRD_WINDOW_RIGHT_BORDER_DISPLACEMENT_AMOUNT > ActualWidth ||
                leftmostAliens[0].XAfterMove() < 0);

            Alien currentAlien;

            for (int y = 0; y < SpaceInvaders.Board.Aliens.Count; ++y)
            {
                for (int x = 0; x < SpaceInvaders.Board.Aliens[y].Count; ++x)
                {
                    currentAlien = SpaceInvaders.Board.Aliens[y][x];

                    if (thisTickAliensGoDown)
                    {
                        currentAlien.MoveDirection = (Direction)((int)currentAlien.MoveDirection * -1);
                        currentAlien.GetCloserToEarth(ALIEN_ADVANCEMENT_RATE);
                    }
                    else
                    {
                        currentAlien.Move();
                    }

                    Canvas.SetTop(AliensRectangles[y][x], currentAlien.Y);
                    Canvas.SetLeft(AliensRectangles[y][x], currentAlien.X);
                }
            }

            previousTickAliensWentDown = thisTickAliensGoDown;

            Alien alienFromBottommostRow = SpaceInvaders.Board.Aliens[SpaceInvaders.Board.Aliens.Count - 1][0];

            if (alienFromBottommostRow.Y + alienFromBottommostRow.Height >= SpaceInvaders.Board.Spaceship.Y)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            gameTimerStandard.Stop();
            gameTimerAliens.Stop();

            ClearBoard();

            SpaceInvaders.Board.Spaceship.ControllingEnabled = false;
            GameOverLabel.Content = "GAME OVER. SCORE: " + Score;
            GameOverLabel.Visibility = Visibility.Visible;
        }

        private void GetAliensWithRightmostPositions()
        {
            rightmostAliens = SpaceInvaders.Board.Aliens.SelectMany(a => a).Where(a => a.X == SpaceInvaders.Board.Aliens.SelectMany(a => a).MaxBy(a => a.X).X).ToList();
        }

        private void GetAliensWithLeftmostPositions()
        {
            leftmostAliens = SpaceInvaders.Board.Aliens.SelectMany(a => a).Where(a => a.X == SpaceInvaders.Board.Aliens.SelectMany(a => a).MinBy(a => a.X).X).ToList();
        }

        private void RemoveAlienFromListsWithUtmostPositions(Alien alien)
        {
            if (rightmostAliens.Remove(alien) && rightmostAliens.Count == 0)
            {
                GetAliensWithRightmostPositions();
            }
            if (leftmostAliens.Remove(alien) && leftmostAliens.Count == 0)
            {
                GetAliensWithLeftmostPositions();
            }
        }

        private void AddBulletToBoard(object sender, NewBulletEventArgs e)
        {
            Rectangle NewBulletRectangle = new Rectangle
            {
                Width = e.NewBullet.Width,
                Height = e.NewBullet.Height,
                Fill = e.NewBullet.BulletSource == Bullet.Source.Spaceship ? SpaceshipBulletImage : AlienBulletImage
            };

            if (e.NewBullet.BulletSource == Bullet.Source.Spaceship)
            {
                SpaceshipBulletsRectangles.Add(NewBulletRectangle);
            }
            else
            {
                AliensBulletsRectangles.Add(NewBulletRectangle);
            }

            Canvas.SetTop(NewBulletRectangle, e.NewBullet.Y);
            Canvas.SetLeft(NewBulletRectangle, e.NewBullet.X);

            BoardPanel.Children.Add(NewBulletRectangle);
        }

        private void ClearBoard()
        {
            foreach (Rectangle bulletRectangle in SpaceshipBulletsRectangles)
            {
                BoardPanel.Children.Remove(bulletRectangle);
            }

            foreach (Rectangle bulletRectangle in AliensBulletsRectangles)
            {
                BoardPanel.Children.Remove(bulletRectangle);
            }

            foreach (Rectangle alienRectangle in AliensRectangles.SelectMany(a => a).ToList())
            {
                BoardPanel.Children.Remove(alienRectangle);
            }

            BoardPanel.Children.Remove(SpaceshipRectangle);
        }
    }
}
