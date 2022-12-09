using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minesweeper.Core.Boards
{
    public partial class Input : Form
    {
        public string NAME { get; set; }
        public Input()
        {
            InitializeComponent();
            NAME = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NAME = textBox1.Text;
            Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
    }
}
