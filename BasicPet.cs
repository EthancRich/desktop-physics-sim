namespace DesktopPhysicsSim;

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
	private bool _dragging = false;
	private int _spriteWidth;
	private int _spriteHeight;
	private Rect2I _bounds;
	private List<MouseSample> _samples = [];
	// TODO: Implement air resistance
	// TODO: Implement bounciness
	// TODO: Implement wall bouncing
	private const int DELTA_SCALAR = 1;
	private Vector2 _gravityAccel = new Vector2( 0, 1000 );
	private Vector2 _velocity = new Vector2( 0, 0 ); // Pixels per second

	public void Initialize( Rect2I bounds )
	{
		SetBounds( bounds );
	}

	public void SetBounds( Rect2I bounds )
	{
		_bounds = bounds;
	}

	public override void _Ready()
	{
		_spriteWidth = GetNode<Sprite2D>( "Sprite2D" ).Texture.GetWidth();
		_spriteHeight = GetNode<Sprite2D>( "Sprite2D" ).Texture.GetHeight();
		_petWindow = GetWindow();
	}

	public override void _Process( double delta )
	{
		if ( !_dragging )
		{
			Vector2 deltaVec2 = new Vector2( (float)delta, (float)delta );

			// Update petWindow position based on the current speed
			Vector2 pos = (Vector2)_petWindow.Position;
			pos += _velocity * deltaVec2 * DELTA_SCALAR;
			_petWindow.Position = (Vector2I)pos;

			// Goes off the left side of the screen
			if ( _petWindow.Position.X < _bounds.Position.X )
			{
				_petWindow.Position = new Vector2I( _bounds.Position.X, _petWindow.Position.Y );
			}

			// Goes off the right side of the screen
			if ( _petWindow.Position.X + _spriteWidth > _bounds.End.X )
			{
				_petWindow.Position = new Vector2I( _bounds.End.X - _spriteWidth, _petWindow.Position.Y );
			}

			// Goes off the top side of the screen
			if ( _petWindow.Position.Y < _bounds.Position.Y )
			{
				_petWindow.Position = new Vector2I( _petWindow.Position.X, _bounds.Position.Y );
			}

			// Goes off the bottom side of the screen
			if ( _petWindow.Position.Y + _spriteHeight > _bounds.End.Y )
			{
				_petWindow.Position = new Vector2I( _petWindow.Position.X, _bounds.End.Y - _spriteHeight );
			}
		}
	}

	public override void _PhysicsProcess( double delta )
	{
		if ( !_dragging )
		{
			Vector2 deltaVec2 = new Vector2( (float)delta, (float)delta );
			_velocity += _gravityAccel * deltaVec2 * DELTA_SCALAR;
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
		_petWindow.Position = centeredPos;
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