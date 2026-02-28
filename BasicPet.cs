namespace DesktopPhysicsSim;

using System;
using System.Collections.Generic;
using Godot;

public partial class BasicPet : Node2D
{
	private struct MouseSample
	{
		public Vector2 Position;
		public double Time;

		public MouseSample()
		{
			Position = DisplayServer.MouseGetPosition();
			Time = Godot.Time.GetTicksMsec() / 1000.0;
		}
	}
	private Window _petWindow;
	private Vector2 _windowPosition;        // a more accurate count for position
	private bool _dragging = false;
	private bool _grounded = false;
	private int _spriteWidth;
	private int _spriteHeight;
	private Rect2I _bounds;
	private List<MouseSample> _samples = [];
	private float _bounciness = 0.5f;
	private float _friction = 0.2f; // This is bounce friction, potentially rename
	private Vector2 _gravityAccel = new Vector2( 0, 1000 );
	private Vector2 _velocity = new Vector2( 0, 0 ); // Pixels per second
	private float MIN_BOUNCE_VELOCITY = 50f;

	// TODO: Implement Air Resistance
	// TODO: Refactor variables, introduce FRICTION value when sliding on floor
	// TODO: Implement movement across monitors
	// TODO: Implement "charging up" speed with enough frames of high movement, color change? 

	public void Initialize( Rect2I bounds )
	{
		SetBounds( bounds );
	}

	public void SetBounds( Rect2I bounds )
	{
		_bounds = bounds;
	}

	private void SetWindowPosition( Vector2 position )
	{
		_windowPosition = position;
		_petWindow.Position = new Vector2I( (int)_windowPosition.X, (int)_windowPosition.Y );
	}

	public override void _Ready()
	{
		_spriteWidth = GetNode<Sprite2D>( "Sprite2D" ).Texture.GetWidth();
		_spriteHeight = GetNode<Sprite2D>( "Sprite2D" ).Texture.GetHeight();
		_petWindow = GetWindow();
		_windowPosition = _petWindow.Position;
	}

	public override void _Process( double delta )
	{
		if ( !_dragging )
		{
			// Update petWindow position based on the current speed			
			SetWindowPosition( _windowPosition + _velocity * (float)delta );

			// Clamp position into screen, update velocities from bounces
			ResolveBounds();
		}
	}

	private void ResolveBounds()
	{
		int minX = _bounds.Position.X;
		int maxX = _bounds.End.X - _spriteWidth;
		int minY = _bounds.Position.Y;
		int maxY = _bounds.End.Y - _spriteHeight;

		if ( _petWindow.Position.X < minX ) // Does referencing the float or just the int here matter?
		{
			BounceHorizontal( minX );
		}
		else if ( _petWindow.Position.X > maxX )
		{
			BounceHorizontal( maxX );
		}

		if ( _petWindow.Position.Y < minY )
		{
			BounceVertical( minY );
		}
		else if ( _petWindow.Position.Y > maxY )
		{
			BounceVertical( maxY );

			if ( MathF.Abs( _velocity.Y ) < MIN_BOUNCE_VELOCITY )
			{
				_velocity.Y = 0;
				_grounded = true;
			}
			else
			{
				_grounded = false;
			}
		}
	}

	private void BounceHorizontal( int clampX )
	{
		// Clamp Position
		SetWindowPosition( new Vector2( clampX, _windowPosition.Y ) );

		// Update Velocity
		_velocity.X = -_velocity.X * _bounciness;
		_velocity.Y *= 1.0f - _friction;
	}

	private void BounceVertical( int clampY )
	{
		// Clamp Position
		SetWindowPosition( new Vector2( _windowPosition.X, clampY ) );

		// Update Velocity
		_velocity.Y = -_velocity.Y * _bounciness;
		_velocity.X *= 1.0f - _friction;
	}

	// Wonder if this is still necessary? Like theoretically this is important
	public override void _PhysicsProcess( double delta )
	{
		GD.Print( _velocity );
		// GD.Print( _dragging, _grounded, MathF.Abs( _velocity.X ) < MIN_BOUNCE_VELOCITY );
		if ( !_dragging && !_grounded )
		{
			_velocity += _gravityAccel * (float)delta;
		}

		// Apply ground friction
		else if ( !_dragging && _grounded )
		{
			_velocity.X = Mathf.MoveToward( _velocity.X, 0f, 6f );
			if ( MathF.Abs( _velocity.X ) < MIN_BOUNCE_VELOCITY )
			{
				_velocity.X = 0;
			}
		}
	}

	public override void _Input( InputEvent @event )
	{
		if ( @event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left )
		{
			// Start dragging if the click is on the sprite.
			if ( !_dragging && mouseEvent.Pressed )
			{
				_dragging = true;
			}
			// Stop dragging if the button is released.
			if ( _dragging && !mouseEvent.Pressed )
			{
				_dragging = false;
				InitializeWindowVelocity();
			}
		}
		else
		{
			// While dragging, move the sprite with the mouse.
			if ( @event is InputEventMouseMotion motionEvent && _dragging )
			{
				UpdateWindowPositionOnDrag();
			}
		}
	}

	private void UpdateWindowPositionOnDrag()
	{
		_samples.Add( new MouseSample() );
		if ( _samples.Count > 3 )
		{
			_samples.RemoveAt( 0 );
		}

		Vector2I mousePos = DisplayServer.MouseGetPosition();
		Vector2I centeredPos = new Vector2I( mousePos.X - _spriteWidth / 2, mousePos.Y - _spriteHeight / 2 );
		SetWindowPosition( centeredPos );
	}

	private void InitializeWindowVelocity()
	{
		if ( _samples.Count >= 2 ) // DO I actually want this if statement? Is there a better way to do the logic for other cases?
		{
			var last = _samples[^1];
			var first = _samples[0];

			Vector2 dp = last.Position - first.Position;
			double dt = last.Time - first.Time;

			if ( dt > 0 )
			{
				_velocity = dp / (float)dt;
			}
		}

		_samples.Clear();
	}
}