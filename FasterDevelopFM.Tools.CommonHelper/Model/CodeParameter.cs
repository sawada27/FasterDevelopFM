using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FasterDevelopFM.Tools.VertificationCodeGenerator.Model
{
    public class CodeParamter
    {
        /// <summary>
        /// 验证码文本
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 验证码颜色
        /// </summary>
        public SKColor[] CodeColors { get; set; }
        /// <summary>
        /// 需要排除的验证码颜色
        /// </summary>
        public SKColor[] ExcludeCodeColors { get; set; }
        /// <summary>
        /// 图片宽度（默认值为120）
        /// </summary>
        public int Width { get; set; } = 120;
        /// <summary>
        /// 图片高度（默认值为30）
        /// </summary>
        public int Height { get; set; } = 30;
        /// <summary>
        /// 干扰线数量（默认值为3）
        /// </summary>
        public uint Line { get; set; } = 3;
        /// <summary>
        /// 干扰线粗细（默认值为2）
        /// </summary>
        public uint LineStrokeWidth { get; set; } = 2;
        /// <summary>
        /// 干扰线颜色
        /// </summary>
        public SKColor[] LineColors { get; set; }
        /// <summary>
        /// 需要排除的干扰线颜色
        /// </summary>
        public SKColor[] ExcludeLineColors { get; set; }
        /// <summary>
        /// 图片格式（默认值为jpeg）
        /// </summary>
        public SKEncodedImageFormat ImageFormat { get; set; } = SKEncodedImageFormat.Jpeg;
        /// <summary>
        /// 图片质量（数值为0-100，默认值为100）
        /// </summary>
        public int ImageQuality { get; set; } = 100;
        /// <summary>
        /// 背景色，默认为亮灰色<see cref="SKColors"/>
        /// </summary>
        public SKColor BackgroundColor { get; set; } = SKColors.LightGray;
    }
}
