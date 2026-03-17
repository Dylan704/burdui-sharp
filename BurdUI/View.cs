using System.Globalization;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace BurdUI;
    
[Serializable]
public class View
{
    [XmlIgnore]
    public Rect Bounds { get; set; }
    
    [XmlArray("Children")]   
    [XmlArrayItem("Button", typeof(Button))]
    [XmlArrayItem("VerticalLayoutPanel", typeof(VerticalLayoutPanel))]

    public List<View> Children { get; internal set; }

    [XmlIgnore]
    public View? Parent { get; internal set; }
    
    [XmlIgnore]
    public App? App { get; set; }


    [XmlAttribute("Bounds")]
    public string BoundsAttr
    {
        get => $"{Bounds.TopLeft.X.ToString(CultureInfo.InvariantCulture)}," +
               $"{Bounds.TopLeft.Y.ToString(CultureInfo.InvariantCulture)}," +
               $"{Bounds.Width.ToString(CultureInfo.InvariantCulture)}," +
               $"{Bounds.Height.ToString(CultureInfo.InvariantCulture)}";
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Bounds = new Rect();
                return;
            }

            var parts = value.Split(',');
            if (parts.Length != 4)
                throw new FormatException("Bounds must be 'x,y,width,height'.");

            int x  = int.Parse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture);
            int y  = int.Parse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture);
            int w  = int.Parse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
            int h  = int.Parse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture);

            Bounds = new Rect(x, y, w, h);
        }
    }


    public View()
    {
        this.Children = new List<View>();
    }


    public virtual void Paint(DrawingContext ctx)
    {
        Paint(ctx, Bounds);
    }
    
    public virtual void Paint(DrawingContext ctx, Rect clip)
    {
        using (ctx.PushTransform(Matrix.CreateTranslation(Bounds.X, Bounds.Y)))
        {
            var localClip = new Rect(
                clip.X - Bounds.X, clip.Y - Bounds.Y,
                clip.Width, clip.Height
                );
            
            foreach (var child in Children)
            {
                var childClip = child.Bounds.Intersect(localClip);
                if(childClip.Height > 0 && childClip.Width > 0)
                    // the intersection is not empty
                    child.Paint(ctx, childClip);
            }
        }    
        // Draw final border rectangle
        var pen = new Pen(Brushes.Red, 1);    
        ctx.DrawRectangle(pen, Bounds);
       
    }

    public void AddChild(View child)
    {
        child.Parent = this;
        this.Children.Add(child);
    }
    
    public void Invalidate()
    {
        this.Invalidate(new RepaintEvent()
        {
            Source = this, 
            DamagedArea =  Bounds
        });
    }

    public void Invalidate(RepaintEvent evt)
    {
        if (this.Parent == null)
        {
            this.App?.PushEvent(evt);
        }
        else
        {
            var parentDamagedArea = new Rect(
                evt.DamagedArea.X  + Bounds.X,
                evt.DamagedArea.Y  + Bounds.Y,
                evt.DamagedArea.Width,
                evt.DamagedArea.Height);
            evt.DamagedArea = parentDamagedArea;
            this.Parent.Invalidate(evt);
            
        }
    }
}