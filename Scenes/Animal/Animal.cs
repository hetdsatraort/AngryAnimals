using Godot;
using System;

public partial class Animal : RigidBody2D
{
	private Vector2 DRAG_LIM_MIN = new Vector2(-60, 0);
	private Vector2 DRAG_LIM_MAX = new Vector2(0, 60);

	private const float IMPULSE_MULTIPLIER = -25f;
	private const float IMPULSE_MAX = 2000f;


	[Export] private Label _label;
	[Export] private Sprite2D _arrowSprite;
	[Export] private AudioStreamPlayer2D _launchSound;
	[Export] private AudioStreamPlayer2D _kickSound;
	[Export] private AudioStreamPlayer2D _stretchSound;
	// Called when the node enters the scene tree for the first time.

	private bool _isDragging = false;
	private bool _isDead = false;
	private Vector2 _dragStart = Vector2.Zero;
	private Vector2 _draggedVector = Vector2.Zero;
	private Vector2 _start = Vector2.Zero;
	private float _arrowScaleX = 0.00f;
	public override void _Ready()
	{
		InputEvent += OnInputEvent;
		_start = Position;
		SleepingStateChanged += OnSleepingStateChanged;
		_arrowScaleX = _arrowSprite.Scale.X;
		_arrowSprite.Hide();
		BodyEntered += OnBodyEntered;
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
		string ds = $"";
		// string ds = $"SL:{Sleeping} FR:{Freeze}\n";
		// ds += $"Drag:{_isDragging} DragStart:{_dragStart} Start:{_start}\n";
		// ds += $"DraggedVector:{_draggedVector} Position:{Position}\n";
		ds += $"DraggedVector:{_draggedVector}";
		_label.Text = ds;
	}

	private void ScaleArrow()
	{
		var fraction = CalculateImpulse().Length() / IMPULSE_MAX;
		fraction = Mathf.Clamp(fraction, 0.0f, 1.0f);
		_arrowSprite.Scale = new Vector2(Mathf.Lerp(_arrowScaleX, _arrowScaleX * 2, fraction), _arrowSprite.Scale.Y);
		_arrowSprite.Rotation = (_start - Position).Angle();
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
		_arrowSprite.Hide();
		ApplyCentralImpulse(CalculateImpulse());
		SignalHub.EmitOnAnimalLaunched();
	}

	private void StartDragging()
	{
		_isDragging = true;
		_dragStart = GetGlobalMousePosition();
		_arrowSprite.Show();
	}

	private void HandleDragging() 
	{
		if (_isDragging)
		{
			Vector2 currentMouse = GetGlobalMousePosition();
			Vector2 _newDraggedVector = currentMouse - _dragStart;
			_newDraggedVector = _newDraggedVector.Clamp(DRAG_LIM_MIN, DRAG_LIM_MAX);

			if((_draggedVector - _newDraggedVector).Length() > 0 && !_stretchSound.Playing)
			{
				_stretchSound.Play();
			}

			_draggedVector = _newDraggedVector;

			Position = _start + _draggedVector;
			ScaleArrow();
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

	
    private void OnBodyEntered(Node body)
    {
        if (body is Cup && !_kickSound.Playing)
		{
			_kickSound.Play();
		}
    }
}
