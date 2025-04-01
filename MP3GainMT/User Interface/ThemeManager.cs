using System;
using System.Drawing;
using System.Windows.Forms;

namespace MP3GainMT
{
    public static class ThemeManager
    {
        public static Color LightBackColor = Color.FromKnownColor(KnownColor.Control);
        public static Color LightForeColor = Color.Black;
        public static Color LightButtonColor = Color.FromKnownColor(KnownColor.ControlLight);
        public static Color LightInputColor = Color.FromKnownColor(KnownColor.ControlLightLight);
        public static Color DarkBackColor = Color.FromArgb(45, 45, 48);
        public static Color DarkForeColor = Color.FromArgb(225, 225, 225);
        public static Color DarkButtonColor = Color.FromKnownColor(KnownColor.ControlDarkDark);
        public static Color DarkInputColor = Color.FromArgb(10,10, 10);

        public static Color RowAltColor = Color.FromArgb(215, 215, 255);
        public static Color RowColor = Color.FromArgb(245, 245, 245);
        public static Color OddRowAltColor = Color.FromArgb(230, 230, 255);
        public static Color OddRowColor = Color.White;

        public static Color LightRowAltColor = Color.FromArgb(215, 215, 255);
        public static Color LightRowColor = Color.FromArgb(235, 235, 235);
        public static Color LightOddRowAltColor = Color.FromArgb(230, 230, 255);
        public static Color LightOddRowColor = Color.White;

        public static Color DarkRowAltColor = Color.FromArgb(25, 25, 85);
        public static Color DarkRowColor = Color.FromArgb(45, 45, 45);
        public static Color DarkOddRowAltColor = Color.FromArgb(30, 30, 55);
        public static Color DarkOddRowColor = Color.FromArgb(25, 25, 27);

        public static Color DarkGridColor = Color.FromArgb(10, 10, 10);
        public static Color LightGridColor = Color.FromKnownColor(KnownColor.ControlLight);

        public static Color DarkTableColor = Color.FromArgb(10, 10, 10);
        public static Color LightTableColor = Color.FromKnownColor(KnownColor.AppWorkspace);

        public static Color DarkHeaderSelectColor = Color.FromArgb(10, 10, 10);
        public static Color LightHeaderSelectColor = Color.FromKnownColor(KnownColor.ControlLight);

        public static Color LightInstructionText = Color.FromArgb(255, 255, 255);
        public static Color DarkInstructionText = Color.FromArgb(0, 0, 0);

        public static Color LightInstructionBackColor = Color.FromArgb(32, 32, 32);
        public static Color DarkInstructionBackColor = Color.FromArgb(255, 255, 255);

        public static Color GridColor = LightGridColor;
        public static Color TableColor = LightTableColor;
        public static Color HeaderSelectColor = LightHeaderSelectColor;
        public static Color InstructionText = LightInstructionText;
        public static Color InstructionBackColor = LightInstructionBackColor;

        public static void ApplyTheme(Form form, bool isDarkTheme)
        {
            Color backColor = isDarkTheme ? DarkBackColor : LightBackColor;
            Color foreColor = isDarkTheme ? DarkForeColor : LightForeColor;
            Color buttonColor = isDarkTheme ? DarkButtonColor : LightButtonColor;
            Color inputColor = isDarkTheme ? DarkInputColor : LightInputColor;

            RowAltColor = isDarkTheme ? DarkRowAltColor : LightRowAltColor;
            RowColor = isDarkTheme ? DarkRowColor : LightRowColor;
            OddRowAltColor = isDarkTheme ? DarkOddRowAltColor : LightOddRowAltColor;
            OddRowColor = isDarkTheme ? DarkOddRowColor : LightOddRowColor;
            GridColor = isDarkTheme ? DarkGridColor : LightGridColor;
            TableColor = isDarkTheme ? DarkTableColor : LightTableColor;
            HeaderSelectColor = isDarkTheme ? DarkHeaderSelectColor : LightHeaderSelectColor;
            InstructionText = isDarkTheme ? DarkInstructionText : LightInstructionText;
            InstructionBackColor = isDarkTheme ? DarkInstructionBackColor : LightInstructionBackColor;


            form.BackColor = backColor;
            form.ForeColor = foreColor;

            ApplyThemeToControls(form.Controls, backColor, foreColor, buttonColor, inputColor);
        }

        private static void ApplyThemeToControls(Control.ControlCollection controls, Color backColor, Color foreColor, Color buttonColor, Color inputColor)
        {
            foreach (Control control in controls)
            {
                Console.WriteLine($"{control.Parent.ToString()} : {control.ToString()}");

                if (control is DataGridView dataGridView)
                {
                    dataGridView.ForeColor = foreColor;
                    dataGridView.ColumnHeadersDefaultCellStyle.BackColor = backColor;
                    dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = foreColor;
                    dataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderSelectColor;
                    dataGridView.RowHeadersDefaultCellStyle.BackColor = backColor;
                    dataGridView.RowHeadersDefaultCellStyle.ForeColor = foreColor;
                    dataGridView.RowHeadersDefaultCellStyle.SelectionBackColor = HeaderSelectColor;
                    dataGridView.RowsDefaultCellStyle.BackColor = backColor;
                    dataGridView.RowsDefaultCellStyle.ForeColor = foreColor;
                    dataGridView.GridColor = GridColor;
                    dataGridView.BackgroundColor = TableColor;
                }
                else if (control is Panel panel && panel.Tag.ToString() == "Instruction")
                {
                    panel.BackColor = InstructionBackColor;
                    panel.ForeColor = InstructionText;

                    if (control.HasChildren)
                    {
                        ApplyInvertThemeToControls(panel.Controls, InstructionBackColor, InstructionText, buttonColor, inputColor);
                    }
                }
                else if (control is NumericUpDown numeric)
                {
                    numeric.BackColor = inputColor;
                    numeric.ForeColor = foreColor;
                }
                else if (control is TextBox textBox)
                {
                    textBox.BackColor = inputColor;
                    textBox.ForeColor = foreColor;
                }
                else if (control is Button button)
                {
                    button.BackColor = buttonColor;
                    button.ForeColor = foreColor;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = Color.LightGray;
                }
                else
                {
                    control.BackColor = backColor;
                    control.ForeColor = foreColor;

                    if (control.HasChildren)
                    {
                        ApplyThemeToControls(control.Controls, backColor, foreColor, buttonColor, inputColor);
                    }
                }

                
            }
        }

        private static void ApplyInvertThemeToControls(Control.ControlCollection controls, Color lightBackColor, Color lightForeColor, Color buttonColor, Color inputColor)
        {
            foreach (Control control in controls)
            {
                if (control.BackColor != control.Parent.BackColor)
                {
                    control.BackColor = lightBackColor;
                }

                control.ForeColor = lightForeColor;

                ApplyInvertThemeToControls(control.Controls, lightBackColor, lightForeColor, buttonColor, inputColor);
            }
        }
    }
}