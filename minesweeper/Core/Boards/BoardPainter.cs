using System.Collections.Generic;
using System.Drawing;

namespace minesweeper.Core.Boards
{
    public class BoardPainter
    {
        public Board Board { get; set; }                                                // ����

        private Dictionary<int, SolidBrush> _cellColours;                               // �����

        private readonly Font _textFont = new Font("Veranda", 16f, FontStyle.Bold);     // �������� �����



        // ����������� ��������� ����
        public BoardPainter()
        {
            SetupColours();
        }

        // ��������� ������
        private void SetupColours()
        {
            if (_cellColours == null)
            {
                _cellColours = new Dictionary<int, SolidBrush>
                {
                    { 0, new SolidBrush(ColorTranslator.FromHtml("0xffffff")) },        // �����
                    { 1, new SolidBrush(ColorTranslator.FromHtml("0x0000FE")) },        // �����
                    { 2, new SolidBrush(ColorTranslator.FromHtml("0x186900")) },        // �������
                    { 3, new SolidBrush(ColorTranslator.FromHtml("0xAE0107")) },        // �������
                    { 4, new SolidBrush(ColorTranslator.FromHtml("0x000177")) },        // �����-�����
                    { 5, new SolidBrush(ColorTranslator.FromHtml("0x8D0107")) },        // �����-�������
                    { 6, new SolidBrush(ColorTranslator.FromHtml("0x007A7C")) },        // ���������
                    { 7, new SolidBrush(ColorTranslator.FromHtml("0x902E90")) },        // ����������
                    { 8, new SolidBrush(ColorTranslator.FromHtml("0x000000")) },        // ������
                };
            }
        }

        // ��������� ����
        public void Paint(Graphics graphics)
        {
            graphics.Clear(Color.White);
            graphics.TranslateTransform(Board.CellSize / 2, Board.CellSize * 2);

            for (int x = 0; x < Board.Width; x++)
            {
                for (int y = 0; y < Board.Height; y++)
                {
                    var cell = Board.Cells[x, y];
                    DrawInsideCell(cell, graphics);
                    graphics.DrawRectangle(Pens.DimGray, cell.Bounds);
                }
            }
        }

        // ��������� ������� �����
        private Brush GetBackgroundBrush(Cell cell)
        {
            if (cell.Opened)
                return Brushes.LightGray;

            return Brushes.DarkGray;
        }

        // ��������� ����� ������ �� ����
        private void DrawInsideCell(Cell cell, Graphics graphics)
        {
            // �������� ������
            if (cell.Closed)
            {
                graphics.FillRectangle(GetBackgroundBrush(cell), cell.Bounds);
            }

            // �������� ������
            if (cell.Opened)
            {
                graphics.FillRectangle(GetBackgroundBrush(cell), cell.Bounds);

                if (cell.NumMines > 0)
                {
                    graphics.DrawString(cell.NumMines.ToString(), _textFont, GetCellColour(cell), cell.CenterPos);
                }
            }

            // ������ � �������
            if (cell.Flagged)
            {
                graphics.DrawString("?", _textFont, Brushes.Black, cell.CenterPos);
            }

            // ������ � �����
            if (cell.IsMine && Board.GameOver)
            {
                // ����������� ������
                if (cell.Opened)
                {
                    graphics.FillRectangle(Brushes.Red, cell.Bounds);
                }

                // ����, ������� �� ���� ��������
                if (!cell.Flagged)
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    graphics.DrawString("�", _textFont, Brushes.Black, cell.Bounds, format);
                }
            }
        }

        // ����, ��������������� ���������� ���
        private SolidBrush GetCellColour(Cell cell)
        {
            return _cellColours[cell.NumMines];
        }
    }
}