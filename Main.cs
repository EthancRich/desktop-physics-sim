namespace DesktopPhysicsSim;

using System;
using System.Linq;
using Godot;

public partial class Main : Node2D
{

	[Export]
	public PackedScene PetScene { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var windowList = DisplayServer.GetScreenCount();

		// Create the first window -- home
		var window = GetWindow();
		window.AlwaysOnTop = true;
		window.Transparent = false;
		window.Borderless = false;
		window.Size = new Vector2I( 500, 300 );

		// Create a basic pet
		CallDeferred( nameof( CreatePetWindow ), window );
	}

	private void CreatePetWindow( Window window )
	{
		// I think the best thing to do here would be to decouple the sprite reference from the child of the pet node
		BasicPet pet = PetScene.Instantiate<BasicPet>();
		Sprite2D sprite = pet.GetNodeOrNull<Sprite2D>( "Sprite2D" );
		pet.Position = new Vector2I( sprite.Texture.GetWidth() / 2, sprite.Texture.GetHeight() / 2 );

		var newWindow = new Window();
		newWindow.Transparent = true;
		newWindow.Borderless = true;
		newWindow.AlwaysOnTop = true;
		newWindow.Position = new Vector2I( window.Position.X - 50, window.Position.Y - 50 );
		newWindow.Size = new Vector2I( sprite.Texture.GetWidth(), sprite.Texture.GetHeight() );


		// TODO: Find a way to update the bounds when the pet leaves the bounds into another monitor
		// How can it get to another monitor if it is bounded in by it?

		newWindow.AddChild( pet );
		GetTree().Root.AddChild( newWindow );

		Rect2I bounds = DisplayServer.ScreenGetUsableRect( newWindow.CurrentScreen ); // This doesn't seem to be finding the right screen
		pet.Initialize( bounds );

		newWindow.Show();
	}
}
