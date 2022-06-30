using SkiaSharp;

namespace Imagize.Core.Extensions
{
    /// <summary>
    /// Extensions methods for SkiaSharp.
    /// From: https://github.com/danielbisar/Medja/blob/00ca44833e163dd4e52b4e15139c1c2b6ab06da5/src/Medja.OpenTk/Utils/SkiaExtensions.cs
    /// </summary>
    public static class SkiaExtensions
    {
        /// <summary>
        /// Same as <see cref="SKCanvas.DrawText(SkiaSharp.SKTextBlob,float,float,SkiaSharp.SKPaint)"/> except that it
        /// does not throw an exception if text is null or empty.
        /// </summary>
        /// <param name="canvas">The <see cref="SKCanvas"/> to draw on.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="x">X-coordinate.</param>
        /// <param name="y">Y-coordinate.</param>
        /// <param name="paint">The <see cref="SKPaint"/> to use.</param>
        public static void DrawTextSafe(this SKCanvas canvas, string text, float x, float y, SKPaint paint)
        {
            if (string.IsNullOrEmpty(text))
                return;

            canvas.DrawText(text, x, y, paint);
        }

        /// <summary>
        /// Gets the width of a text.
        /// </summary>
        /// <param name="paint"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static float GetTextWidth(this SKPaint paint, string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return paint.MeasureText(text);
        }
    }

}
