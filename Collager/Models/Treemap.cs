﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Taken from http://pascallaurin42.blogspot.com/2013/12/implementing-treemap-in-c.html

namespace Collager.Models
{
    public class Treemap
    {
        public static Slice<T> GetSlice<T>(IEnumerable<Element<T>> elements, double totalSize,
            double sliceWidth)
        {
            if (!elements.Any()) return null;
            if (elements.Count() == 1) return new Slice<T>
            {
                Elements = elements,
                Size = totalSize
            };

            var sliceResult = GetElementsForSlice(elements, sliceWidth);

            return new Slice<T>
            {
                Elements = elements,
                Size = totalSize,
                SubSlices = new[]
                {
                    GetSlice(sliceResult.Elements, sliceResult.ElementsSize, sliceWidth),
                    GetSlice(sliceResult.RemainingElements, 1 - sliceResult.ElementsSize,
                        sliceWidth)
                }
            };
        }

        private static SliceResult<T> GetElementsForSlice<T>(IEnumerable<Element<T>> elements,
            double sliceWidth)
        {
            var elementsInSlice = new List<Element<T>>();
            var remainingElements = new List<Element<T>>();
            double current = 0;
            double total = elements.Sum(x => x.Value);

            foreach (var element in elements)
            {
                if (current > sliceWidth)
                    remainingElements.Add(element);
                else
                {
                    elementsInSlice.Add(element);
                    current += element.Value / total;
                }
            }

            return new SliceResult<T>
            {
                Elements = elementsInSlice,
                ElementsSize = current,
                RemainingElements = remainingElements
            };
        }

        public static IEnumerable<SliceRectangle<T>> GetRectangles<T>(Slice<T> slice,
            int width, int height)
        {
            var area = new SliceRectangle<T> { Slice = slice, Width = width, Height = height };

            foreach (var rect in GetRectangles(area))
            {
                // Make sure no rectangle go outside the original area
                if (rect.X + rect.Width > area.Width) rect.Width = area.Width - rect.X;
                if (rect.Y + rect.Height > area.Height) rect.Height = area.Height - rect.Y;

                yield return rect;
            }
        }

        private static IEnumerable<SliceRectangle<T>> GetRectangles<T>(
            SliceRectangle<T> sliceRectangle)
        {
            var isHorizontalSplit = sliceRectangle.Width >= sliceRectangle.Height;
            var currentPos = 0;
            foreach (var subSlice in sliceRectangle.Slice.SubSlices)
            {
                var subRect = new SliceRectangle<T> { Slice = subSlice };
                int rectSize;

                if (isHorizontalSplit)
                {
                    rectSize = (int)Math.Round(sliceRectangle.Width * subSlice.Size);
                    subRect.X = sliceRectangle.X + currentPos;
                    subRect.Y = sliceRectangle.Y;
                    subRect.Width = rectSize;
                    subRect.Height = sliceRectangle.Height;
                }
                else
                {
                    rectSize = (int)Math.Round(sliceRectangle.Height * subSlice.Size);
                    subRect.X = sliceRectangle.X;
                    subRect.Y = sliceRectangle.Y + currentPos;
                    subRect.Width = sliceRectangle.Width;
                    subRect.Height = rectSize;
                }

                currentPos += rectSize;

                if (subSlice.Elements.Count() > 1)
                {
                    foreach (var sr in GetRectangles(subRect))
                        yield return sr;
                }
                else if (subSlice.Elements.Count() == 1)
                    yield return subRect;
            }
        }

    }

    public class SliceResult<T>
    {
        public IEnumerable<Element<T>> Elements { get; set; }
        public double ElementsSize { get; set; }
        public IEnumerable<Element<T>> RemainingElements { get; set; }
    }

    public class Slice<T>
    {
        public double Size { get; set; }
        public IEnumerable<Element<T>> Elements { get; set; }
        public IEnumerable<Slice<T>> SubSlices { get; set; }
    }

    public class Element<T>
    {
        public T Object { get; set; }
        public double Value { get; set; }
    }

    public class SliceRectangle<T>
    {
        public Slice<T> Slice { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}