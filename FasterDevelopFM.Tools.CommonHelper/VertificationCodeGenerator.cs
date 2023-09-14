using FasterDevelopFM.Tools.CommonHelper.Model;
using SkiaSharp;
using System;

namespace FasterDevelopFM.Tools.CommonHelper
{
    public class VertificationCodeGenerator
    {
        /// <summary>
        /// 颜色集合
        /// </summary>
        private static readonly List<SKColor> Colors = new List<SKColor>
        {
            SKColors.AliceBlue,
            SKColors.PaleGreen,
            SKColors.PaleGoldenrod,
            SKColors.Orchid,
            SKColors.OrangeRed,
            SKColors.Orange,
            SKColors.OliveDrab,
            SKColors.Olive,
            SKColors.OldLace,
            SKColors.Navy,
            SKColors.NavajoWhite,
            SKColors.Moccasin,
            SKColors.MistyRose,
            SKColors.MintCream,
            SKColors.MidnightBlue,
            SKColors.MediumVioletRed,
            SKColors.MediumTurquoise,
            SKColors.MediumSpringGreen,
            SKColors.LightSteelBlue,
            SKColors.LightYellow,
            SKColors.Lime,
            SKColors.LimeGreen,
            SKColors.Linen,
            SKColors.PaleTurquoise,
            SKColors.Magenta,
            SKColors.MediumAquamarine,
            SKColors.MediumBlue,
            SKColors.MediumOrchid,
            SKColors.MediumPurple,
            SKColors.MediumSeaGreen,
            SKColors.MediumSlateBlue,
            SKColors.Maroon,
            SKColors.PaleVioletRed,
            SKColors.PapayaWhip,
            SKColors.PeachPuff,
            SKColors.Snow,
            SKColors.SpringGreen,
            SKColors.SteelBlue,
            SKColors.Tan,
            SKColors.Teal,
            SKColors.Thistle,
            SKColors.Tomato,
            SKColors.Violet,
            SKColors.Wheat,
            SKColors.White,
            SKColors.WhiteSmoke,
            SKColors.Yellow,
            SKColors.YellowGreen,
            SKColors.Turquoise,
            SKColors.LightSkyBlue,
            SKColors.SlateBlue,
            SKColors.Silver,
            SKColors.Peru,
            SKColors.Pink,
            SKColors.Plum,
            SKColors.PowderBlue,
            SKColors.Purple,
            SKColors.Red,
            SKColors.SkyBlue,
            SKColors.RosyBrown,
            SKColors.SaddleBrown,
            SKColors.Salmon,
            SKColors.SandyBrown,
            SKColors.SeaGreen,
            SKColors.SeaShell,
            SKColors.Sienna,
            SKColors.RoyalBlue,
            SKColors.LightSeaGreen,
            SKColors.LightSalmon,
            SKColors.LightPink,
            SKColors.Crimson,
            SKColors.Cyan,
            SKColors.DarkBlue,
            SKColors.DarkCyan,
            SKColors.DarkGoldenrod,
            SKColors.DarkGray,
            SKColors.Cornsilk,
            SKColors.DarkGreen,
            SKColors.DarkMagenta,
            SKColors.DarkOliveGreen,
            SKColors.DarkOrange,
            SKColors.DarkOrchid,
            SKColors.DarkRed,
            SKColors.DarkSalmon,
            SKColors.DarkKhaki,
            SKColors.DarkSeaGreen,
            SKColors.CornflowerBlue,
            SKColors.Chocolate,
            SKColors.AntiqueWhite,
            SKColors.Aqua,
            SKColors.Aquamarine,
            SKColors.Azure,
            SKColors.Beige,
            SKColors.Bisque,
            SKColors.Coral,
            SKColors.Black,
            SKColors.Blue,
            SKColors.BlueViolet,
            SKColors.Brown,
            SKColors.BurlyWood,
            SKColors.CadetBlue,
            SKColors.Chartreuse,
            SKColors.BlanchedAlmond,
            SKColors.Transparent,
            SKColors.DarkSlateBlue,
            SKColors.DarkTurquoise,
            SKColors.IndianRed,
            SKColors.Indigo,
            SKColors.Ivory,
            SKColors.Khaki,
            SKColors.Lavender,
            SKColors.LavenderBlush,
            SKColors.HotPink,
            SKColors.LawnGreen,
            SKColors.LightBlue,
            SKColors.LightCoral,
            SKColors.LightCyan,
            SKColors.LightGoldenrodYellow,
            SKColors.LightGreen,
            SKColors.LemonChiffon,
            SKColors.Honeydew,
            SKColors.Green,
            SKColors.DarkViolet,
            SKColors.DeepPink,
            SKColors.DeepSkyBlue,
            SKColors.DimGray,
            SKColors.DodgerBlue,
            SKColors.Firebrick,
            SKColors.GreenYellow,
            SKColors.FloralWhite,
            SKColors.Fuchsia,
            SKColors.Gainsboro,
            SKColors.GhostWhite,
            SKColors.Gold,
            SKColors.Goldenrod,
            SKColors.ForestGreen
        };

        private static Random _random = new Random();
        /// <summary>
        /// 获取文字验证码图片
        /// </summary>
        /// <param name="paramter">验证码参数</param>
        /// <returns></returns>
        public static byte[] GetTextImage(CodeParamter paramter)
        {
            //创建bitmap位图
            using var bitmap = new SKBitmap(paramter.Width, paramter.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            //创建画布
            using var canvas = new SKCanvas(bitmap);
            //填充背景颜色
            canvas.DrawColor(paramter.BackgroundColor);
            var array = paramter.Code.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                //将文字写到画布上
                using var paintText = CreatePaint(paramter.Height, paramter.CodeColors, paramter.ExcludeCodeColors);
                canvas.DrawText(array[i].ToString(), i * 32 + 1, paramter.Height - 1, paintText);
            }
            DrawInterferenceLine(paramter, canvas);
            //返回图片byte
            using var img = SKImage.FromBitmap(bitmap);
            using var data = img.Encode(paramter.ImageFormat, paramter.ImageQuality);
            return data.ToArray();
        }

        /// <summary>
        /// 画随机干扰线
        /// </summary>
        /// <returns></returns>
        private static void DrawInterferenceLine(CodeParamter paramter, SKCanvas canvas)
        {
            using SKPaint paint = new SKPaint();
            for (int i = 0; i < paramter.Line; i++)
            {
                paint.Color = GetRandomColor(paramter.LineColors, paramter.ExcludeLineColors);
                paint.StrokeWidth = paramter.LineStrokeWidth;
                var x0 = _random.Next(0, paramter.Width);
                var y0 = _random.Next(0, paramter.Height);
                var x1 = _random.Next(0, paramter.Width);
                var y1 = _random.Next(0, paramter.Height);
                canvas.DrawLine(x0, y0, x1, y1, paint);
            }
        }

        /// <summary>
        /// 创建画笔
        /// </summary>
        /// <param name="fontSize">画笔大小</param>
        /// <param name="includeColors">需要包含的颜色</param>
        /// <param name="excludeColors">需要排除的颜色</param>
        /// <returns></returns>
        private static SKPaint CreatePaint(float fontSize, SKColor[] includeColors, SKColor[] excludeColors)
        {
            SKPaint paint = new SKPaint
            {
                IsAntialias = true,
                Color = GetRandomColor(includeColors, excludeColors),
                TextSize = fontSize
            };
            return paint;
        }

        /// <summary>
        /// 随机获取颜色
        /// </summary>
        /// <param name="includeColors">需要包含的颜色</param>
        /// <param name="excludeColors">需要排除的颜色</param>
        /// <returns></returns>
        private static SKColor GetRandomColor(SKColor[] includeColors, SKColor[] excludeColors)
        {
            var colors = GetColors(includeColors, excludeColors);
            return colors[_random.Next(colors.Length)];
        }

        /// <summary>
        /// 获取排除颜色后的颜色
        /// </summary>
        /// <param name="includeColors">需要包含的颜色</param>
        /// <param name="excludeColors">需要排除的颜色</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">颜色数量</exception>
        private static SKColor[] GetColors(SKColor[] includeColors, SKColor[] excludeColors)
        {
            IEnumerable<SKColor> colors;
            if (includeColors == null)
            {
                colors = Colors;
            }
            else
            {
                if (includeColors.Length < 3)
                {
                    throw new ArgumentException("the number of colors cannot be less than 3");
                }
                colors = includeColors;
            }
            if (excludeColors == null || excludeColors.Length == 0)
            {
                return colors.ToArray();
            }
            colors = colors.Except(excludeColors).ToArray();
            if (colors.Count() < 3)
            {
                throw new ArgumentException("the number of colors cannot be less than 3");
            }
            return colors.ToArray();
        }



    }
}