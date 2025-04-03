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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MP3GainMT.User_Interface
{
    public class ReadOnlyCheckBox : CheckBox
    {
        private bool _readOnly = false;
        private Color _readOnlyForeColor = Color.Gray;
        private Color _normalForeColor = Color.Black;

        [System.ComponentModel.Category("Behavior")]
        [System.ComponentModel.DefaultValue(false)]
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                if (_readOnly != value)
                {
                    _readOnly = value;
                    UpdateForColor();
                }
            }
        }

        [System.ComponentModel.Category("Appearance")]
        [System.ComponentModel.DefaultValue(typeof(Color), "Black")]
        public Color NormalForeColor
        {
            get => _normalForeColor;
            set
            {
                if (_normalForeColor != value)
                {
                    _normalForeColor = value;
                    UpdateForColor();
                }
            }
        }

        [System.ComponentModel.Category("Appearance")]
        [System.ComponentModel.DefaultValue(typeof(Color), "Gray")]
        public Color ReadOnlyForeColor
        {
            get => _readOnlyForeColor;
            set
            {
                if (_readOnlyForeColor != value)
                {
                    _readOnlyForeColor = value;
                    UpdateForColor();
                }
            }
        }

        // Hide ForeColor from the editor
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(
            System.ComponentModel.EditorBrowsableState.Never)]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        public ReadOnlyCheckBox()
        {
            UpdateForColor();
        }

        private void UpdateForColor()
        {
            ForeColor = ReadOnly ? ReadOnlyForeColor : NormalForeColor;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            // Disable highlight when the cursor is over the CheckBox
            if (!ReadOnly) base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Disable reacting (logically or visibly) to a mouse click
            if (!ReadOnly) base.OnMouseDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Suppress space key to disable checking/unchecking 
            if (!ReadOnly || e.KeyData != Keys.Space) base.OnKeyDown(e);
        }
    }

}
