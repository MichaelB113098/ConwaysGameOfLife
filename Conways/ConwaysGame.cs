using System;
using System.Drawing;
using System.Windows.Forms;

namespace Conways
{
    public partial class ConwaysGame : Form
    {
        public static Bitmap Map; //Bitmap to store graphics
        public static Color CurrentColor = Color.Black; //Current color, allows color of blocks to change when the game is running
        public Square[,] SquareArray; //Array to store sqaures of the game
        public bool LoopingBoard = false; //Boolean to determine if the game board loops around itself or is bound

        public ConwaysGame()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SquareArray = new Square[pictureBox1.Width / 10, pictureBox1.Height / 10]; //Intialize SqaureArray, sized to be 10x10 pixels
            Map = new Bitmap(pictureBox1.Width, pictureBox1.Height); //Set map to the size of the picturebox itll be displayed in

            //Loop through each index in the SquareArray, setting its starting point and calling the empty square method for each
            for (var i = 0; i < SquareArray.GetLength(0); i++) 
            for (var j = 0; j < SquareArray.GetLength(1); j++)
            {
                SquareArray[i, j] = new Square(new Point(i * 10, j * 10));
                SquareArray[i, j].EmptySquare();
            }

            UpdateGraphics(); 
        }

        //Loop through each square in the array, if the square has been set to change, switch from alive to empty, or empty to alive. Call proper graphics method for each
        public void UpdateSquares()
        {
            for (var i = 0; i < SquareArray.GetLength(0); i++)
                for (var j = 0; j < SquareArray.GetLength(1); j++)
                    if (SquareArray[i, j].SetForChange)
                    {
                        if (SquareArray[i, j].IsAlive)
                        {
                            SquareArray[i, j].EmptySquare();
                            SquareArray[i, j].SetForChange = false;
                            SquareArray[i, j].IsAlive = false;
                        }
                        else
                        {
                            SquareArray[i, j].FillSquare(CurrentColor);
                            SquareArray[i, j].SetForChange = false;
                            SquareArray[i, j].IsAlive = true;
                        }
                    }

            UpdateGraphics();
        }
        
        //Set picturebox image to new bitmap
        public void UpdateGraphics()
        {
            pictureBox1.Image = Map;
        }

        //Recolor all filled squares to the current color, used when turning the game on or off
        public void ReColorAll()
        {
            for (var i = 0; i < SquareArray.GetLength(0); i++)
            for (var j = 0; j < SquareArray.GetLength(1); j++)
                if (SquareArray[i, j].IsFilled)
                    SquareArray[i, j].FillSquare(CurrentColor);
            UpdateGraphics();
        }

        //When the user presses enter, start or end the game
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (GenerationTimer.Enabled)
                {
                    CurrentColor = Color.Black;
                    GenerationTimer.Enabled = false;
                    setSpeedToolStripMenuItem.Enabled = true;
                    loopingBoardToolStripMenuItem.Enabled = true;
                    GenerationTimer.Stop();
                    ReColorAll();
                }
                else
                {
                    CurrentColor = Color.Blue;
                    GenerationTimer.Enabled = true;
                    setSpeedToolStripMenuItem.Enabled = false;
                    loopingBoardToolStripMenuItem.Enabled = false;
                    GenerationTimer.Start();
                    ReColorAll();
                }
            }
        }

        /*
        Core game logic, loop through each square and find the count of alive neighbors 
        If an alive square doesnt have 2 or 3 neighbors, set it for change to be emptied
        If an empty square has exactly 3 neighbors, set it for change to be alive
        Then call the updateSqaures method to empty and fill squares
        */
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

        /*
        Gets the amount of alive neighbors around the given index[i,j]  
        if the board is not looping, ignore indexes which would be out of bounds
        if the board is looping, instead wrap the index around to the end or start of the array(if i == 0, check where i == array.length - 1)
        return count as int
        */
        private int GetNeighborCount(int i, int j)
        {
            var count = 0;
            if (!LoopingBoard)
            {
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
            }
            else
            {
                var iMinus = i - 1;
                var jMinus = j - 1;
                var iPlus = i + 1;
                var jPlus = j + 1;
                if (i == 0)
                    iMinus = SquareArray.GetLength(0) - 1;
                if (j == 0)
                    jMinus = SquareArray.GetLength(1) - 1;
                if (i == SquareArray.GetLength(0) - 1)
                    iPlus = 0;
                if (j == SquareArray.GetLength(1) - 1)
                    jPlus = 0;
                if (SquareArray[iMinus, jMinus].IsAlive) count++;
                if (SquareArray[iMinus, jPlus].IsAlive) count++;
                if (SquareArray[iPlus, jPlus].IsAlive) count++;
                if (SquareArray[iPlus, jMinus].IsAlive) count++;

                if (SquareArray[iMinus, j].IsAlive) count++;
                if (SquareArray[iPlus, j].IsAlive) count++;
                if (SquareArray[i, jMinus].IsAlive) count++;
                if (SquareArray[i, jPlus].IsAlive) count++;
            }

            return count;
        }

        //Tick for when the left mouse is held down to fill in squares cursor is currently on, checks to make sure game isnt running and that the mouse is in the form
        private void FillSquare_Tick(object sender, EventArgs e)
        {
            if (!pictureBox1.ClientRectangle.Contains(PointToClient(MousePosition)) ||
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
        //See above method, repeated logic for when right mouse is held down to empty squares
        private void EmptySquare_Tick(object sender, EventArgs e)
        {
            if (!pictureBox1.ClientRectangle.Contains(PointToClient(MousePosition)) ||
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
        //Starts the square tick methods when mouse buttons are pressed
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
        //Stops the square tick methods when mouse buttons are no longer pressed
        private void SquareGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                timer1.Stop();
            else
                timer2.Stop();
        }

        //New game strip menu, stops game, sets all squares to not alive and empty
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < SquareArray.GetLength(0); i++)
            for (var j = 0; j < SquareArray.GetLength(1); j++)
                if (SquareArray[i, j].IsAlive)
                {
                    SquareArray[i, j].IsAlive = false;
                    SquareArray[i, j].EmptySquare();
                }
            if (GenerationTimer.Enabled)
            {
                CurrentColor = Color.Black;
                GenerationTimer.Enabled = false;
                setSpeedToolStripMenuItem.Enabled = true;
                loopingBoardToolStripMenuItem.Enabled = true;
                GenerationTimer.Stop();
                ReColorAll();
            }
            UpdateGraphics();
        }
        //Sets speed of generation tick using tool strip
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

        //Square Class, with bitmap graphics methods
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

        
        private void loopingBoardToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            LoopingBoard = !LoopingBoard;
        }
    }
}