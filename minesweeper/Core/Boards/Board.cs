using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace minesweeper.Core.Boards
{
    public class Board
    {
        public Minesweeper Minesweeper { get; set; }

        public BoardPainter Painter { get; set; }

        public int Width { get; set; }                                  // ������
        public int Height { get; set; }                                 // ������

        public int NumMines { get; set; }                               // ���������� ��� �� ����

        public GameMode Mode { get; set; }
        public int NumMinesRemaining => NumMines - FlagsPlaced();       // ���������� ���������� ���

        public Cell[,] Cells { get; set; }                              // ������

        public bool GameOver { get; set; }                              // ��������� ����

        public int Time { get; set; }                                   // ����� ����

        public bool IsFirstStep { get; set; }                           // ������ �� ���?



        public const int CellSize = 32;                                 // ������ ������



        // ����������� ����
        public Board(Minesweeper minesweeper, int width, int height, int mines, GameMode mode)
        {
            Minesweeper = minesweeper;
            Width = width;
            Height = height;
            NumMines = mines;
            Mode = mode;
            Cells = new Cell[width, height];
            Painter = new BoardPainter { Board = this };
        }

        // ���������� ����
        public void SetupBoard()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Cells[x, y] = new Cell(x, y, this);
                }
            }
            Time = 0;
            IsFirstStep = true;
            GameOver = false;
        }

        // ����������� ���
        public void PlaceMines(int xcur = -1, int ycur = -1)
        {
            var MinesPlaced = 0;
            var random = new Random();

            while (MinesPlaced < NumMines)
            {
                int x = random.Next(0, Width);
                int y = random.Next(0, Height);

                if (!Cells[x, y].IsMine && x != xcur && y != ycur)
                {
                    Cells[x, y].CellType = CellType.Mine;
                    MinesPlaced++;
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var c = Cells[x, y];
                    c.NumMines = c.GetNeighborCells().Where(n => n.IsMine).Count();
                }
            }

            Minesweeper.Invalidate();
        }

        // ��������. �������� ���� � ������������� ����
        public void RevealMines()
        {
            
            // �������� ����
            GameOver = true;
            Minesweeper.Invalidate();

            // ������ ������� �����
            HandleGameOver(gameWon: false);
        }

        // ��������� ���� � �� ����������
        private void HandleGameOver(bool gameWon)
        {
            var message = gameWon ? "����������! �� ��������!" : "� ���������, �� ���������...";
            message += "\n������ ������� ���?";

            var response = MessageBox.Show(message, "���� ��������", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (response == DialogResult.Yes)
            {
                SetupBoard();
                //PlaceMines();
            }
        }

        // ���������� �����, ���������� ��������
        public int FlagsPlaced()
        {
            int totalFlagged = 0;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Cells[x, y].Flagged)
                    {
                        totalFlagged++;
                    }
                }
            }

            return totalFlagged;
        }

        // �������� ������
        public void CheckForWin()
        {
            var correctMines = 0;           // ��������� ��������� ����
            var incorrectMines = 0;         // ����������� ��������� ����
            var remainingCells = 0;         // ���������� ������

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var c = Cells[x, y];

                    // ���� �� ������ ��� ���� ����� ������
                    if (c.CellType == CellType.Flagged)
                    {
                        incorrectMines++;
                    }

                    // ���� �� ������ � ����� ����� ������
                    if (c.CellType == CellType.FlaggedMine)
                    {
                        correctMines++;
                    }

                    // ���� ������ �������
                    if (c.Closed)
                    {
                        remainingCells++;
                    }
                }
            }

            // ��� ���� ��������
            bool flaggedAllMines = correctMines == NumMines && incorrectMines == 0;
            // ��� ������ ������ - ����
            bool onlyCellsLeftAreMines = remainingCells == NumMines;

            // ������
            if (flaggedAllMines || onlyCellsLeftAreMines)
            {
                GameOver = true;
                Input input_name = new Input();
                input_name.ShowDialog();
                string name = input_name.NAME;

                string filename = "../../../res/Records_";
                if (Mode == GameMode.Beginner) filename += "beginner.bin";
                else if (Mode == GameMode.Intermediate) filename += "intermediate.bin";
                else if (Mode == GameMode.Expert) filename += "expert.bin";

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Append)))
                {
                    
                    bw.Write(name);
                    bw.Write(Time);
                }
                HandleGameOver(gameWon: true);
            }
        }
    }
}