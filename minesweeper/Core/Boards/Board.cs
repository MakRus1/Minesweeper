namespace minesweeper.Core.Boards
{
    public class Board
    {
        public Minesweeper Minesweeper { get; set; }

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
        }

        // 
    }
}