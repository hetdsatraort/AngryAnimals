using Godot;
using System;

public partial class Cup : StaticBody2D
{
	// Called when the node enters the scene tree for the first time.
	
	[Export] private AnimationPlayer _animationPlayer;

	public static int NumCups = 0;
	public override void _Ready()
	{
		NumCups++;
		GD.Print($"Cup Initialized: {NumCups}");
		_animationPlayer.AnimationFinished += OnAnimationFinished;
	}

    private void OnAnimationFinished(StringName animName)
	{
		GD.Print($"Cup Animation Finished: {animName}, destroying cup, remaining cups: {NumCups}");
		QueueFree();
		NumCups--;
		SignalHub.EmitOnCupDestroyed(NumCups);
	}

    public void Vanish()
	{
		_animationPlayer.Play("vanish");
	}
}
