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

using System.Collections.Generic;
using System.Linq;
namespace MooreNeighbor
{
    /// <summary>
    /// This class provides methods to perfrom a contour tracing on a black and white image
    /// represented as a 2 dimensional boolean array using the moore-neighborhood algorthim
    /// http://en.wikipedia.org/wiki/Moore_neighborhood
    /// </summary>
    public class ContourTrace
    {
        /// <summary>
        /// For any given key point, the point that is clockwise to it in a moore-neighborhood
        /// </summary>

        private static readonly Dictionary<Point, Point> clockwiseOffset = new Dictionary<Point, Point>()
        {
             {new Point(1,0), new Point(1,-1) },    // right        => down-right
             {new Point(1,-1), new Point(0,-1)},    // down-right   => down
             {new Point(0,-1), new Point(-1,-1)},   // down         => down-left
             {new Point(-1,-1), new Point(-1,0)},   // down-left    => left
             {new Point(-1,0), new Point(-1,1)},    // left         => top-left
             {new Point(-1,1), new Point(0,1)},     // top-left     => top
             {new Point(0,1), new Point(1,1)},      // top          => top-right
             {new Point(1,1), new Point(1,0)}       // top-right    => right
        };

        /// <summary>
        /// returns all the points that make up the outline of a two dimensional black and white image as represented by a bool[,]
        ///
        /// Pseudo code for Moore-Neighborhood
        /// retrieved from http://en.wikipedia.org/wiki/Moore_neighborhood
        /// Begin
        ///     Set B to be empty.
        ///     From bottom to top and left to right scan the cells of T until a black pixel, s, of P is found.
        ///     Insert s in B.
        ///     Set the current boundary point p to s i.e. p=s
        ///     b = the pixel from which s was entered during the image scan.
        ///     Set c to be the next clockwise pixel (from b) in M(p).
        ///     While c not equal to s do
        ///     If c is black
        ///         insert c in B
        ///         b = p
        ///         p = c
        ///         (backtrack: move the current pixel c to the pixel from which p was entered)
        ///         c = next clockwise pixel (from b) in M(p).
        ///     else
        ///         (advance the current pixel c to the next clockwise pixel in M(p) and update backtrack)
        ///         b = c
        ///         c = next clockwise pixel (from b) in M(p).
        ///     end While
        /// End
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Point[] Trace(bool[,] image)
        {
            int xBound = image.GetLength(1), yBound = image.GetLength(0);

            HashSet<Point> outline = new HashSet<Point>();
            Point prev,       // The point we entered curr from
                    curr,       // The point currently being inspected
                    boundary,   // current know black pixel we're finding neighbours of
                    first,      // the first black pxiel found
                    firstPrev;  // the point we entered first from

            // find the fist black pixel, searching from bottom-left to top-right
            for (int y = yBound - 1; y >= 0; y--)
            {
                firstPrev = new Point(0, y - 1);
                for (int x = 0; x < xBound; x++)
                {
                    // is black then move on
                    if (image[y, x])
                    {
                        first = new Point(x, y);
                        goto FoundFirstPixel;
                    }
                    firstPrev = new Point(x, y);
                }
            }

            // Couldn't find any black pixels
            return outline.ToArray();

        FoundFirstPixel:
            prev = firstPrev;
            outline.Add(first);
            boundary = first;

            curr = Clockwise(boundary, prev);

            // Jacob's stopping criterion:
            // stop only when we enter the original pixel in the same way we entered it
            while (curr != first || prev != firstPrev)
            {
                // if the current pixel is black
                // then add it to the outline
                if (curr.y >= 0 && curr.x >= 0 &&
                    curr.y < yBound && curr.x < xBound &&
                    image[curr.y, curr.x])
                {
                    outline.Add(curr);
                    prev = boundary;
                    boundary = curr;
                    curr = Clockwise(boundary, prev);
                }
                else
                {
                    prev = curr;
                    curr = Clockwise(boundary, prev);
                }
            }
            return outline.ToArray();
        }

        private static Point Clockwise(Point target, Point prev)
        {
            return clockwiseOffset[prev - target] + target;
        }
    }
}