using Godot;
using System;

public partial class LevelButton : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private int _levelNumber;

	[Export] private Label _levelLabel;
	[Export] private Label _scoreLabel;
	public override void _Ready()
	{
		MouseEntered += _OnMouseEntered;
		MouseExited += _OnMouseExited;
		Pressed += _OnPressed;
		_levelLabel.Text = $"{_levelNumber}";
		_scoreLabel.Text = $"{ScoreManager.Instance.LevelScores.GetBestScore(_levelNumber).ToString("D3")}";
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public void _OnMouseEntered()
	{
		_animationPlayer.Play("ButtonHover");
	}

	
    private void _OnMouseExited()
	{
		_animationPlayer.Play("RESET");
	}

	private void _OnPressed()
	{
		ScoreManager.LevelSelected = _levelNumber;
		// _animationPlayer.Play("ButtonPressed");
		GetTree().ChangeSceneToFile($"res://Scenes/LevelBase/Level{_levelNumber}.tscn");
	}
}
