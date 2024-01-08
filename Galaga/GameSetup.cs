using System.Collections.Generic;

namespace Galaga
{
    public class GameSetup
    {
        public const int ALIENS_IN_ROW_COUNT = 10;
        public const int ALIENS_IN_COLUMN_COUNT = 5;

        public const int BOARD_HORIZONTAL_MARGIN = 46;
        public const int BOARD_VERTICAL_MARGIN = 30;

        public const int SPACE_BETWEEN_ROWS = 30;

        public const int SPACESHIP_INITIAL_X = MainWindow.WINDOW_WIDTH / 2 - MainWindow.WEIRD_WINDOW_RIGHT_BORDER_DISPLACEMENT_AMOUNT;
        public const int SPACESHIP_INITIAL_Y = 550;
        public const int SPACESHIP_WIDTH = 39;
        public const int SPACESHIP_HEIGHT = 39;

        public Alien.Type[] AlienTypesInRows =
        [
            Alien.Type.AlienType1,
            Alien.Type.AlienType2,
            Alien.Type.AlienType2,
            Alien.Type.AlienType3,
            Alien.Type.AlienType3
        ];

        public static Dictionary<Alien.Type, int> SpacesBetweenAliens = new Dictionary<Alien.Type, int>()
        {
            { Alien.Type.AlienType1, 24 },
            { Alien.Type.AlienType2, 15 },
            { Alien.Type.AlienType3, 12 }
        };

        public Board Board;

        public GameSetup(Board board)
        {
            Board = board;
        }

        public void Setup()
        {
            Board.Aliens = new List<List<Alien>>(ALIENS_IN_COLUMN_COUNT);

            int leftOffset;
            int topOffset = BOARD_VERTICAL_MARGIN;
            int currSpaceBetweenAliens;

            for (int y = 0; y < ALIENS_IN_COLUMN_COUNT; ++y)
            {
                Board.Aliens.Add(new List<Alien>(ALIENS_IN_ROW_COUNT));

                leftOffset = BOARD_HORIZONTAL_MARGIN;
                topOffset = BOARD_VERTICAL_MARGIN + y * (SPACE_BETWEEN_ROWS + Alien.AlienHeight);
                currSpaceBetweenAliens = SpacesBetweenAliens[AlienTypesInRows[y]];

                switch (AlienTypesInRows[y])
                {
                    case Alien.Type.AlienType2: { leftOffset += 1; break; }
                    case Alien.Type.AlienType1: { leftOffset += 2; break; }
                }

                for (int x = 0; x < ALIENS_IN_ROW_COUNT; ++x)
                {
                    Board.Aliens[y].Add(new Alien(leftOffset, topOffset, AlienTypesInRows[y]));

                    leftOffset += currSpaceBetweenAliens + Alien.AlienTypeWidth[AlienTypesInRows[y]];
                }
            }

            Board.Spaceship = new Spaceship(SPACESHIP_INITIAL_X, SPACESHIP_INITIAL_Y, SPACESHIP_WIDTH, SPACESHIP_HEIGHT);
            Board.Spaceship.ControllingEnabled = true;

            Board.SpaceshipBullets = new List<Bullet>();
            Board.AlienBullets = new List<Bullet>();

            Board.Setup();
        }
    }
}
