using System;
using System.Drawing;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BurdUI.Utils
{
    public class AlignedText : IXmlSerializable
    {
        public enum VerticalTextAlignment
        {
            Top,
            Middle,
            Bottom
        }

        public enum HorizontalTextAlignment
        {
            Left,
            Center,
            Right
        }

        [XmlAttribute]
        public string Value { get; set; } = "";

        [XmlAttribute]
        public VerticalTextAlignment VerticalAlignment { get; set; } = VerticalTextAlignment.Top;

        [XmlAttribute]
        public HorizontalTextAlignment HorizontalAlignment { get; set; } = HorizontalTextAlignment.Left;

        public Color Color { get; set; } = Color.Black;

        [XmlIgnore]
        public Font Font { get; set; } = SystemFonts.DefaultFont;

        public AlignedText() { }

        public AlignedText(string value)
        {
            Value = value;
        }

        public AlignedText(
            string value, Font font, Color color,
            HorizontalTextAlignment hAlign = HorizontalTextAlignment.Left,
            VerticalTextAlignment vAlign = VerticalTextAlignment.Top)
        {
            Value = value;
            Font = font;
            Color = color;
            HorizontalAlignment = hAlign;
            VerticalAlignment = vAlign;
        }

        /// <summary>
        /// Draws the aligned text inside the given rectangle.
        /// </summary>
        public void Draw(Graphics g, Rectangle bounds)
        {
            using (StringFormat sf = new StringFormat())
            {
                // Horizontal alignment
                switch (HorizontalAlignment)
                {
                    case HorizontalTextAlignment.Left:
                        sf.Alignment = StringAlignment.Near;
                        break;
                    case HorizontalTextAlignment.Center:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case HorizontalTextAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }

                // Vertical alignment
                switch (VerticalAlignment)
                {
                    case VerticalTextAlignment.Top:
                        sf.LineAlignment = StringAlignment.Near;
                        break;
                    case VerticalTextAlignment.Middle:
                        sf.LineAlignment = StringAlignment.Center;
                        break;
                    case VerticalTextAlignment.Bottom:
                        sf.LineAlignment = StringAlignment.Far;
                        break;
                }

                sf.FormatFlags = StringFormatFlags.NoClip;

                using (Brush brush = new SolidBrush(Color))
                {
                    g.DrawString(Value, Font, brush, bounds, sf);
                }
            }
        }

        // ------------------------------
        // IXmlSerializable implementation
        // ------------------------------

        public XmlSchema? GetSchema() => null;

        /// <summary>
        /// Writes this object as attributes:
        /// vertical, horizontal, value, color (#RRGGBB), font (FontConverter invariant).
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("vertical", VerticalAlignment.ToString());
            writer.WriteAttributeString("horizontal", HorizontalAlignment.ToString());
            writer.WriteAttributeString("value", Value ?? string.Empty);
            writer.WriteAttributeString("color", ColorToHexRgb(Color));

            // Use invariant string for reliable round-tripping regardless of locale.
            var fc = new FontConverter();
            string fontStr = fc.ConvertToInvariantString(Font) ?? string.Empty;
            writer.WriteAttributeString("font", fontStr);
        }

        /// <summary>
        /// Reads attributes: vertical, horizontal, value, color (#RRGGBB or #AARRGGBB), font.
        /// Missing or invalid values fall back to sensible defaults.
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToAttribute("vertical"))
            {
                if (Enum.TryParse(reader.Value, true, out VerticalTextAlignment v))
                    VerticalAlignment = v;
            }

            if (reader.MoveToAttribute("horizontal"))
            {
                if (Enum.TryParse(reader.Value, true, out HorizontalTextAlignment h))
                    HorizontalAlignment = h;
            }

            if (reader.MoveToAttribute("value"))
            {
                Value = reader.Value ?? string.Empty;
            }

            if (reader.MoveToAttribute("color"))
            {
                var parsed = TryParseHexColor(reader.Value, out var c);
                Color = parsed ? c : Color.Black;
            }

            if (reader.MoveToAttribute("font"))
            {
                try
                {
                    var fc = new FontConverter();
                    var parsed = fc.ConvertFromInvariantString(reader.Value);
                    if (parsed is Font f) Font = f;
                }
                catch
                {
                    Font = SystemFonts.DefaultFont;
                }
            }

            // Move back to the element and consume it properly.
            reader.MoveToElement();

            if (reader.IsEmptyElement)
            {
                reader.Read(); // <AlignedText ... />
            }
            else
            {
                // <AlignedText ...> ... </AlignedText>
                reader.ReadStartElement();
                // No inner content expected; skip anything unexpected safely
                while (reader.NodeType != XmlNodeType.EndElement && !reader.EOF)
                {
                    reader.Skip();
                }
                if (!reader.EOF) reader.ReadEndElement();
            }
        }

        // ------------------------------
        // Helpers
        // ------------------------------

        /// <summary>
        /// Returns #RRGGBB (no alpha) uppercase.
        /// </summary>
        public static string ColorToHexRgb(Color color) =>
            $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        /// <summary>
        /// Parses #RRGGBB or #AARRGGBB. If alpha present, it will be applied.
        /// Returns false on invalid input.
        /// </summary>
        public static bool TryParseHexColor(string? text, out Color color)
        {
            color = Color.Black;
            if (string.IsNullOrWhiteSpace(text)) return false;

            var t = text.Trim();
            if (t.StartsWith("#", StringComparison.Ordinal)) t = t.Substring(1);

            if (t.Length == 6)
            {
                if (byte.TryParse(t.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r) &&
                    byte.TryParse(t.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) &&
                    byte.TryParse(t.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
                {
                    color = Color.FromArgb(r, g, b);
                    return true;
                }
            }
            else if (t.Length == 8)
            {
                if (byte.TryParse(t.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var a) &&
                    byte.TryParse(t.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r) &&
                    byte.TryParse(t.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) &&
                    byte.TryParse(t.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
                {
                    color = Color.FromArgb(a, r, g, b);
                    return true;
                }
            }

            return false;
        }
    }
}