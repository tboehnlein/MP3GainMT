using System.Windows.Forms;

namespace MP3GainMT.User_Interface
{
    internal class DataGridViewBuffered : DataGridView
    {
        public DataGridViewBuffered()
        {
            DoubleBuffered = true;
        }
    }
}