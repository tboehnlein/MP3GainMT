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
