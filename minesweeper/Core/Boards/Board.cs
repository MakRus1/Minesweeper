using System;
using System.Linq;
using System.Windows.Forms;

namespace minesweeper.Core.Boards
{
    public class Board
    {
        public Minesweeper Minesweeper { get; set; }

        public BoardPainter Painter { get; set; }

        public int Width { get; set; }                                  // ������
        public int Height { get; set; }                                 // ������

        public int NumMines { get; set; }                               // ���������� ��� �� ����
        public int NumMinesRemaining => NumMines - FlagsPlaced();       // ���������� ���������� ���

        public Cell[,] Cells { get; set; }                              // ������

        public bool GameOver { get; set; }                              // ��������� ����



        public const int CellSize = 32;                                 // ������ ������



        // ����������� ����
        public Board(Minesweeper minesweeper, int width, int height, int mines)
        {
            Minesweeper = minesweeper;
            Width = width;
            Height = height;
            NumMines = mines;
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

            GameOver = false;
        }

        // ����������� ���
        public void PlaceMines()
        {
            var MinesPlaced = 0;
            var random = new Random();

            while (MinesPlaced < NumMines)
            {
                int x = random.Next(0, Width);
                int y = random.Next(0, Height);

                if (!Cells[x, y].IsMine)
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
                PlaceMines();
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
                HandleGameOver(gameWon: true);
            }
        }
    }
}