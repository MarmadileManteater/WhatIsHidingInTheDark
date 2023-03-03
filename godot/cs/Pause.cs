using Godot;
using System;

public class Pause : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	private World Parent;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Parent = GetParent<World>();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{


		if (!Parent.IsGameOver)
		{
			if (Input.IsActionJustPressed("ui_select"))
			{
				Parent.IsPaused = !Parent.IsPaused;
				GetTree().Paused = Parent.IsPaused;
				if (Parent.IsPaused && !Parent.PausedText.Visible)
				{
					Parent.PausedText.Show();
				}
				else if (!Parent.IsPaused && Parent.PausedText.Visible)
				{
					Parent.PausedText.Hide();
				}
			}
		} 
		else
		{
			if (Input.IsActionJustPressed("ui_select"))
			{
				// Reset the state if the game is over
				Parent.ResetState();
			}
		}
	}
}
