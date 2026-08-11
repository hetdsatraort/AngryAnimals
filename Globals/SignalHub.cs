using Godot;
using System;

public partial class SignalHub : Node
{
	// Called when the node enters the scene tree for the first time.
	public static SignalHub Instance { get; private set; }
	[Signal] public delegate void AnimalLaunchedEventHandler();	
	[Signal] public delegate void AnimalDiedEventHandler();
	[Signal] public delegate void CupDestroyedEventHandler(int numRemaining);
	public override void _Ready()
	{
		Instance = this;
	}

	public static void EmitOnAnimalLaunched()
	{
		Instance.EmitSignal(SignalName.AnimalLaunched);
	}

	public static void EmitOnAnimalDied()
	{
		Instance.EmitSignal(SignalName.AnimalDied);
	}

	public static void EmitOnCupDestroyed(int remainingCups)
	{
		Instance.EmitSignal(SignalName.CupDestroyed, remainingCups);
	}
}
