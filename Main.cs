namespace DesktopPhysicsSim;

using System;
using System.Runtime.InteropServices;
using Godot;

public partial class Main : Node2D
{

	// [DllImport( "user32.dll" )]
	// static extern int GetWindowLong( IntPtr hWnd, int nIndex );

	// [DllImport( "user32.dll", SetLastError = true )]
	// static extern IntPtr SetWindowLongPtr( IntPtr hWnd, int nIndex, IntPtr dwNewLong );

	// const int GWL_EXSTYLE = -20;
	// const int WS_EX_LAYERED = 0x80000;
	// const int WS_EX_TRANSPARENT = 0x20;

	// private bool _clickThroughEnabled;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Vector2I screenTwoSize = DisplayServer.ScreenGetSize( 1 );
		// Vector2I screenTwoOrigin = DisplayServer.ScreenGetPosition( 1 );

		// window.Size = screenTwoSize;
		// window.Position = screenTwoOrigin;

		// _clickThroughEnabled = true;
		// ToggleClickThrough();

		Sprite2D icon = GetNode<Sprite2D>( "Icon" );
		float width = icon.Texture.GetWidth() * icon.Scale.X;
		float height = icon.Texture.GetHeight() * icon.Scale.Y;

		var window = GetWindow();
		window.AlwaysOnTop = true;
		window.Transparent = true;
		window.Borderless = true;
		window.Size = new Vector2I( (int)width, (int)height );


	}

	public override void _Process( double delta )
	{

	}


	// private void ToggleClickThrough()
	// {
	// 	// Get the HWND (Window ID)
	// 	var window = GetWindow();
	// 	long rawHandle = DisplayServer.WindowGetNativeHandle(
	// 		DisplayServer.HandleType.WindowHandle,
	// 		window.GetWindowId()
	// 	);
	// 	IntPtr hwnd = new( rawHandle );

	// 	// Update Window Style based on toggle
	// 	int style = GetWindowLong( hwnd, GWL_EXSTYLE );
	// 	style |= WS_EX_LAYERED;

	// 	if ( !_clickThroughEnabled )
	// 	{
	// 		style |= WS_EX_TRANSPARENT;
	// 	}
	// 	else
	// 	{
	// 		style &= ~WS_EX_TRANSPARENT;
	// 	}
	// 	SetWindowLongPtr( hwnd, GWL_EXSTYLE, style );
	// 	_clickThroughEnabled = !_clickThroughEnabled;
	// }
}
