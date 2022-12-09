using minesweeper.Core;
using minesweeper.Core.Boards;
using System;
using System.Windows.Forms;

namespace minesweeper
{
    public partial class Minesweeper : Form
    {
        public Board _board { get; private set; }

        public Random _random { get; private set; }

        public GameMode _mode { get; private set; }

        public int BOARD_WIDTH { get; private set; }
        public int BOARD_HEIGHT { get; private set; }

        public int NUM_MINES { get; private set; }

        public Records Records { get; private set; }



        // ����������� ����
        public Minesweeper()
        {
            InitializeComponent();
            DoubleBuffered = true;

            _random = new Random();

            _mode = GameMode.Intermediate;

            //Records = new Records();

            StartGame();
        }

        // ������ ����
        private void StartGame()
        {
            GetBoardSize();

            _board = new Board(this, BOARD_WIDTH, BOARD_HEIGHT, NUM_MINES, _mode);
            _board.SetupBoard();
            //_board.PlaceMines();

            Width = (BOARD_WIDTH * Board.CellSize) + (int)(Board.CellSize * 1.5);
            Height = (BOARD_HEIGHT * Board.CellSize) + Board.CellSize * 4;

            UpdateMinesRemaining();
            UpdateTimer();
        }

        // ���������� �������� ��� �� ������
        public void UpdateMinesRemaining()
        {
            lblMinesLeft.Text = $"�������� ���: {_board.NumMinesRemaining}";
        }

        // ���������� ������� �� ������
        public void UpdateTimer()
        {
            lblTimer.Text = $"�����: {_board.Time}";
        }

        // ��������� ������ � ������
        private void GetBoardSize()
        {
            switch (_mode)
            {
                case GameMode.Beginner:
                    BOARD_WIDTH = 8;
                    BOARD_HEIGHT = 8;
                    NUM_MINES = 10;
                    break;

                case GameMode.Intermediate:
                    BOARD_WIDTH = 16;
                    BOARD_HEIGHT = 16;
                    NUM_MINES = 40;
                    break;

                case GameMode.Expert:
                    BOARD_WIDTH = 30;
                    BOARD_HEIGHT = 16;
                    NUM_MINES = 99;
                    break;
            };
        }

        // ��������� ����� �� ������
        private Cell GetCellFromMouseEvent(MouseEventArgs mouseArgs)
        {
            if (_board != null && _board.GameOver)
            {
                return null;
            }

            var clickedX = mouseArgs.X - (Board.CellSize / 2);
            var clickedY = mouseArgs.Y - (Board.CellSize * 2);

            var cellX = clickedX / Board.CellSize;
            var cellY = clickedY / Board.CellSize;

            if (clickedX < 0 || clickedY < 0 || cellX < 0 || cellY < 0 || cellX >= _board.Width || cellY >= _board.Height)
            {
                return null;
            }

            return _board.Cells[cellX, cellY];
        }

        // ������� �� �������
        private void AfterClick()
        {
            _board.CheckForWin();

            Invalidate();
        }

        // ��������� �������� ����
        private void Minesweeper_Paint(object sender, PaintEventArgs e)
        {
            if (_board != null)
            {
                _board.Painter?.Paint(e.Graphics);
            }
        }

        // �������� ����
        private void Minesweeper_MouseMove(object sender, MouseEventArgs e)
        {
            var cell = GetCellFromMouseEvent(e);
            Cursor = cell != null && cell.Closed ? Cursors.Hand : Cursors.Default;

            Invalidate();
        }

        // �����
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var response = MessageBox.Show("������ ����� �� ����?", "�����?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (response == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // ��������� ������
        private void Minesweeper_Click(object sender, EventArgs e)
        {
            var mouseArgs = (MouseEventArgs)e;

            var cell = GetCellFromMouseEvent(mouseArgs);
            if (cell == null)
            {
                return;
            }

            switch (mouseArgs.Button)
            {
                case MouseButtons.Left:
                    if (_board.IsFirstStep)
                    {
                        _board.PlaceMines(cell.XLoc, cell.YLoc);
                        timer1.Start();
                        UpdateMinesRemaining();
                        UpdateTimer();
                        _board.IsFirstStep = false;
                    }
                    cell.OnClick();
                    AfterClick();
                    break;

                case MouseButtons.Right:
                    if (cell.Closed)
                    {
                        cell.OnFlag();
                        AfterClick();
                    }
                    break;

                default:
                    break;
            }
        }

        // ���������: �������
        private void beginnerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfirmNewGame(GameMode.Beginner);
        }

        // ���������: �������
        private void intermediateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfirmNewGame(GameMode.Intermediate);
        }

        // ���������: ������������
        private void expertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfirmNewGame(GameMode.Expert);
        }

        // ������������� ������ ����� ����
        private void ConfirmNewGame(GameMode mode)
        {
            var response = MessageBox.Show("�� ������ ������ ������ ����� ����?\n������� ���� ����� �������.", "������ ����� ����", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (response == DialogResult.Yes)
            {
                _mode = mode;
                StartGame();
            }
        }

        // ��� �������
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_board.GameOver)
            {
                timer1.Stop();
            }

            if (!_board.IsFirstStep)
            {
                _board.Time++;
                UpdateTimer();
            }
        }

        private void RecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Records = new Records();
            Records.ShowDialog();
        }
    }
}