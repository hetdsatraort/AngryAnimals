using Godot;
using System;

public partial class Water : Area2D
{
	[Export] private AudioStreamPlayer2D _splashSound;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

    private void OnBodyEntered(Node2D body)
	{
		if (body is Animal animal)
		{
			animal.Sleeping = true;
			_splashSound.GlobalPosition = animal.Position;
			_splashSound.Play();
			animal.CallDeferred(nameof(animal.HandleDeath));
		}
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
