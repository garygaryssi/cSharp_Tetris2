using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cSharp_Tetris2
{
    internal class Block
    {

        private static Block[] blockArray;
        private int[,] backupDots;

        public int Width;
        public int Height;
        public int[,] Dots;



        static Block()
        {
            blockArray = new Block[]
            {
               new Block
               {
                   Width = 3,
                   Height = 2,
                   Dots = new int[2, 3]
                   {
                       {1, 1, 0},
                       {0, 1, 1}
                   }
               },

               new Block
               {
                   Width = 2,
                   Height = 2,
                   Dots = new int[2, 2]
                   {
                       {1, 1},
                       {1, 1}
                   }
               },

               new Block
               {
                   Width = 1,
                   Height = 4,
                   Dots = new int [4, 1]
                   {
                       {1},
                       {1},
                       {1},
                       {1}
                   }
               },

               new Block
               {
                   Width = 2,
                   Height = 3,
                   Dots = new int[3, 2]
                   {
                       {1, 1 },
                       {1, 0 },
                       {1, 0 }
                   }
               }
            };

        }

        public void turn()
        {
            backupDots = Dots;

            Dots = new int[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Dots[i, j] = backupDots[Height - 1 - j, i];
                }
            }

            var temp = Width;
            Width = Height;
            Height = temp;

        }



        public static Block GetRandomShape()
        {
            var block = blockArray[new Random().Next(blockArray.Length)];

            return block;
        }
    }
}
