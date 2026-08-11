using Godot;
using System;

public partial class Animal : RigidBody2D
{
	private Vector2 DRAG_LIM_MIN = new Vector2(-60, 0);
	private Vector2 DRAG_LIM_MAX = new Vector2(0, 60);

	private const float IMPULSE_MULTIPLIER = -25f;


	[Export] private Label _label;
	[Export] private AudioStreamPlayer2D _launchSound;
	[Export] private AudioStreamPlayer2D _kickSound;
	[Export] private AudioStreamPlayer2D _stretchSound;
	// Called when the node enters the scene tree for the first time.

	private bool _isDragging = false;
	private bool _isDead = false;
	private Vector2 _dragStart = Vector2.Zero;
	private Vector2 _draggedVector = Vector2.Zero;
	private Vector2 _start = Vector2.Zero;
	public override void _Ready()
	{
		InputEvent += OnInputEvent;
		_start = Position;
		SleepingStateChanged += OnSleepingStateChanged;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.

    public override void _PhysicsProcess(double delta)
	{
		UpdateDebug();
		HandleDragging();
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionReleased("drag") && _isDragging)
		{
			CallDeferred(nameof(HandleRelease));
		}
	}

	private void UpdateDebug()
	{
		string ds = $"SL:{Sleeping} FR:{Freeze}\n";
		ds += $"Drag:{_isDragging} DragStart:{_dragStart} Start:{_start}\n";
		ds += $"DraggedVector:{_draggedVector} Position:{Position}\n";
		_label.Text = ds;
	}

	private Vector2 CalculateImpulse()
	{
		return _draggedVector * IMPULSE_MULTIPLIER;
	}

	private void HandleRelease()
	{
		GD.Print($"HandleRelease: Final DraggedVector:{_draggedVector} Impulse:{CalculateImpulse()}");
		_isDragging = false;
		Freeze = false;
		_launchSound.Play();
		ApplyCentralImpulse(CalculateImpulse());
		SignalHub.EmitOnAnimalLaunched();
	}

	private void StartDragging()
	{
		_isDragging = true;
		_dragStart = GetGlobalMousePosition();
	}

	private void HandleDragging() 
	{
		if (_isDragging)
		{
			Vector2 currentMouse = GetGlobalMousePosition();
			_draggedVector = currentMouse - _dragStart;
			_draggedVector = _draggedVector.Clamp(DRAG_LIM_MIN, DRAG_LIM_MAX);
			Position = _start + _draggedVector;
		}
	}

	public void HandleDeath()
	{
		if (_isDead) return;
		_isDead = true;
		SignalHub.EmitOnAnimalDied();
		QueueFree();
	}

	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
		if (@event.IsActionPressed("drag"))
		{
			InputEvent -= OnInputEvent;
			StartDragging();
		}
		// else if (@event.IsActionPressed("kick"))
		// {
		// 	_kickSound.Play();
		// }
		// else if (@event.IsActionPressed("stretch"))
		// {
		// 	_stretchSound.Play();
		// }
    }

    private void OnSleepingStateChanged()
	{
		if(!Sleeping) return;

		var collidingBodies = GetCollidingBodies();
		foreach (var body in collidingBodies)
		{
			if (body is Cup cup)
			{
				cup.Vanish();
				HandleDeath();
				return;
			}
		}
	}
}
