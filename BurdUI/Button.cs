using System.Runtime.InteropServices.JavaScript;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Media;
using BurdUI.Utils;

namespace BurdUI;

[Serializable]
public class Button : View
{
    
    public Border? Border { get; set; }
    public AlignedText? Text { get; set; }
    


    public Button()
    {
        
    }
    public Button(string label = "")
    {
        this.Text = new AlignedText(label)
        {
            HorizontalAlignment = AlignedText.HorizontalTextAlignment.Center,
            VerticalAlignment = AlignedText.VerticalTextAlignment.Middle
        };
    }
    public override void Paint(DrawingContext g, Rect clip)
    {
        
        
        using (g.PushClip(new Rect(clip.X, clip.Y,clip.Width,clip.Height)))
        {
            var thickBounds = new Rect(
                Bounds.X + Border.StrokeThickness, Bounds.Y + Border.StrokeThickness,
                Bounds.Width - 2.0 * Border.StrokeThickness,
                Bounds.Height - 2.0 * Border.StrokeThickness
            );
            this.Border?.Fill(g, thickBounds);
            this.Text?.Draw(g, this.Bounds);
            this.Border?.Draw(g, thickBounds);
        }
        
    }
}