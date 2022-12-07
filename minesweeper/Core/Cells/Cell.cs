using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using minesweeper.Core.Boards;

namespace minesweeper.Core
{
    public class Cell
    {
        public int XLoc { get; set; }                               // ����� ������ �� X
        public int YLoc { get; set; }                               // ����� ������ �� Y
        public int XPos { get; set; }                               // ������� �� ���� �� X
        public int YPos { get; set; }                               // ������� �� ���� �� Y
        public Point CenterPos { get; set; }                        // ����� ������
        public Point TopLeftPos { get; set; }                       // ������� ����� �����
        public Point BottomLeftPos { get; set; }                    // ������ ����� �����

        public int CellSize { get; set; }                           // ������ ������

        public CellState CellState { get; set; }                    // ��������� ������
        public CellType CellType { get; set; }                      // ��� ������

        public int NumMines { get; set; }                           // ���������� ���

        public Board Board { get; set; }                            // ������� ����

        public Rectangle Bounds { get; private set; }               // ������� ������

        private List<Cell> Surrounding { get; set; }                // ������ �������� ������



        // ������ ���������� �������
        public List<Cell> SurroundingFlagged => GetNeighborCells().Where(cell => cell.Flagged).ToList();

        // ���������� ���������� ���
        public int MinesRemaining => Opened && NumMines > 0 ? (NumMines - SurroundingFlagged.Count) : 0;

        // �������� �� ������ �������
        public bool Flagged => CellType == CellType.Flagged || CellType == CellType.FlaggedMine;

        // ������� �� ������
        public bool Closed => CellState == CellState.Closed;

        // ������� �� ������
        public bool Opened => CellState == CellState.Opened;

        // ���� �� ���� � ���� ������
        public bool IsMine => CellType == CellType.Mine || CellType == CellType.FlaggedMine;



        // ����������� ������
        public Cell(int x, int y, Board board)
        {
            XLoc = x;
            YLoc = y;

            CellSize = Board.CellSize;
            CellState = CellState.Closed;
            CellType = CellType.Regular;
            
            Board = board;

            XPos = XLoc * CellSize;
            YPos = YLoc * CellSize;
            Bounds = new Rectangle(XPos, YPos, CellSize, CellSize);

            CenterPos = new Point(XPos + (CellSize / 2 - 10), YPos + (CellSize / 2 - 10));
            TopLeftPos = new Point(XPos, YPos);
            BottomLeftPos = new Point(XPos, YPos + (CellSize - 10));
        }

        // ������� ������ �������
        public void OnFlag()
        {
            CellType = CellType switch
            {
                CellType.Regular => CellType.Flagged,
                CellType.Mine => CellType.FlaggedMine,
                CellType.Flagged => CellType.Regular,
                CellType.FlaggedMine => CellType.Mine,
                _ => throw new Exception($"Error type {CellType}")
            };

            Board.Minesweeper.UpdateMinesRemaining();
            Board.Minesweeper.Invalidate();
        }

        // ������� �� ������
        public void OnClick(bool recursiveCall = false)
        {
            if (recursiveCall)
            {
                // ����������� �������� ����������� ���� ��� ������ �� ������� ��� ��������� ������ �� ��������
                if (CellType != CellType.Regular || CellState != CellState.Closed)
                {
                    return;
                }
            }

            // ���� � ������ ����
            if (CellType == CellType.Mine)
            {
                CellState = CellState.Opened;
                Board.RevealMines();
                return;
            }

            // ���� ������ �������
            if (CellType == CellType.Regular)
            {
                CellState = CellState.Opened;
            }

            // ����������� ������� ������
            if (NumMines == 0 || MinesRemaining == 0)
            {
                foreach (var n in GetNeighborCells())
                {
                     n.OnClick(true);
                }
            }
        }

        // ��������� ������ �������
        public List<Cell> GetNeighborCells()
        {
            if (Surrounding == null)
            {
                Surrounding = new List<Cell>();

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        // �� ��������� ������� ������
                        if (x == 0 && y == 0)
                        {
                            continue;
                        }

                        // ���� ������ ������� �� ������� ����
                        if (XLoc + x < 0 || XLoc + x >= Board.Width || YLoc + y < 0 || YLoc + y >= Board.Height)
                        {
                            continue;
                        }

                        Surrounding.Add(Board.Cells[XLoc + x, YLoc + y]);
                    }
                }
            }

            return Surrounding;
        }
    }
}