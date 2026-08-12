using Godot;
using System;

public partial class LevelBase : Node
{
	[Export] private PackedScene _animalScene;
	[Export] private PackedScene _mainScene;
	[Export] private Marker2D _spawnMarker;
	// Called when the node enters the scene tree for the first time.

	public override void _EnterTree()
	{
		
		Cup.NumCups = 0;
		GD.Print($"LevelBase Initialized: {Cup.NumCups}");
	}
	public override void _Ready()
	{
		SignalHub.Instance.AnimalDied += SpawnAnimal;
		SpawnAnimal();
	}
    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event.IsActionPressed("ui_cancel"))
		{
			GetTree().ChangeSceneToPacked(_mainScene);
		}
    }

	private void SpawnAnimal()
	{
		Animal animal = _animalScene.Instantiate<Animal>();
		animal.Position = _spawnMarker.GlobalPosition;
		CallDeferred(MethodName.AddChild, animal);
	}

	public override void _ExitTree()
	{
		SignalHub.Instance.AnimalDied -= SpawnAnimal;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
