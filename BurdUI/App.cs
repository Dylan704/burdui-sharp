namespace BurdUI;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Input;


/// <summary>
/// This class represents a BurdUI-based application. It provides to the BurdUI
/// View Tree the raw information coming from the devices and manages the
/// Event loop. In a nutshell, this class contains the logic required to access
/// OS-level information about the screen output and device input.
/// In order to simplify our development, we do not access directly the OS information
/// but we use the Avalonia representation (i.e., we pass through another UI toolkit). 
/// </summary>

public class App : Control
{
    // a window in burdUI has max res 2k 
    private const int MaxWidth = 2560;
    private const int MaxHeight = 1440;
    
    private RenderTargetBitmap _bitmap;
    private int _width = 800;
    private int _height = 600;
    private DrawingContext burdUIContext;

    public View? Root { get; set; }

    public App()
    {
        // init BurdUI graphics context
        var size = new PixelSize(MaxWidth, MaxHeight);
        var dpi = new Vector(192, 192);

        _bitmap = new RenderTargetBitmap(size, dpi);
        burdUIContext = _bitmap.CreateDrawingContext();

        Focusable = true;

        // get raw events from mouse and keyboard
        PointerPressed += (s, e) =>
        {
            var p = e.GetPosition(this);
            OnMouseDown(p.X, p.Y, e);
        };

        PointerMoved += (s, e) =>
        {
            var p = e.GetPosition(this);
            OnMouseMove(p.X, p.Y, e);
        };

        KeyDown += (s, e) =>
        {
            OnKey(e.Key);
        };
        
    }

    // ==========================
    // EVENTI PASSATI A TE
    // ==========================

    protected virtual void OnMouseDown(double x, double y, PointerPressedEventArgs e)
    {
        //TODO: add management
    }

    protected virtual void OnMouseMove(double x, double y, PointerEventArgs e)
    {
        //TODO: add management
    }
    

    protected virtual void OnKey(Key key)
    {
        //TODO: add management
    }


    /// <summary>
    /// Renders the BurdUI applications on an Avalonia Window
    /// </summary>
    /// <param name="ctx"></param>
    public override void Render(DrawingContext ctx)
    {
        base.Render(ctx);
        
        // painting BurdUI
        Root?.Paint(burdUIContext);

       // passing the result to the Avalonia Window
        ctx.DrawImage(
            _bitmap,
            sourceRect: new Rect(0, 0, MaxWidth, MaxHeight),
            destRect: new Rect(0, 0, MaxWidth, MaxHeight));
    }

    
}