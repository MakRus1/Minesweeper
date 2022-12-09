using minesweeper.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minesweeper
{
    public partial class Records : Form
    {
        public GameMode Mode { get; set; }
        public Records()
        {
            InitializeComponent();
            Mode = GameMode.Intermediate;
            label1.Text = "Таблица рекордов Средний";
            SetData();
        }

        public void SetData()
        {
            dataGridView1.Rows.Clear();
            List<string[]> data = FromFile();
            for (int i = 0; i < data.Count; i++)
            {
                dataGridView1.Rows.Add(data[i]);
            }
        }

        public List<string[]> FromFile()
        {
            string filename = "../../../res/Records_";
            if (Mode == GameMode.Beginner) filename += "beginner.bin";
            else if (Mode == GameMode.Intermediate) filename += "intermediate.bin";
            else if (Mode == GameMode.Expert) filename += "expert.bin";
            List<string[]> data = new List<string[]>();
            using (BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                if (br.PeekChar() != -1)
                {
                    for (int i = 0; i < 10 && br.PeekChar() != -1; i++)
                    {
                        data.Add(new string[3]);
                        data[i][0] = br.ReadString();
                        data[i][1] = Convert.ToString(br.ReadInt32());
                    }
                }
            }

            data.Sort(delegate (string[] x, string[] y) { return Convert.ToInt32(x[1]) > Convert.ToInt32(y[1]) ? 1 : -1; });

            return data;
        }

        private void beginnerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode = GameMode.Beginner;
            SetData();
            label1.Text = "Таблица рекордов Новичок";
        }

        private void intermediateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode = GameMode.Intermediate;
            SetData();
            label1.Text = "Таблица рекордов Средний";
        }

        private void expertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode = GameMode.Expert;
            SetData();
            label1.Text = "Таблица рекордов Профессионал";
        }
    }
}
