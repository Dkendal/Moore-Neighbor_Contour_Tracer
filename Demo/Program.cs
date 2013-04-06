/* Copyright (C) 2013 Dylan Kendal
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Linq;
using System.IO;
namespace MooreNeighbor
{
    /// <summary>
    /// Toy example of using moore-neighbor algorithim
    /// </summary>
    internal class Program
    {
        private static int arrayX, arrayY;

        /// <summary>
        /// Initialize image array
        /// </summary>
        /// <param name="array"></param>
        private static void Init(out bool[,] array)
        {

            string[] lines = File.ReadAllLines(@"Demo\Image.txt");

            if (lines.Min(x => x.Length) != lines.Max(x => x.Length)) throw new Exception("Image rows not of uniform size");

            arrayY = lines.Length;
            arrayX = lines[0].Length;

            array = new bool[arrayY, arrayX];

            for (int y = 0; y < arrayY; y++)
            {
                for (int x = 0; x < arrayX; x++)
                {
                    array[y, x] = lines[y][x] == '1';
                }
            }

        }

        private static void Main(string[] args)
        {
            bool[,] array;   // array that will store our black and white image;

            Init(out array);

            Redraw(array);

            System.Threading.Thread.Sleep(2000);

            var outline = ContourTrace.Trace(array);

            array = new bool[arrayY, arrayX];

            foreach (var p in outline)
            {
                array[p.y, p.x] = true;
            }

            Redraw(array);
            System.Threading.Thread.Sleep(2000);
        }

        private static void Redraw(bool[,] array)
        {
            Console.Clear();
            for (int y = 0; y <= arrayY; y++)
            {
                if (y == arrayY)
                {
                    for (int i = 0; i < arrayX; i++)
                    {
                        Console.Write('=');
                    }
                }
                else
                {
                    for (int x = 0; x <= arrayX; x++)
                    {
                        if (x == arrayX)
                        {
                            Console.Write('|');
                        }
                        else
                        {
                            Console.Write(array[y, x] ? 'X' : ' ');
                        }
                    }
                }
                Console.Write('\n');
            }
        }
    }
}