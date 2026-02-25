namespace DesktopPhysicsSim;

using System;
using Godot;

public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Vector2I screenTwoSize = DisplayServer.ScreenGetSize( 1 );
		Vector2I screenTwoOrigin = DisplayServer.ScreenGetPosition( 1 );

		Console.WriteLine( DisplayServer.GetScreenCount() );
		Console.WriteLine( DisplayServer.ScreenGetPosition( 0 ) );
		Console.WriteLine( DisplayServer.ScreenGetPosition( 1 ) );

		DisplayServer.WindowSetSize( screenTwoSize );
		DisplayServer.WindowSetPosition( screenTwoOrigin );
		// DisplayServer.WindowSetFlag( DisplayServer.WindowFlags.Borderless, true );
		DisplayServer.WindowSetFlag( DisplayServer.WindowFlags.AlwaysOnTop, true );
		GetTree().Root.TransparentBg = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process( double delta )
	{
	}
}
