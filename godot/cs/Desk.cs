using Godot;
using System;

public class Desk : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	private Sprite[] Bottles;
	private int CurrentBottle = 2;
	public int PotionsBrewed = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Bottles = new Sprite[]
		{
			GetNode<Sprite>("Bottle A"),
			GetNode<Sprite>("Bottle B"),
			GetNode<Sprite>("Bottle C")
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			int newBottle = (int)(GD.Randi() % 3);
			while (newBottle == CurrentBottle)
			{
				newBottle = (int)(GD.Randi() % 3);
			}
			Bottles[CurrentBottle].Hide();
			CurrentBottle = newBottle;
			Bottles[CurrentBottle].Show();
			PotionsBrewed++;
		}
	}
}
