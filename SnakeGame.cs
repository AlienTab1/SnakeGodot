using Godot;
using System;
using System.Collections.Generic;

public partial class SnakeGame : Node2D
{
	private enum Direction { Up, Down, Left, Right }
	private struct Pixel
	{
		public Vector2 Position;
		public Color Color;
		public Pixel(Vector2 position, Color color)
		{
			Position = position;
			Color = color;
		}
	}

	private Direction currentMovement = Direction.Right;
	private List<Pixel> body = new List<Pixel>();
	private Random rand = new Random();
	private int score = 3;  // Start with a body of 3 pixels
	private bool gameover = false;

	private Pixel head;
	private Pixel headOld;
	private Pixel berry;
	private Timer timer;

	public override void _Ready()
	{
		head = new Pixel(new Vector2(256, 256), Colors.Red);
		berry = new Pixel(GetRandomPosition(), Colors.Cyan);

		timer = new Timer();
		timer.WaitTime = 0.5f;
		timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
		AddChild(timer);
		timer.Start();
	}

	public override void _Process(double delta)
	{
		if (gameover)
			return;

		if (Input.IsActionJustPressed("ui_up") && currentMovement != Direction.Down)
			currentMovement = Direction.Up;
		else if (Input.IsActionJustPressed("ui_down") && currentMovement != Direction.Up)
			currentMovement = Direction.Down;
		else if (Input.IsActionJustPressed("ui_left") && currentMovement != Direction.Right)
			currentMovement = Direction.Left;
		else if (Input.IsActionJustPressed("ui_right") && currentMovement != Direction.Left)
			currentMovement = Direction.Right;
	}

	private void OnTimerTimeout()
	{
		if (gameover)
			return;

		headOld = head;
		MoveHead();

		if (head.Position == berry.Position)
		{
			score++;
			berry = new Pixel(GetRandomPosition(), Colors.Cyan);
			// Add a new pixel at the end of the snake
			AddBodyPixel();
		}

		body.Add(new Pixel(headOld.Position, Colors.Green));
		if (body.Count > score)
		{
			body.RemoveAt(0);
		}

		gameover |= CheckCollision();

		if (gameover)
		{
			GD.Print($"Game over, Score: {score - 3}");
			timer.Stop();
		}

		QueueRedraw();
	}

	private void MoveHead()
	{
		switch (currentMovement)
		{
			case Direction.Up:
				head.Position.Y -= 16;
				break;
			case Direction.Down:
				head.Position.Y += 16;
				break;
			case Direction.Left:
				head.Position.X -= 16;
				break;
			case Direction.Right:
				head.Position.X += 16;
				break;
		}
	}

	private void AddBodyPixel()
	{
		Pixel tail = body[0];
		body.Insert(0, new Pixel(tail.Position, Colors.Green));
	}

	private bool CheckCollision()
	{
		if (head.Position.X < 0 || head.Position.X >= 512 || head.Position.Y < 0 || head.Position.Y >= 512)
			return true;

		foreach (var pixel in body)
		{
			if (head.Position == pixel.Position)
				return true;
		}

		return false;
	}

	private Vector2 GetRandomPosition()
	{
		return new Vector2(rand.Next(1, 31) * 16, rand.Next(1, 31) * 16);
	}

	public override void _Draw()
	{
		DrawRect(new Rect2(head.Position, new Vector2(16, 16)), head.Color);
		DrawRect(new Rect2(berry.Position, new Vector2(16, 16)), berry.Color);

		foreach (var pixel in body)
		{
			DrawRect(new Rect2(pixel.Position, new Vector2(16, 16)), pixel.Color);
		}

		DrawBorder();
	}

	private void DrawBorder()
	{
		for (int i = 0; i < 32; i++)
		{
			DrawRect(new Rect2(i * 16, 0, 16, 16), Colors.White);
			DrawRect(new Rect2(i * 16, 496, 16, 16), Colors.White);
		}

		for (int i = 0; i < 32; i++)
		{
			DrawRect(new Rect2(0, i * 16, 16, 16), Colors.White);
			DrawRect(new Rect2(496, i * 16, 16, 16), Colors.White);
		}
	}
}
