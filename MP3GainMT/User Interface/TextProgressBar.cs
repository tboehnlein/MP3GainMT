// Copyright (c) 2025 Thomas Boehnlein
// 
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
// 
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MP3GainMT.User_Interface
{
    public class TextProgressBar : ProgressBar
    {
        [Description("The text to display in the center of the progress bar."), Category("Appearance"), DefaultValue("")]
        public string CustomText { get; set; }

        [Description("The text to display when the progress is 100% complete."), Category("Appearance"), DefaultValue("")]
        public string FinishedText { get; set; }

        public TextProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;
            Graphics g = e.Graphics;

            // Draw the background
            ProgressBarRenderer.DrawHorizontalBar(g, rect);

            // Draw the progress
            if (Value > 0)
            {
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((double)Value / Maximum) * rect.Width), rect.Height);
                ProgressBarRenderer.DrawHorizontalChunks(g, clip);
            }

            int percent = (int)(((double)this.Value / this.Maximum) * 100);
            string text;

            if (!string.IsNullOrEmpty(this.CustomText))
            {
                text = this.CustomText;
            }
            else if (this.Value == this.Maximum && !string.IsNullOrEmpty(this.FinishedText))
            {
                text = this.FinishedText;
            }
            else
            {
                text = $"{percent}%";
            }

            // Draw the text
            TextRenderer.DrawText(g, text, Font, rect, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        }
    }
}