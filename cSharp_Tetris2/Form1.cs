using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace cSharp_Tetris2
{
    public partial class Form1 : Form
    {
        string rtspUrl = "rtsp://192.168.21.96:554/media/1/1/Profile1";
        Block currentBlock;
        Timer timer = new Timer();


        int currentX;
        int currentY;


        public Form1()
        {
            InitializeComponent();

            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;

            boardLoad();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Game Start!!!!!!!");

            timer.Interval = 500;
            timer.Tick += timer1_Tick;
            timer.Start();

            currentBlock = getRandomBlock();

            LoadipCam();

        }

        private void LoadipCam()
        {
            VideoCapture video = new VideoCapture();

            video.Open(rtspUrl);

            using (Mat image = new Mat())
            {
                while (true)
                {
                    if (!video.Read(image))
                    {
                        Cv2.WaitKey();
                    }

                    if (!image.Empty())
                    {
                        Bitmap bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);

                        pictureBox1.BackgroundImage = bmp;

                    }
                    if (Cv2.WaitKey(1) >= 0)
                        break;
                }
            video = null;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            var moveBlock = moveBlockCheck(moveDown: 1);

            if (!moveBlock)
            {

                boardBitmap = new Bitmap(workingBitmap);

                boardUpdate();

                currentBlock = getRandomBlock();

                rowClear();
            }
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
                    currentBlock.turn();
                    break;

                default:
                    return;
            }
            var movecheck = moveBlockCheck(horizontalMove, verticalMove);

        }



        // block 움직임
        private bool moveBlockCheck(int moveDown = 0, int moveSide = 0)
        {
            var newX = currentX + moveSide;
            var newY = currentY + moveDown;

            if (newX < 0 || newX + currentBlock.Width > boardWidth || newY + currentBlock.Height > boardHeight)
                return false;

            for (int i = 0; i < currentBlock.Width; i++)
            {
                for (int j = 0; j < currentBlock.Height; j++)
                {
                    if (newY + j > 0 && boardArray[newX + i, newY + j] == 1 && currentBlock.Dots[j, i] == 1)
                        return false;
                }
            }

            currentX = newX;
            currentY = newY;

            drawShape();

            return true;
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
        int[,] boardArray;



        // 게임 보드판 생성
        private void boardLoad()
        {
            pictureBox1.Width = boardWidth * dotSize;
            pictureBox1.Height = boardHeight * dotSize;

            boardBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            boardGraphics = Graphics.FromImage(boardBitmap);

            boardGraphics.FillRectangle(Brushes.Transparent, 0, 0, boardBitmap.Width, boardBitmap.Height);

            pictureBox1.Image = workingBitmap;

            pictureBox1.Image = boardBitmap;

            boardArray = new int[boardWidth, boardHeight];
            
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
                        workingGraphics.FillRectangle(Brushes.Red, (currentX + i) * dotSize, (currentY + j) * dotSize, dotSize, dotSize);
                }
            }

            pictureBox1.Image = workingBitmap;

        }


        private void boardUpdate()
        {

            for (int i = 0; i < currentBlock.Width; i++)
            {
                for (int j = 0; j < currentBlock.Height; j++)
                {
                    if (currentBlock.Dots[j, i] == 1)
                    {

                        if (currentX + i > 15 || currentY + j < 0)
                        {
                            break;
                        }
                        checkGameOver();

                        boardArray[currentX + i, currentY + j] = 1;

                    }
                }

            }

        }

        private bool checkGameOver()
        {
            if (currentY < 0)
            {
                timer.Stop();
                MessageBox.Show("Game Over");
                Application.Restart();
            }

            return true;
        }

        int score;
        bool clearCheck = false;

        private void rowClear()
        {

            for (int i = 0; i < boardHeight; i++)
            {
                int j;
                for (j = boardWidth - 1; j >= 0; j--)
                {
                    if (boardArray[j, i] == 0)
                        break;
                }

                if (j == -1)
                {
                    clearCheck = true;
                    score++;
                    label1.Text = "Score: " + score;
                    label2.Text = "Level: " + score / 10;
                    timer.Interval -= 10;

                    for (j = 0; j < boardWidth; j++)
                    {
                        for (int k = i; k > 0; k--)
                        {
                            // Row 삭제후 block 이동
                            boardArray[j, k] = boardArray[j, k - 1];
                        }

                        boardArray[j, 0] = 0;
                    }
                }
            }

            for (int i = 0; i < boardHeight; i++)
            {

                for (int j = 0; j < boardWidth; j++)
                {
                    Console.Write(boardArray[j, i] + " ");

                    boardGraphics = Graphics.FromImage(boardBitmap);

                    if (boardArray[j, i] == 1)
                    {
                        boardGraphics.FillRectangle(Brushes.BlanchedAlmond, j * dotSize, i * dotSize, dotSize, dotSize);
                    }

                    if (boardArray[j, i] == 0 && clearCheck == true)
                    {
                        boardGraphics.Clear(Color.Transparent);

                        clearCheck = false;
                    }
                }
                Console.WriteLine(" ");
            }

            pictureBox1.Image = boardBitmap;
        }

    }

}
