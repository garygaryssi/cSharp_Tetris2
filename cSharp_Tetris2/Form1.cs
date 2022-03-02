using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cSharp_Tetris2
{
    public partial class Form1 : Form
    {
        Block currentBlock;
        Timer timer = new Timer();
        int currentX;
        int currentY;

        public Form1()
        {
            InitializeComponent();

            boardLoad();

            currentBlock = getRandomBlock();

            timer.Interval = 500;
            timer.Tick += timer1_Tick;
            timer.Start();

            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var verticalMove = 0;
            var horizontalMove = 0;

            switch (e.KeyCode)
            {

                case Keys.A:
                    verticalMove -= 1;
                    break;

                case Keys.D:
                    verticalMove += 1;
                    break;

                case Keys.S:
                    horizontalMove++;
                    break;

                case Keys.W:
                    Console.WriteLine("turn");
                    break;

                default:
                    return;
            }

            moveBlock(horizontalMove, verticalMove);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            moveBlock(moveDown: 1);
            check();
            
        }

        // block 움직임
        private void moveBlock(int moveDown = 0, int moveSide = 0)
        {
            var newX = currentX + moveSide;
            var newY = currentY + moveDown;

            currentX = newX;
            currentY = newY;

            drawShape();
        }

        private void check()
        {
        
        }


        // block class에서 랜덤하게 가져옴
        private Block getRandomBlock()
        {
            var block = Block.GetRandomShape();

            currentX = 7;
            currentY = -block.Height;

            return block;
        }

        Bitmap boardBitmap;
        Graphics boardGraphics;

        int boardWidth = 15;
        int boardHeight = 20;
        int dotSize = 20;

        // 게임 보드판 생성
        private void boardLoad()
        {
            pictureBox1.Width = boardWidth * dotSize;
            pictureBox1.Height = boardHeight * dotSize;

            boardBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            boardGraphics = Graphics.FromImage(boardBitmap);

            boardGraphics.FillRectangle(Brushes.LightGray, 0, 0, boardBitmap.Width, boardBitmap.Height);

            pictureBox1.Image = boardBitmap;
        }


        Bitmap workingBitmap;
        Graphics workingGraphics;

        //block 그리기
        private void drawShape()
        {
            workingBitmap = new Bitmap(boardBitmap);
            workingGraphics = Graphics.FromImage(workingBitmap);

            for (int i = 0; i < currentBlock.Width; i++)
            {
                for (int j = 0; j < currentBlock.Height; j++)
                {
                    if (currentBlock.Dots[j, i] == 1)
                        workingGraphics.FillRectangle(Brushes.Red, (currentX + i) * dotSize,  (currentY + j) * dotSize, dotSize, dotSize);
                }
            }

            pictureBox1.Image = workingBitmap;

        }

        
    }
}
