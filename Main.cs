namespace DesktopPhysicsSim;

using Godot;

public partial class Main : Node2D
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Sprite2D icon = GetNode<Sprite2D>( "Icon" );
		float width = icon.Texture.GetWidth() * icon.Scale.X;
		float height = icon.Texture.GetHeight() * icon.Scale.Y;

		var window = GetWindow();
		window.AlwaysOnTop = true;
		window.Transparent = true;
		window.Borderless = true;
		window.Size = new Vector2I( (int)width, (int)height );

	}
}
