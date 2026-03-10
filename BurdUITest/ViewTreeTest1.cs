using System.Xml.Serialization;
using BurdUI.Utils;

namespace BurdUITest;
using BurdUI;

/// <summary>
/// First test for the BurdUI package
/// </summary>
public partial class ViewTreeTest1 : Form
{
    public ViewTreeTest1()
    {
        InitializeComponent();
        
        this.Text = "Burdui WinForms Demo"; this.ClientSize = new Size(900, 600);
        var host = new App{ Dock = DockStyle.Fill };
        host.Root = BuildTree2();
        this.Controls.Add(host);
    }

    private View BuildTree()
    {
        // write the code building the view tree here...
        var container = new View();
        container.Bounds = new Rectangle(10, 10, 300, 300);
        var child1 = new View();
        child1.Bounds = new Rectangle(25, 10, 100, 50);
        
        var child2 = new View();
        child2.Bounds = new Rectangle(150, 10, 100, 50);
        
        container.Children.Add(child1);
        container.Children.Add(child2);
        return container;
    }
    
    private View BuildTree2()
    {
        // write the code building the view tree here...
        var container = new View();
        container.Bounds = new Rectangle(10, 10, 300, 300);
        var child1 = new Button("Hello World");
        child1.Bounds = new Rectangle(25, 10, 100, 50);
        child1.Border = new Border(Color.CornflowerBlue, Color.White, 3, 5);
        
        
        var child2 = new Button("Click me!");
        child2.Bounds = new Rectangle(150, 10, 100, 50);
        child2.Border = new Border(Color.BlueViolet, Color.White, 3, 5);
        
        container.Children.Add(child1);
        container.Children.Add(child2);
        return container;
    }

    private View BuildTree3()
    {
        // write the code building the view tree here...
        var container = new VerticalLayoutPanel();
        container.Origin = VerticalStackOrigin.Bottom;
        container.Spacing = 10;
        container.Bounds = new Rectangle(10, 10, 300, 300);
        var child1 = new Button("Hello World");
        child1.Bounds = new Rectangle(25, 10, 100, 50);
        child1.Border = new Border(Color.CornflowerBlue, Color.White, 3, 5);
        
        
        var child2 = new Button("Click me!");
        child2.Bounds = new Rectangle(150, 10, 100, 50);
        child2.Border = new Border(Color.BlueViolet, Color.White, 3, 5);
        
        container.Children.Add(child1);
        container.Children.Add(child2);
        
        var serializer = new XmlSerializer(typeof(VerticalLayoutPanel));
        
        using (var file = File.Create("ui.xml"))
        {
            serializer.Serialize(file, container);
        }

        return container;
    }
    
    
}