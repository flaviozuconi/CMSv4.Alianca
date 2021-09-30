using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace CMSApp.Helpers
{
    /// <summary>
    /// Retorna uma imagem de placeholder no tamanho solicitado
    /// </summary>
    public class ImagePlaceHolder : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int w;
            int h;
            if (!int.TryParse(context.Request["w"], out w)) w = 100;
            if (!int.TryParse(context.Request["h"], out h)) h = 100;

            var picture = new System.Drawing.Bitmap(w, h);
            var paint = Graphics.FromImage(picture);
            var pen = new Pen(Color.White);

            // fundo
            paint.FillRectangle(Brushes.LightGray, 0, 0, w, h);

            // linhas
            paint.DrawLine(pen, 0, 0, w, h);
            paint.DrawLine(pen, w, 0, 0, h);

            // corte das linhas no centro
            var innerRectangle = new Rectangle((int)Math.Floor(w * 0.2), (int)Math.Floor(h * 0.2), (int)Math.Floor(w * 0.6), (int)Math.Floor(h * 0.6));
            paint.FillRectangle(Brushes.LightGray, innerRectangle);

            // circulo
            var ratio = innerRectangle.Height < innerRectangle.Width ? innerRectangle.Height : innerRectangle.Width;
            ratio = (int)Math.Floor(ratio * 0.8);
            var circleRectangle = new Rectangle((int)(int)Math.Floor((w - ratio)/2d), (int)Math.Floor((h - ratio)/2d), ratio, ratio);
            paint.DrawEllipse(pen, circleRectangle);

            MemoryStream mem = new MemoryStream();
            picture.Save(mem, ImageFormat.Jpeg);

            context.Response.Clear();
            context.Response.ContentType = "image/jpeg";
            context.Response.StatusCode = 200;
            context.Response.BinaryWrite(mem.ToArray());
            context.Response.Flush();

        }

        private void CompressAndSaveImage(Image img, string fileName, long quality)
        {
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            img.Save(fileName, GetCodecInfo("image/jpeg"), parameters);
        }

        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            foreach (ImageCodecInfo encoder in ImageCodecInfo.GetImageEncoders())
                if (encoder.MimeType == mimeType)
                    return encoder;
            throw new ArgumentOutOfRangeException(
                string.Format("'{0}' not supported", mimeType));
        }

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return false; }
        }

    }
}
