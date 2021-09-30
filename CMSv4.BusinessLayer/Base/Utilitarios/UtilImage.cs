using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CMSv4.BusinessLayer
{
    public class UtilImage
    {
        public static Image GenerateThumb(string imagemOriginal, int width, int heigth, bool crop = true)
        {
            if (crop)
                return ResizeAndCrop(imagemOriginal, width, heigth);

            return Resize(imagemOriginal, width, heigth);
        }

        public static Image Resize(string imagemOriginal, int width, int heigth)
        {
            var webImage = Image.FromFile(imagemOriginal);

            double aspectRatio = (double)webImage.Width / webImage.Height;
            heigth = Convert.ToInt32(width / aspectRatio);

            Image resizedImage = new Bitmap(width, heigth);

            using (var source = new Bitmap(webImage))
            {
                using (Graphics g = Graphics.FromImage(resizedImage))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.Clear(Color.White);
                    g.DrawImage(source, 0, 0, width, heigth);
                }
            }

            return resizedImage;
        }

        public static Image ResizeAndCrop(string imgToResize, int width, int heigth)
        {
            var imagem = Image.FromFile(imgToResize);

            var originalWidth = imagem.Width;
            var originalHeight = imagem.Height;

            //how many units are there to make the original length
            var hRatio = (float)originalHeight / heigth;
            var wRatio = (float)originalWidth / width;

            //Obter o menor lado
            var ratio = Math.Min(hRatio, wRatio);

            var hScale = Convert.ToInt32(heigth * ratio);
            var wScale = Convert.ToInt32(width * ratio);

            //Define o centro da imagem
            var startX = (originalWidth - wScale) / 2;
            var startY = (originalHeight - hScale) / 2;

            //Cortar imagem a partir do size espeficiado
            var sourceRectangle = new Rectangle(startX, startY, wScale, hScale);

            //Definir qual será o tamanho da imagem
            var bitmap = new Bitmap(width, heigth);

            //Preencer o bitmap
            var destinationRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(imagem, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        public static EncoderParameters GetCompressEncoder(int quality = 80)
        {
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            return parameters;
        }

        public static ImageCodecInfo GetCodecInfo(string mimeType = "image/jpeg")
        {
            foreach (ImageCodecInfo encoder in ImageCodecInfo.GetImageEncoders())
                if (encoder.MimeType == mimeType)
                    return encoder;
            throw new ArgumentOutOfRangeException(
                string.Format("'{0}' not supported", mimeType));
        }
    }
}
