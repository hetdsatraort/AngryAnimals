using Godot;
using System;

public partial class UserInterface : Control
{
	// Called when the node enters the scene tree for the first time.
	 [Export] private Label _attemptsLabel;
	 [Export] private Label _levelLabel;
	 [Export] private VBoxContainer _levelCompleteContainer;
	 [Export] private AudioStreamPlayer2D _levelCompleteSound;

	 private int _attempts = -1;
	
	public override void _Ready()
	{
		OnAnimalLaunched();
		SignalHub.Instance.AnimalLaunched += OnAnimalLaunched;
		SignalHub.Instance.CupDestroyed += OnCupDestroyed;
		_levelLabel.Text = $"Level {ScoreManager.LevelSelected}";
	}

    private void OnCupDestroyed(int cupsRemaining)
	{
		GD.Print($"Cups left: {cupsRemaining}");
		if (cupsRemaining == 0)
		{
			_levelCompleteContainer.Show();
			_levelCompleteSound.Play();
			ScoreManager.SetScoreForCurrentLevel(_attempts);
		}
	}

    private void OnAnimalLaunched()
	{
		_attempts++;
		_attemptsLabel.Text = $"Attempts: {_attempts}";
	}

	public override void _ExitTree()
	{
		SignalHub.Instance.AnimalLaunched -= OnAnimalLaunched;
		SignalHub.Instance.CupDestroyed -= OnCupDestroyed;
	}
}
