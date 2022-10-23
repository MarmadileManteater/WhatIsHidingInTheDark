using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.CompilerServices;
using WhatsHidingInTheDarkSpooky2DJam2022;

public class Door : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	private IDictionary<string, Sprite> Monsters;
	
	public int DoorOpening = 0;
	public int MonsterMoving = 0;
	public bool MonsterCanLeave = false;
	private Sprite ClosedDoor;
	public Sprite OpenDoor;
	private Sprite Darkness;
	public Sprite Hand;
	private Timer OpenDoorTimer;
	private Timer MonsterCanLeaveTimer;
	public RichTextLabel[] BattleOptions;
	public RichTextLabel BattleText;
	public AudioStreamPlayer UnsettlingEncounterMusic;
	private Vector2 OriginalDarknessPosition;
	private World Parent;
	private Sprite Title;
	public IList<Monster> MonstersInCirculation;

	private int CurrentSelection;

	public bool IsDoorOpen = true;
	public string WhoIsAtTheDoor = null;
	public bool IsSomeoneAtDoor
	{
		get
		{
			return WhoIsAtTheDoor != null;
		}
		set
		{
			if (value)
			{
				float randomEntry = GD.Randf();
				float rollingFloor = 0.0f;
				
				foreach (Monster monster in MonstersInCirculation)
				{
					if (randomEntry < monster.EncounterRate + rollingFloor)
					{
						if (randomEntry > rollingFloor)
						{
							SetWhoIsAtDoor(monster.Name);
							break;
						}
					}
					rollingFloor += monster.EncounterRate;
				}
			}
			else
			{
				SetWhoIsAtDoor(null);
			}
		}
	}
	public bool IsEngaged;
	public int Courage
	{
		get
		{
			return Parent.Courage;
		}
		set
		{
			Parent.SetCourage(value);
		}
	}

	public bool NoMoreMonsters;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ClosedDoor = GetNode<Sprite>("Door Closed");
		OpenDoor = GetNode<Sprite>("Door Open");
		Darkness = GetNode<Sprite>("Darkness");
		Monsters = new Dictionary<string, Sprite> {
			{ "TongueSpider", GetNode<Sprite>("TongueSpider") },
			{ "Mask", GetNode<Sprite>("Mask") },
			{ "Ghost", GetNode<Sprite>("Ghost") },
			{ "EyeCrab", GetNode<Sprite>("EyeCrab") },
			{ "Human-Ghost", GetNode<Sprite>("Human-Ghost") },
			{ "Human-Witch", GetNode<Sprite>("Human-Witch") }
		};
		OpenDoorTimer = GetNode<Timer>("OpenDoorTimer");
		MonsterCanLeaveTimer = GetNode<Timer>("MonsterCanLeaveTimer");
		BattleOptions = new RichTextLabel[] {
			GetNode<RichTextLabel>("BattleOptions/OptionA"),
			GetNode<RichTextLabel>("BattleOptions/OptionB"),
			GetNode<RichTextLabel>("BattleOptions/OptionC"),
			GetNode<RichTextLabel>("BattleOptions/OptionD")
		};
		BattleText = GetNode<RichTextLabel>("BattleText/What Are You Going To Do");
		OriginalDarknessPosition = new Vector2(Darkness.Position.x, Darkness.Position.y);
		UnsettlingEncounterMusic = GetNode<AudioStreamPlayer>("Unsettling Encounter Music");
		Hand = GetNode<Sprite>("BattleOptions/Hand");
		Parent = GetParent<World>();
		MonstersInCirculation = Monster.Monsters.Select(monster => monster).ToList();
		Title = GetNode<Sprite>("Title");
	}
	public void SetWhoIsAtDoor(string whoIs)
	{
		if (WhoIsAtTheDoor != null)
		{
			Monsters[WhoIsAtTheDoor].Hide();
		}
		WhoIsAtTheDoor = whoIs;
		if (WhoIsAtTheDoor != null)
		{
			Monsters[WhoIsAtTheDoor].Show();
		}
	}

	public void ToggleDoor()
	{
		IsDoorOpen = !IsDoorOpen;
		if (IsDoorOpen)
		{
			ClosedDoor.Hide();
			DoorOpening = 200;
			if (IsSomeoneAtDoor)
			{
				StartTimer();
			}
		} 
		else
		{
			ClosedDoor.Show();
			DoorOpening = 0;
			Darkness.Position = OriginalDarknessPosition;
			SetWhoIsAtDoor(null);
		}
	}

	private void StartTimer()
	{
		OpenDoorTimer.Start(2);
	}

	public void OnOpenDoorTimeout()
	{
		// Enter battle
		if (WhoIsAtTheDoor != null)
		{
			Monster monsterAtTheDoor = MonstersInCirculation.Where(monster => monster.Name == WhoIsAtTheDoor).FirstOrDefault();
			GD.Print($"Monster: {monsterAtTheDoor.LocaleName}");
			if (monsterAtTheDoor != null)
			{
				// If the monster has a battle menu,
				if (monsterAtTheDoor.ActionResponses != null)
				{
					for (int i = 0; i < monsterAtTheDoor.ActionResponses.Length; i++)
					{
						var actionResponse = monsterAtTheDoor.ActionResponses[i];
						string actionName = actionResponse.Name;
						var option = BattleOptions[i];
						option.Text = actionName;
						option.Show();
					}
					OpenDoor.Hide();
					UnsettlingEncounterMusic.Play();
					Hand.Show();
					string n = monsterAtTheDoor.LocaleName.StartsWith("E") ? "n" : "";

					BattleText.Text = $"A{n} {monsterAtTheDoor.LocaleName} appeared!\nWhat are you going to do?";
					BattleText.Show();
					IsEngaged = true;
					CurrentSelection = (int) (GD.Randi() % monsterAtTheDoor.ActionResponses.Length);
				}
			}
		}
		// Only run once each time
		OpenDoorTimer.Stop();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (IsEngaged)
		{
			if (!IsDoorOpen)
			{
				// Trying to prevent a click from occuring and 
				ClosedDoor.Hide();
			}
			if (Input.IsActionJustPressed("ui_down"))
			{
				CurrentSelection += 1;
				if (CurrentSelection > 2)
				{
					CurrentSelection = 0;
				}
			}
			if (Input.IsActionJustPressed("ui_up"))
			{
				CurrentSelection -= 1;
				if (CurrentSelection < 0)
				{
					CurrentSelection = 2; 
				}
			}
			Hand.Position = new Vector2(Hand.Position.x, 50 + CurrentSelection * 50);
			if (Input.IsActionJustPressed("ui_accept") && MonsterMoving == 0)
			{
				if (MonsterCanLeave)
				{
					MonsterMoving = 200;
					MonsterCanLeave = false;
				}
				else
				{
					Monster monsterAtTheDoor = MonstersInCirculation.Where(monster => monster.Name == WhoIsAtTheDoor).FirstOrDefault();
					BattleText.Text = monsterAtTheDoor.ActionResponses[CurrentSelection].ResponseText;
					Courage += monsterAtTheDoor.ActionResponses[CurrentSelection].CourageModifier;
					MonsterCanLeave = true;
					MonsterCanLeaveTimer.Start(4);

				}
			}
			// I don't know if this does anything, but it might fix a bug
			// for some reason IsActionJustPressed is triggering twice in a row like 3 seconds
			// after the action is pressed
			if (MonsterMoving > 0 && BattleText.Text.EndsWith("What are you going to do?"))
			{
				MonsterMoving = 0;
			}
			if (MonsterMoving > 0)
			{
				Monsters[WhoIsAtTheDoor].Position += new Vector2(2, 0);
				MonsterMoving--;
				if (MonsterMoving <= 0) {
					Monsters[WhoIsAtTheDoor].Hide();
					Monsters[WhoIsAtTheDoor].Position -= new Vector2(2 * 200, 0);
					IsEngaged = false;
					OpenDoor.Show();
					UnsettlingEncounterMusic.Stop();
					Hand.Hide();
					BattleText.Hide();
					foreach (var option in BattleOptions)
					{
						option.Hide();
					}
					Monster monsterAtTheDoor = MonstersInCirculation.Where(monster => monster.Name == WhoIsAtTheDoor).FirstOrDefault();
					
					// When you engage a monster, it removes it from the pool
					MonstersInCirculation.Remove(monsterAtTheDoor);
					NoMoreMonsters = !MonstersInCirculation.Where(monster => monster.ActionResponses != null).Any();
				}
			}
		}
		else
		{
			if (Input.IsActionJustPressed("ui_accept"))
			{
				if (Title.Visible)
				{
					Title.Hide();
				}
				ToggleDoor();
			}
			if (DoorOpening > 0)
			{
				Darkness.Position += new Vector2(-4, 0);
				DoorOpening--;
			}
		}
	}
	private void OnMonsterCanLeave()
	{
		MonsterMoving = 200;
		MonsterCanLeave = false;
		MonsterCanLeaveTimer.Stop();
	}

}
