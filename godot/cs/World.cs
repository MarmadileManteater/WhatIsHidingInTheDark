using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using WhatsHidingInTheDarkSpooky2DJam2022;

public class World : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	static private int MAX_COURAGE = 9;
	static private string[] SCENE_NAMES = new string[] { 
		"Online",
		"Desk",
		"Door", 
		"GameOver",
		"YouWin"
	};
	static private string[] EMOTION_NAMES = new string[] { 
		"Chill",
		"Startled",
		"Uneasy",
		"Shaken",
		"Anxious",
		"Fearful",
		"Petrified",
		"Perturbed",
		"Blanched"
	};
	private Timer __anxietyTimer;
	private Timer KnockKnockTimer;
	private AudioStreamPlayer[] __knockSoundEffects;
	private Node EmotionMeter;
	private Sprite MostRecentEmotion;
	private AnimatedSprite KnockKnock;
	public Sprite PausedText;
	private AudioStreamPlayer UnsettlingMusicPlayer;
	private IDictionary<string, Node> Scenes;

	private int DoorsOpened;
	private float GameTime;
	public bool IsGameOver;
	public bool IsPaused;
	// Courage is the meter to tell if the player has died
	// Think of it as health, but we will represent this to the player as 
	// a fear meter which fills up inversly proportional to this number
	// When this number is MAX_COURAGE, the fear meter will be empty,
	// and when this number is 0, the fear meter will be full
	public int Courage;
	private string Location;
	private bool IsSomeoneAtTheDoor
	{
		get
		{
			return ((Door)Scenes["Door"]).IsSomeoneAtDoor;
		}
		set
		{
			((Door)Scenes["Door"]).IsSomeoneAtDoor = value;

		}
	}
	private bool IsDoorOpen
	{
		get
		{
			return ((Door)Scenes["Door"]).IsDoorOpen; 
		}
		set
		{
			((Door)Scenes["Door"]).IsDoorOpen = value;

		}
	}
	private bool WasDoorJustOpen;
	private bool IsEngagedInEncounter
	{
		get
		{
			return ((Door)Scenes["Door"]).IsEngaged;
		}
		set
		{
			((Door)Scenes["Door"]).IsEngaged = value;
		}
	}
	private bool NoMoreMonsters
	{
		get
		{
			return ((Door)Scenes["Door"]).NoMoreMonsters;
		}
		set
		{
			((Door)Scenes["Door"]).NoMoreMonsters = value;
		}
	}

	private AudioStreamPlayer UnsettlingEncounterMusic
	{
		get
		{
			return ((Door)Scenes["Door"]).UnsettlingEncounterMusic;
		}
	}
	private int DoorOpening
	{
		get
		{
			return ((Door)Scenes["Door"]).DoorOpening;
		}
		set
		{
			((Door)Scenes["Door"]).DoorOpening = value;

		}
	}
	private int MonsterMoving
	{
		get
		{
			return ((Door)Scenes["Door"]).MonsterMoving;
		}
		set
		{
			((Door)Scenes["Door"]).MonsterMoving = value;

		}
	}
	private bool MonsterCanLeave
	{
		get
		{
			return ((Door)Scenes["Door"]).MonsterCanLeave;
		}
		set
		{
			((Door)Scenes["Door"]).MonsterCanLeave = value;

		}
	}

	private IList<Monster> MonstersInCirculation
	{
		get
		{
			return ((Door)Scenes["Door"]).MonstersInCirculation;
		}
		set
		{
			((Door)Scenes["Door"]).MonstersInCirculation = value;

		}
	}

	private Sprite OpenDoor
	{
		get {
			return ((Door)Scenes["Door"]).OpenDoor;
		}
		set
		{
			((Door)Scenes["Door"]).OpenDoor = value;

		}
	}
	private RichTextLabel[] BattleOptions
	{
		get
		{
			return ((Door)Scenes["Door"]).BattleOptions;
		}
	}
	private RichTextLabel BattleText
	{
		get
		{
			return ((Door)Scenes["Door"]).BattleText;
		}
	}
	private Sprite Hand
	{
		get
		{
			return ((Door)Scenes["Door"]).Hand;
		}
	}
	private int PotionsBrewed
	{
		get
		{
			return ((Desk)Scenes["Desk"]).PotionsBrewed;
		}
		set
		{
			((Desk)Scenes["Desk"]).PotionsBrewed = value;

		}
	}
	// Called when the node enters the scene tree for the first time.
	private int CurrentFearLevel
	{
		get
		{
			return MAX_COURAGE - Courage;
		}
	}
	public override void _Ready()
	{
		__anxietyTimer = GetNode<Timer>("AnxietyTimer");
		__knockSoundEffects = new AudioStreamPlayer[] {
			GetNode<AudioStreamPlayer>("Knock1"),
			GetNode<AudioStreamPlayer>("Knock2")
		};
		Courage = MAX_COURAGE;
		EmotionMeter = GD.Load<PackedScene>("res://scenes/Emotion-Meter.tscn").Instance();
		AddChild(EmotionMeter);
		UnsettlingMusicPlayer = GetNode<AudioStreamPlayer>("Unsettling Loop A");
		UnsettlingMusicPlayer.Play();
		GD.Print(UnsettlingMusicPlayer.IsPlaying());
		MostRecentEmotion = EmotionMeter.GetNode<Sprite>(EMOTION_NAMES[CurrentFearLevel]);
		Scenes = new Dictionary<string, Node>();
		for (int i = 0; i < SCENE_NAMES.Length; i++)
		{
			// Preload all of our scenes
			string sceneName =	SCENE_NAMES[i];
			Scenes[sceneName] = GD.Load<PackedScene>($"res://scenes/{sceneName}.tscn").Instance();
		}
		SetLocation("Door");
		StartTimer();
		KnockKnock = GetNode<AnimatedSprite>("KnockKnock");
		KnockKnockTimer = GetNode<Timer>("KnockKnockTimer");
		PausedText = GetNode<Sprite>("Paused");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (!IsPaused)
		{
			GameTime += delta;
			if (IsDoorOpen && !WasDoorJustOpen)
			{
				DoorsOpened++;
			}
			WasDoorJustOpen = IsDoorOpen;
			if (IsEngagedInEncounter && UnsettlingMusicPlayer.Playing)
			{
				UnsettlingMusicPlayer.Stop();
			}
			if (!IsEngagedInEncounter && !UnsettlingMusicPlayer.Playing)
			{
				UnsettlingMusicPlayer.Play();
			}
			if (NoMoreMonsters && UnsettlingMusicPlayer.Playing && Location != "YouWin") 
			{
				SetLocation("YouWin");
			}
			if (Input.IsActionPressed("ui_left"))
			{

			}
			if (Input.IsActionPressed("ui_right"))
			{

			}
			if (Input.IsActionPressed("ui_up"))
			{

			}
			if (Input.IsActionJustPressed("ui_down"))
			{
				switch (Location)
				{
					case "Door":
						if (!IsDoorOpen && !IsEngagedInEncounter)
							SetLocation("Desk");
						break;
					case "Desk":
						SetLocation("Door");
						break;
					default:
						break;
				}
			}
		}
	}

	public void SetCourage(int courage)
	{
		Courage = courage;
		MostRecentEmotion.Hide();
		if (CurrentFearLevel < EMOTION_NAMES.Length)
		{
			MostRecentEmotion = EmotionMeter.GetNode<Sprite>(EMOTION_NAMES[CurrentFearLevel]);
			MostRecentEmotion.Show();
		}
		if (Courage < 1)
		{
			// You were so frightened that you retreated to your room.
			// You were ultimately completely fine, but it was a bummer to
			// miss out on the rest of a killer halloween
			SetLocation("GameOver");
		}
	}

	private void SetLocation(string newLocation)
	{
		// If location has never been set before, there will be no child to remove
		if (Location != null)
		{
			RemoveChild(Scenes[Location]);
		}
		Location = newLocation;
		AddChild(Scenes[Location]);
		if (Location == "YouWin" || Location == "GameOver")
		{
			IsGameOver = true;
			__anxietyTimer.Stop();
			var potionsBrewedLabel = Scenes[Location].GetNode<RichTextLabel>("Background/Potions Brewed");
			var doorsOpenedLabel = Scenes[Location].GetNode<RichTextLabel>("Background/Doors Opened");
			var timeSpentLabel = Scenes[Location].GetNode<RichTextLabel>("Background/Time Spent");

			potionsBrewedLabel.Text = $"{PotionsBrewed}";
			doorsOpenedLabel.Text = $"{DoorsOpened}";
			timeSpentLabel.Text = $"{((float)Math.Floor(GameTime*100))/100.0} seconds";
		}
	}

	private int GetWaitTime()
	{
		int multiplier = (int)(GD.Randi() % 3);
		// Every 20, 40, 60 seconds or so
		return Math.Abs(multiplier * 4);
	}

	private void StartTimer()
	{
		__anxietyTimer.WaitTime = GetWaitTime();
		__anxietyTimer.Start();
	}

	protected void OnAnxietyTimeout()
	{
		if (IsDoorOpen)
			return;
		if (Location == "GameOver" || Location == "YouWin")
			return;
		if (IsEngagedInEncounter)
			return;

		if (IsSomeoneAtTheDoor)
		{
			// Scared to open the door?
			SetCourage(Courage - 1);
		} 
		else
		{
			// Well, someone is now.
			IsSomeoneAtTheDoor = true;
		}
		PlayKnockSoundEffect();
		if (Location == "Online")
		{
			SetLocation("Desk");
		}

		__anxietyTimer.WaitTime = GetWaitTime();
	}

	private void PlayKnockSoundEffect()
	{
		if (Location == "GameOver" || Location == "YouWin")
			return;
		var effect = __knockSoundEffects[GD.Randi() % 2];
		effect.Play();
		KnockKnock.Show();
		KnockKnockTimer.Start(1.5f);
	}
	
	public void OnKnockKnockTimeout()
	{
		KnockKnock.Hide();
		KnockKnockTimer.Stop();
	}

	public void ResetState()
	{
		IsPaused = false;
		GetTree().Paused = false;
		GameTime = 0;
		PotionsBrewed = 0;
		DoorsOpened = 0;
		StartTimer();
		SetCourage(MAX_COURAGE);
		if (UnsettlingEncounterMusic.Playing)
		{
			UnsettlingEncounterMusic.Stop();
		}
		if (!UnsettlingMusicPlayer.Playing)
		{
			UnsettlingMusicPlayer.Play();
		}
		IsSomeoneAtTheDoor = false;
		IsEngagedInEncounter = false;
		IsDoorOpen = true;
		DoorOpening = 0;
		MonsterMoving = 0;
		MonsterCanLeave = false;
		SetLocation("Door");
		IsGameOver = false;
		MonstersInCirculation = Monster.Monsters.Select(monster => monster).ToList();
		NoMoreMonsters = false;
		OpenDoor.Show();
		foreach (var option in BattleOptions)
		{
			option.Hide();
		}
		BattleText.Hide();
		Hand.Hide();
		KnockKnock.Hide();
		KnockKnockTimer.Stop();
	}
}
