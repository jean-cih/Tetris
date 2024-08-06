using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using Cell;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] cellImages = [
            new BitmapImage(new Uri("../Assets/EmptyCell.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/ICell.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/JCell.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/LCell.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/OCell.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/SCell.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/TCell.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/ZCell.png", UriKind.Relative))
        ];

        private readonly ImageSource[] blockImages = [
            new BitmapImage(new Uri("../Assets/EmptyBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/IBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/JBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/LBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/OBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/SBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/TBlock.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/ZBlock.png", UriKind.Relative))
        ];

        private readonly Image[,] imageControls;

        private readonly int maxDelay = 1000;
        private readonly int minDelay = 100;
        private readonly int delayDecrease = 15;

        private GameState gameState = new();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for(int r = 0; r < grid.Rows; r++)
            {
                for(int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = cellImages[id];
                }
            }
        }
        private void DrawBlock(Cell.Block block)
        {
            foreach(Position p in block.TilePositions())
            {
                imageControls[p.Rows, p.Columns].Opacity = 1;
                imageControls[p.Rows, p.Columns].Source = cellImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Cell.Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];
        }

        private void DrawHeldBlock(Cell.Block heldBlock)
        {
            if(heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }

        private void DrawGhostBlock(Cell.Block block)
        {
            int dropDistance = gameState.BlockDropDistance();

            foreach(Position p in block.TilePositions())
            {
                imageControls[p.Rows + dropDistance, p.Columns].Opacity = 0.25;
                imageControls[p.Rows + dropDistance, p.Columns].Source = cellImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        private async Task GameLoop()
        {
            Draw(gameState);

            while (!gameState.GameOver)
            {
                int delay = System.Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease));
                await Task.Delay(delay);
                gameState.MoveBlockDown();
                Draw(gameState);
            }
        
            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {gameState.Score}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.Z:
                    gameState.RotateBlockCCW();
                    break;
                case Key.C:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.DropBlock();
                    break;
                default:
                    return;
            }

            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }
    }
}