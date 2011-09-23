using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roberocity
{
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;

	public class ImageEditing
	{
		/// <summary>
		/// Gets the resized image
		/// </summary>
		/// <param name="path">The path to the image to resize</param>
		/// <param name="width">The width to size it to</param>
		/// <param name="height">The height to size it to</param>
		/// <returns>A byte array of image data</returns>
		public static byte[] GetResizedImage(String path, int width, int height)
		{
			byte[] outBytes;
			using(var imgIn = new Bitmap(path))
			{
				double y = imgIn.Height;
				double x = imgIn.Width;
				double factor = 1;
				if (width > 0)
				{
					factor = width/x;
				}
				else if (height > 0)
				{
					factor = height/y;
				}

				var offset = 0;
				using (var outStream = new MemoryStream())
				{
					using (var imgOut = new Bitmap((int) (x*factor) + offset, (int) (y*factor) + offset))
					{
						using (var g = Graphics.FromImage(imgOut))
						{
							//var colors = new List<Color>() { Color.Blue, Color.Green, Color.Red, Color.Salmon, Color.Silver, Color.Yellow, Color.Tomato, Color.Orange, Color.Pink };
							//var d = DateTime.Now.Ticks;
							//var r = (int)(d % colors.Count);
							//g.Clear(colors[r]);
                                   g.Clear(Color.White); 
							g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
							g.DrawImage(
								imgIn,
								new Rectangle(offset/2, offset/2, (int) (factor*x), (int) (factor*y)),
								new Rectangle(0, 0, (int) x, (int) y),
								GraphicsUnit.Pixel);

							//g.DrawString(DateTime.Now.ToString("hh:mm:ss") + "\n" + g.InterpolationMode.ToString(), new Font("Arial", 11.0f), Brushes.White, new PointF(15, 20));
							g.Flush();
							imgOut.Save(outStream, GetImageFormat(path));
							//throw new Exception("Testing");
							outBytes = outStream.ToArray();
						}
					}
				}
			}
			return outBytes;
		}

		/// <summary>
		/// Guess the content type based on the extension 
		/// of the original
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetContentType(String path)
		{
			switch(Path.GetExtension(path)) {
				case ".bmp": return "Image/bmp";
				case ".gif": return "Image/gif";
				case ".jpg": return "Image/jpeg";
				case ".png": return "Image/png";
				default: break;
			}
			return "";
		}


		/// <summary>
		/// Guess the image format based on the extension
		/// of the original
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static ImageFormat GetImageFormat(String path)
		{
			switch(Path.GetExtension(path)) {
				case ".bmp": return ImageFormat.Bmp;
				case ".gif": return ImageFormat.Gif;
				case ".jpg": return ImageFormat.Jpeg;
				case ".png": return ImageFormat.Png;
				default: break;
			}
			return ImageFormat.Jpeg;
		}
	}
}
