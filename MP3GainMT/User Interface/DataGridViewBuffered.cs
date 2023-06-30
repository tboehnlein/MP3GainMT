using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
