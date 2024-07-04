using System;
using System.Diagnostics;
using static System.Console;

namespace SnakeGame
{
    public class Program
    {
        private const int MapWidth = 30;
        private const int MapHeight = 20;

        private const int ScreenWidth = MapWidth * 3;
        private const int ScreenHeight = MapHeight * 3;

        private const int FrameMs = 200;

        private const ConsoleColor BorderColor = ConsoleColor.Gray;

        private const ConsoleColor HeadColor = ConsoleColor.DarkBlue;
        private const ConsoleColor BodyColor = ConsoleColor.Cyan;

        private const ConsoleColor FoodColor = ConsoleColor.Green;

        private static readonly Random Random = new Random();

        public static void Main(string[] args)
        {
            SetWindowSize(ScreenWidth, ScreenHeight);
            SetBufferSize(ScreenWidth, ScreenHeight);
            CursorVisible = false;

            while(true)
            {
                StartGame();
                Thread.Sleep(1000);
                ReadKey();
            }
        }

        private static void StartGame()
        {
            Clear();

            DrawBorder();

            Direction currentMovement = Direction.Right;

            Snake snake = new Snake(10, 5, HeadColor, BodyColor);

            Pixel food = GenFood(snake);
            food.Draw();

            int score = 0;

            int lagMs = 0;

            Stopwatch sw = new Stopwatch();

            while (true)
            {
                sw.Restart();

                Direction oldMovement = currentMovement;

                while (sw.ElapsedMilliseconds <= FrameMs - lagMs)
                    if (currentMovement == oldMovement)
                        currentMovement = ReadMovement(currentMovement);

                sw.Restart();

                if (snake.Head.X == food.X && snake.Head.Y == food.Y)
                {
                    snake.Move(currentMovement, true);

                    food = GenFood(snake);
                    food.Draw();

                    score++;

                    Task.Run(() => Beep(1200, 200));
                } else
                    snake.Move(currentMovement);

                if (snake.Head.X == MapWidth - 1
                  || snake.Head.X == 0
                  || snake.Head.Y == MapHeight - 1
                  || snake.Head.Y == 0
                  || snake.Body.Any(b => b.X == snake.Head.X && b.Y == snake.Head.Y))
                    break;

                lagMs = (int)sw.ElapsedMilliseconds;
            }

            snake.Clear();
            food.Clear();

            SetCursorPosition(ScreenWidth / 3, ScreenHeight / 2);
            WriteLine($"Game over, score: {score}");

            Task.Run(() => Beep(200, 600));
        }

        private static Pixel GenFood(Snake snake)
        {
            Pixel food;

            do
            {
                food = new Pixel(Random.Next(1, MapWidth - 2), Random.Next(1, MapHeight - 2), FoodColor);
            } while (snake.Head.X == food.X && snake.Head.Y == food.Y
            || snake.Body.Any(b => b.X == food.X && b.Y == food.Y));

            return food;
        }

        private static Direction ReadMovement(Direction currentDirection)
        {
            if (!KeyAvailable)
                return currentDirection;

            ConsoleKey key = ReadKey(true).Key;

            currentDirection = key switch
            {
                ConsoleKey.UpArrow when currentDirection != Direction.Down => Direction.Up,
                ConsoleKey.DownArrow when currentDirection != Direction.Up => Direction.Down,
                ConsoleKey.LeftArrow when currentDirection != Direction.Right => Direction.Left,
                ConsoleKey.RightArrow when currentDirection != Direction.Left => Direction.Right,
                _ => currentDirection
            };

            return currentDirection;
        }

        public static void DrawBorder()
        {
            for (int row = 0; row < MapWidth; row++)
            {
                new Pixel(row, 0, BorderColor).Draw();
                new Pixel(row, MapHeight - 1, BorderColor).Draw();
            }

            for (int col = 0; col < MapHeight; col++)
            {
                new Pixel(0, col, BorderColor).Draw();
                new Pixel(MapWidth - 1, col, BorderColor).Draw();
            }
        }
    }
}