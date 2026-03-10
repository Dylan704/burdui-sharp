using System.Globalization;
using System.Xml.Serialization;

namespace BurdUI;
    
[Serializable]
public class View
{
    [XmlIgnore]
    public Rectangle Bounds { get; set; }
    
    [XmlArray("Children")]   
    [XmlArrayItem("Button", typeof(Button))]
    [XmlArrayItem("VerticalLayoutPanel", typeof(VerticalLayoutPanel))]

    public List<View> Children { get; internal set; }
    
    
    [XmlAttribute("Bounds")]
    public string BoundsAttr
    {
        get => $"{Bounds.X.ToString(CultureInfo.InvariantCulture)}," +
               $"{Bounds.Y.ToString(CultureInfo.InvariantCulture)}," +
               $"{Bounds.Width.ToString(CultureInfo.InvariantCulture)}," +
               $"{Bounds.Height.ToString(CultureInfo.InvariantCulture)}";
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Bounds = Rectangle.Empty;
                return;
            }

            var parts = value.Split(',');
            if (parts.Length != 4)
                throw new FormatException("Bounds must be 'x,y,width,height'.");

            int x  = int.Parse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture);
            int y  = int.Parse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture);
            int w  = int.Parse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
            int h  = int.Parse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture);

            Bounds = new Rectangle(x, y, w, h);
        }
    }


    public View()
    {
        this.Children = new List<View>();
    }
    

    public virtual void Paint(Graphics g)
    {
        var state = g.Save();
        g.TranslateTransform(this.Bounds.X, this.Bounds.Y);
        foreach (View child in this.Children)
        {
            child.Paint(g);
            
        }
        
        g.Restore(state);
        g.DrawRectangle(Pens.Black, Bounds);
       
    }

    public void AddChild(View child)
    {
        this.Children.Add(child);
    }
}