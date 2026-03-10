using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BurdUI.Utils
{
    public class Border : IXmlSerializable
    {
        public Color Color { get; set; } = Color.Black;

        public Color BackgroudColor { get; set; } = Color.White;

        [XmlAttribute]
        public float StrokeThickness { get; set; } = 2f;

        [XmlAttribute]
        public int CornerRadius { get; set; } = 10;

        public Border() { }

        public Border(Color color, Color background, float thickness, int cornerRadius = 10)
        {
            Color = color;
            BackgroudColor =  background;
            StrokeThickness = thickness;
            CornerRadius = cornerRadius;
        }

        /// <summary>
        /// Draws a rounded rectangle border on the provided graphics context.
        /// </summary>
        public void Draw(Graphics g, Rectangle rect)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, CornerRadius))
            using (Pen pen = new Pen(Color, StrokeThickness))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(pen, path);
            }
        }

        public void Fill(Graphics g, Rectangle rect)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, CornerRadius))
            using (Brush brush = new SolidBrush(BackgroudColor))
            {
                g.FillPath(brush, path);
            }
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int d = radius * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }

        // ------------------------------
        // IXmlSerializable implementation
        // ------------------------------

        public XmlSchema? GetSchema() => null;

        /// <summary>
        /// Writes attributes: color (#RRGGBB), strokeThickness (float), cornerRadius (int).
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("color", AlignedText.ColorToHexRgb(Color));
            writer.WriteAttributeString("background", AlignedText.ColorToHexRgb(BackgroudColor));
            writer.WriteAttributeString("strokeThickness", StrokeThickness.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("cornerRadius", CornerRadius.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Reads attributes and falls back to defaults if missing/invalid.
        /// Expects: color (#RRGGBB or #AARRGGBB), strokeThickness (float), cornerRadius (int).
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToAttribute("color"))
            {
                if (AlignedText.TryParseHexColor(reader.Value, out var c))
                    Color = c;
                else
                    Color = Color.Black;
            }
            
            if (reader.MoveToAttribute("background"))
            {
                if (AlignedText.TryParseHexColor(reader.Value, out var c))
                    BackgroudColor = c;
                else
                    BackgroudColor = Color.Black;
            }

            if (reader.MoveToAttribute("strokeThickness"))
            {
                if (float.TryParse(reader.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var t))
                    StrokeThickness = t;
            }

            if (reader.MoveToAttribute("cornerRadius"))
            {
                if (int.TryParse(reader.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var r))
                    CornerRadius = r;
            }

            // Return to the element and consume it
            reader.MoveToElement();

            if (reader.IsEmptyElement)
            {
                // <Border ... />
                reader.Read();
            }
            else
            {
                // <Border ...> ... </Border> — no inner content expected; skip safely
                reader.ReadStartElement();
                while (reader.NodeType != XmlNodeType.EndElement && !reader.EOF)
                    reader.Skip();
                if (!reader.EOF) reader.ReadEndElement();
            }
        }
        
    }
}