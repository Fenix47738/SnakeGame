﻿using System;

namespace SnakeGame
{
    public class Snake
    {
        private readonly ConsoleColor _headColor;
        private readonly ConsoleColor _bodyColor;

        public Snake(int initialX, 
            int initialY, 
            ConsoleColor headColor, 
            ConsoleColor bodyColor, 
            int bodyLength = 3)
        {
            _headColor = headColor;
            _bodyColor = bodyColor;

            Head = new Pixel(initialX, initialY, _headColor);

            for(int i = bodyLength; i >= 0; i--)
            {
                Body.Enqueue(new Pixel(Head.X - i - 1, initialY, _bodyColor));
            }

            Draw();
        }

        public void Move(Direction direction, bool eat = false)
        {
            Clear();

            Body.Enqueue(new Pixel(Head.X, Head.Y, _bodyColor));

            if(!eat)
                Body.Dequeue();

            Head = direction switch
            {
                Direction.Right => new Pixel(Head.X + 1, Head.Y, _headColor),
                Direction.Left => new Pixel(Head.X - 1, Head.Y, _headColor),
                Direction.Down => new Pixel(Head.X, Head.Y + 1, _headColor),
                Direction.Up => new Pixel(Head.X, Head.Y - 1, _headColor),
                _ => Head
            };

            Draw();
        }

        public void Draw()
        {
            Head.Draw();

            foreach(Pixel pixel in Body)
            {
                pixel.Draw();
            }
        }

        public void Clear()
        {
            Head.Clear();

            foreach (Pixel pixel in Body)
            {
                pixel.Clear();
            }
        }

        public Pixel Head { get; private set; }
        public Queue<Pixel> Body { get; } = new Queue<Pixel>();
    }
}
