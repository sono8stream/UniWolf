using System;

namespace Models
{
    class GameBasicInfo
    {

        public string GameName { get; private set; }
        public int TilePixels { get; private set; }
        public int VerticalTileCount { get; private set; }
        public int CharaImageDirections { get; private set; }
        public int CharaAnimations { get; private set; }
        public int CharaMoveDirections { get; private set; }
        public bool ShowShadow { get; private set; }
        public int DeltaPixelsPerMove { get; private set; }
        public int CollisionPixels { get; private set; }

        static GameBasicInfo instance;
        public static GameBasicInfo Instance
        {
            get
            {
                if (instance == null)
                {
                    InitializeInstance();
                }
                return instance;
            }
        }

        /// <summary>
        /// なんらかのロード処理に取って代えられる予定
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="tileSize"></param>
        /// <param name="verticalGridCount"></param>
        /// <param name="charaImageDirections"></param>
        /// <param name="charaAnimations"></param>
        /// <param name="charaMoveDirections"></param>
        /// <param name="showShadow"></param>
        /// <param name="deltaPixelsPerMove"></param>
        /// <param name="collisionPixels"></param>
        public static void InitializeInstance(string gameName = "Sample Game", int tilePixels = 16,
            int verticalTileCount = 15, int charaImageDirections = 8, int charaAnimations = 3,
            int charaMoveDirections = 8, bool showShadow = true, int deltaPixelsPerMove = 8,
            int collisionPixels = 16)
        {
            instance = new GameBasicInfo()
            {
                GameName = gameName,
                TilePixels = tilePixels,
                VerticalTileCount = verticalTileCount,
                CharaImageDirections = charaImageDirections,
                CharaAnimations = charaAnimations,
                CharaMoveDirections = charaMoveDirections,
                ShowShadow = showShadow,
                DeltaPixelsPerMove = deltaPixelsPerMove,
                CollisionPixels = collisionPixels
            };
        }
    }
}
