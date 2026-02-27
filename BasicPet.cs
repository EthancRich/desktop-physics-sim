namespace DesktopPhysicsSim;

using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public partial class BasicPet : Node2D
{
	private bool _dragging = false;
	private int _spriteWidth;
	private int _spriteHeight;

	private const int DELTA_SCALAR = 1;
	private Vector2 _gravityAccel = new Vector2( 0, 1000 );
	private Vector2 _velocity = new Vector2( 0, 0 ); // Pixels per second

	private struct MouseSample
	{
		public Vector2 Position;
		public double Time;
	}
	private List<MouseSample> _samples = [];

	public override void _Ready()
	{
		_spriteWidth = GetNode<Sprite2D>( "Sprite2D" ).Texture.GetWidth();
		_spriteHeight = GetNode<Sprite2D>( "Sprite2D" ).Texture.GetHeight();
	}

	public override void _Process( double delta )
	{
		if ( !_dragging )
		{
			Vector2 deltaVec2 = new Vector2( (float)delta, (float)delta );

			Vector2 pos = (Vector2)GetWindow().Position;
			pos += _velocity * deltaVec2 * DELTA_SCALAR;
			GetWindow().Position = (Vector2I)pos;
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

				if ( _samples.Count >= 2 )
				{
					var last = _samples[^1];
					var first = _samples[0];

					Vector2 dp = last.Position - first.Position;
					double dt = last.Time - first.Time;

					if ( dt > 0 )
					{
						_velocity = dp / (float)dt;
					}
					GD.Print( _velocity );
				}

				_samples.Clear();
			}
		}
		else
		{
			if ( @event is InputEventMouseMotion motionEvent && _dragging )
			{
				MouseSample sample = new MouseSample
				{
					Position = DisplayServer.MouseGetPosition(),
					Time = Time.GetTicksMsec() / 1000.0
				};
				_samples.Add( sample );

				if ( _samples.Count > 3 )
				{
					_samples.RemoveAt( 0 );
				}


				// // While dragging, move the sprite with the mouse.
				// var prevPos = GetWindow().Position;
				var newPos = DisplayServer.MouseGetPosition();
				GetWindow().Position = newPos;

				// _velocity = (newPos - prevPos);

			}
		}
	}
}
