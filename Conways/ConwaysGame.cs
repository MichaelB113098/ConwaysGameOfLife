using System;
using System.Drawing;
using System.Windows.Forms;

namespace Conways
{
    public partial class ConwaysGame : Form
    {
        public static Bitmap Map;
        public static Color CurrentColor = Color.Black;
        public bool IsStarted = false;
        public Square[,] SquareArray;

        public ConwaysGame()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SquareArray = new Square[pictureBox1.Width / 10, pictureBox1.Height / 10];
            Map = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            for (var i = 0; i < SquareArray.GetLength(0); i++)
            for (var j = 0; j < SquareArray.GetLength(1); j++)
            {
                SquareArray[i, j] = new Square(new Point(i * 10, j * 10));
                SquareArray[i, j].EmptySquare();
            }

            UpdateGraphics();
        }

        public void UpdateSquares()
        {
            for (var i = 0; i < SquareArray.GetLength(0); i++)
            for (var j = 0; j < SquareArray.GetLength(1); j++)
                if (SquareArray[i, j].IsAlive && SquareArray[i, j].SetForChange)
                {
                    SquareArray[i, j].EmptySquare();
                    SquareArray[i, j].SetForChange = false;
                    SquareArray[i, j].IsAlive = false;
                }
                else if (!SquareArray[i, j].IsAlive && SquareArray[i, j].SetForChange)
                {
                    SquareArray[i, j].FillSquare(CurrentColor);
                    SquareArray[i, j].SetForChange = false;
                    SquareArray[i, j].IsAlive = true;
                }

            UpdateGraphics();
        }

        public void UpdateGraphics()
        {
            pictureBox1.Image = Map;
        }

        public void ReColorAll()
        {
            for (var i = 0; i < SquareArray.GetLength(0); i++)
            for (var j = 0; j < SquareArray.GetLength(1); j++)
                if (SquareArray[i, j].IsFilled)
                    SquareArray[i, j].FillSquare(CurrentColor);
            UpdateGraphics();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (GenerationTimer.Enabled)
                {
                    CurrentColor = Color.Black;
                    GenerationTimer.Enabled = false;
                    setSpeedToolStripMenuItem.Enabled = true;
                    GenerationTimer.Stop();
                    ReColorAll();
                }
                else
                {
                    CurrentColor = Color.Blue;
                    GenerationTimer.Enabled = true;
                    setSpeedToolStripMenuItem.Enabled = false;
                    GenerationTimer.Start();
                    ReColorAll();
                }
            }
        }

        public void NextGeneration()
        {
            for (var i = 0; i < SquareArray.GetLength(0); i++)
            for (var j = 0; j < SquareArray.GetLength(1); j++)
            {
                var currentSquare = SquareArray[i, j];
                var count = GetNeighborCount(i, j);
                if (currentSquare.IsAlive)
                {
                    if (count < 2 || count > 3)
                        currentSquare.SetForChange = true;
                }
                else if (count == 3)
                {
                    currentSquare.SetForChange = true;
                }
            }

            UpdateSquares();
        }

        private int GetNeighborCount(int i, int j)
        {
            var count = 0;
            if (i != 0)
                if (SquareArray[i - 1, j].IsAlive)
                    count++;
            if (j != 0)
                if (SquareArray[i, j - 1].IsAlive)
                    count++;
            if (j != 0 && i != 0)
                if (SquareArray[i - 1, j - 1].IsAlive)
                    count++;
            if (j != SquareArray.GetLength(1) - 1)
                if (SquareArray[i, j + 1].IsAlive)
                    count++;
            if (i != SquareArray.GetLength(0) - 1)
                if (SquareArray[i + 1, j].IsAlive)
                    count++;
            if (i != SquareArray.GetLength(0) - 1 && j != SquareArray.GetLength(1) - 1)
                if (SquareArray[i + 1, j + 1].IsAlive)
                    count++;
            if (i != 0 && j != SquareArray.GetLength(1) - 1)
                if (SquareArray[i - 1, j + 1].IsAlive)
                    count++;
            if (j != 0 && i != SquareArray.GetLength(0) - 1)
                if (SquareArray[i + 1, j - 1].IsAlive)
                    count++;
            return count;
        }

        private void FillSquare_Tick(object sender, EventArgs e)
        {
            if (IsStarted || !pictureBox1.ClientRectangle.Contains(PointToClient(MousePosition)) ||
                GenerationTimer.Enabled) return;
            var position = pictureBox1.PointToClient(Cursor.Position);
            var currentSquare = SquareArray[(position.X - position.X % 10) / 10,
                (position.Y - position.Y % 10) / 10];
            if (!currentSquare.IsAlive)
            {
                currentSquare.IsAlive = true;
                currentSquare.FillSquare(CurrentColor);
            }

            UpdateGraphics();
        }

        private void EmptySquare_Tick(object sender, EventArgs e)
        {
            if (IsStarted || !pictureBox1.ClientRectangle.Contains(PointToClient(MousePosition)) ||
                GenerationTimer.Enabled) return;
            var position = pictureBox1.PointToClient(Cursor.Position);
            var currentSquare = SquareArray[(position.X - position.X % 10) / 10,
                (position.Y - position.Y % 10) / 10];
            if (currentSquare.IsAlive)
            {
                currentSquare.IsAlive = false;
                currentSquare.EmptySquare();
            }

            UpdateGraphics();
        }

        private void Generation_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void SquareGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                timer1.Enabled = true;
                timer1.Start();
            }
            else
            {
                timer2.Enabled = true;
                timer2.Start();
            }
        }

        private void SquareGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                timer1.Stop();
            else
                timer2.Stop();
        }


        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < SquareArray.GetLength(0); i++)
            for (var j = 0; j < SquareArray.GetLength(1); j++)
                if (SquareArray[i, j].IsAlive)
                {
                    SquareArray[i, j].IsAlive = false;
                    SquareArray[i, j].EmptySquare();
                }

            UpdateGraphics();
        }

        private void setSpeedToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == msToolStripMenuItem)
            {
                msToolStripMenuItem.Checked = true;
                msToolStripMenuItem1.Checked = false;
                msToolStripMenuItem2.Checked = false;
                msToolStripMenuItem3.Checked = false;
                GenerationTimer.Interval = 10;
            }

            if (e.ClickedItem == msToolStripMenuItem1)
            {
                msToolStripMenuItem.Checked = false;
                msToolStripMenuItem1.Checked = true;
                msToolStripMenuItem2.Checked = false;
                msToolStripMenuItem3.Checked = false;
                GenerationTimer.Interval = 50;
            }

            if (e.ClickedItem == msToolStripMenuItem2)
            {
                msToolStripMenuItem.Checked = false;
                msToolStripMenuItem1.Checked = false;
                msToolStripMenuItem2.Checked = true;
                msToolStripMenuItem3.Checked = false;
                GenerationTimer.Interval = 100;
            }

            if (e.ClickedItem == msToolStripMenuItem3)
            {
                msToolStripMenuItem.Checked = false;
                msToolStripMenuItem1.Checked = false;
                msToolStripMenuItem2.Checked = false;
                msToolStripMenuItem3.Checked = true;
                GenerationTimer.Interval = 500;
            }
        }

        public class Square
        {
            private readonly Point _endPoint;
            private readonly Point _startPoint;
            public bool IsAlive;
            public bool IsFilled;
            public bool SetForChange;

            public Square(Point startPoint)
            {
                _startPoint = startPoint;
                _endPoint = new Point(startPoint.X + 10, startPoint.Y + 10);
            }

            public void FillSquare(Color color)
            {
                for (var i = _startPoint.X; i < _endPoint.X - 1; i++)
                for (var j = _startPoint.Y; j < _endPoint.Y - 1; j++)
                    Map.SetPixel(i, j, color);
                IsFilled = true;
            }

            public void EmptySquare()
            {
                FillSquare(Color.White);

                using (var g = Graphics.FromImage(Map))
                {
                    using (var pen = new Pen(Color.Black, 1))
                    {
                        g.DrawRectangle(pen, _startPoint.X, _startPoint.Y, _endPoint.X, _endPoint.Y);
                    }
                }

                IsFilled = false;
            }
        }
    }
}