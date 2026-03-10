using System;
using System.Collections.Generic;
using System.Drawing;

namespace BurdUI
{
    public enum VerticalStackOrigin
    {
        Top,
        Bottom
    }

    [Serializable]
    public class VerticalLayoutPanel : View
    {
        /// <summary>
        /// Whether children are laid out from the top or from the bottom.
        /// </summary>
        public VerticalStackOrigin Origin { get; set; } = VerticalStackOrigin.Top;

        /// <summary>
        /// Vertical spacing (in pixels) between consecutive children.
        /// </summary>
        public int Spacing { get; set; } = 0;

        /// <summary>
        /// Performs the layout of children based on panel Bounds and Origin.
        /// Children are sized to fill the panel width, and their height is taken from their current Bounds.Height.
        /// Child Bounds are assigned in the panel's local coordinates (relative to this view),
        /// since View.Paint translates by this.Bounds before painting children.
        /// </summary>
        private void LayoutChildren()
        {
            int innerWidth = Math.Max(0, this.Bounds.Width);

            if (Origin == VerticalStackOrigin.Top)
            {
                int y = 0;
                foreach (var child in this.Children)
                {
                    int h = Math.Max(0, child.Bounds.Height);
                    child.Bounds = new Rectangle(0, y, innerWidth, h);
                    y += h + Spacing;
                }
            }
            else // VerticalStackOrigin.Bottom
            {
                int y = this.Bounds.Height;
                foreach (var child in this.Children)
                {
                    int h = Math.Max(0, child.Bounds.Height);
                    y -= h; // reserve space for the child
                    child.Bounds = new Rectangle(0, y, innerWidth, h);
                    y -= Spacing;
                }
            }
        }

        /// <summary>
        /// Lays out children, then defers to base to paint them.
        /// </summary>
        public override void Paint(Graphics g)
        {
            // Compute child bounds before drawing
            LayoutChildren();

            // base.Paint will translate by this.Bounds and then paint children using their relative Bounds
            base.Paint(g);
        }
    }
}