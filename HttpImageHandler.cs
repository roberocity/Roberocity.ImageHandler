using System.Web;

namespace Roberocity.ImageHandler
{
	using System;
	using System.IO;

	public static class GuidHelpers
	{
		public static Guid TryCreate(string g)
		{
			var ng = Guid.Empty;

			try {
				ng = new Guid(g);
			}
			catch(Exception) {
			}

			return ng;
		}
	}

	/// <summary>
	/// Resizes an image based on the querystring paramenters of Height and Width. 
	/// If no parameters are given then it just returns the image. 
	/// The image will always retain the proporitions of the original
	/// image. The height and width of the image returned may not match the height and
	/// width specified. The image will be scaled based on the smallest side.
	/// </summary>
	public class HttpImageHandler : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "image/png";

			var id = context.Request.QueryString["id"];
			
			byte[] buffer;

			var imgId = GuidHelpers.TryCreate(id);

			if(imgId == Guid.Empty)
			{

				id = string.IsNullOrEmpty(id) ? context.Request.PhysicalPath : context.Server.MapPath(id);

				if(ShouldResizeImage(context)) {
					int h;
					int w;
					int.TryParse(context.Request.QueryString["Width"], out w);
					int.TryParse(context.Request.QueryString["Height"], out h);
					buffer = Roberocity.ImageEditing.GetResizedImage(id, w, h);
				}
				else {
					buffer = File.ReadAllBytes(id);
				}
			}
			else {
				var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\pressreleases");
				if(ShouldResizeImage(context)) {
					int h;
					int w;
					int.TryParse(context.Request.QueryString["Width"], out w);
					int.TryParse(context.Request.QueryString["Height"], out h);
					buffer = Roberocity.ImageEditing.GetResizedImage(Path.Combine(basePath, id), w, h);
				}
				else {
					Roberocity.Storage.IFileStore storage = new Roberocity.Storage.DiskFileStore(basePath);
					using(var stream = storage.Get(imgId)) {
						buffer = new byte[stream.Length];
						stream.Read(buffer, 0, buffer.Length);
					}
				}
			}
			context.Response.OutputStream.Write(buffer, 0, buffer.Length);
		}

		static bool ShouldResizeImage(HttpContext context)
		{
			return !(string.IsNullOrEmpty(context.Request.QueryString["Width"]) && string.IsNullOrEmpty(context.Request.QueryString["Height"]));
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}