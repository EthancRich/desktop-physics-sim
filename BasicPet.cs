namespace DesktopPhysicsSim;

using Godot;

public partial class BasicPet : Node2D
{
	private bool _dragging = false;
	private int _clickRadius = 64;

	public override void _Input( InputEvent @event )
	{

		if ( @event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left )
		{
			GD.Print( mouseEvent.Position );
			GD.Print( Position );
			if ( (mouseEvent.Position - Position).Length() < _clickRadius )
			{
				// Start dragging if the click is on the sprite.
				if ( !_dragging && mouseEvent.Pressed )
				{
					_dragging = true;
				}
			}
			// Stop dragging if the button is released.
			if ( _dragging && !mouseEvent.Pressed )
			{
				_dragging = false;
			}
		}
		else
		{
			if ( @event is InputEventMouseMotion motionEvent && _dragging )
			{

				// While dragging, move the sprite with the mouse.
				Position = motionEvent.Position;
			}
		}
	}
}
