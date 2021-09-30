using CMSv4.Model;
using Framework.Utilities;
using System;
using System.IO;
using System.Net;

namespace CMSv4.BusinessLayer
{
    public class Thumb
    {
        private ThumbResult Result { get; set; }
        private string Portal { get; set; }
        private string Modulo { get; set; }
        private string CodigoRegistro { get; set; }
        private int Width { get; set; }
        private int Heigth { get; set; }
        private string Imagem { get; set; }
        private string ImagemMapedPath { get; set; }
        private string ImagemThumbMapedPath { get; set; }
        private string Extension { get; set; }
        private string PhysicalDirectory { get; set; }
        private bool? Crop { get; set; }

        public Thumb(string portal, string modulo, string codigoRegistro, string width, string heigth, string imagem, bool? crop)
        {
            Portal = portal ?? BLPortal.Atual.Diretorio;
            Modulo = modulo;
            CodigoRegistro = codigoRegistro ?? "";
            Width = Convert.ToInt16(width);
            Heigth = Convert.ToInt16(heigth);
            Imagem = imagem;
            Crop = crop;
            Result = new ThumbResult()
            {
                StatusCode = HttpStatusCode.Continue,
                ContentType = ThumbSpupportedExtensions.keyValuePairs[".jpg"],
                File = HttpContextFactory.Current.Server.MapPath("~/Content/Site/img/blank.jpg")
            };
        }

        public ThumbResult Generate()
        {
            if (RequiredFieldsNotFilled())
                return GetResultRequiredFieldsNotFilled();

            if (OriginalFileNotFound())
                return GetResultOriginalFileNotFound();

            if (ImageFormatNotSupported())
                return GetResultFormartNotSupported();

            if(ThumbAlreadyExist())
                return GetResultThumbAlreadyExist();

            if(GenerateThumb())
                return GetResultThumb();

            return Result;
        }

        private bool RequiredFieldsNotFilled()
        {
            if (string.IsNullOrEmpty(Modulo) || string.IsNullOrEmpty(Imagem))
                return true;

            return false;
        }

        private ThumbResult GetResultRequiredFieldsNotFilled()
        {
            Result.StatusCode = HttpStatusCode.BadRequest;

            return Result;
        }

        private bool OriginalFileNotFound()
        {
            GetImageExtension();

            BindImagePath();

            if (!File.Exists(ImagemMapedPath))
                return true;

            return false;
        }

        private ThumbResult GetResultOriginalFileNotFound()
        {
            Result.StatusCode = HttpStatusCode.NotFound;

            return Result;
        }

        private void GetImageExtension()
        {
            Extension = Path.GetExtension(Imagem);
        }

        private void BindImagePath()
        {
            var virtualDirectory = BLConfiguracao.Pastas.ModuloGenerico(Portal, Modulo);

            if (!string.IsNullOrWhiteSpace(CodigoRegistro))
                virtualDirectory = Path.Combine(virtualDirectory, CodigoRegistro);

            PhysicalDirectory = HttpContextFactory.Current.Server.MapPath(virtualDirectory);

            ImagemMapedPath = Path.Combine(PhysicalDirectory, Imagem);
        }

        private bool ImageFormatNotSupported()
        {
            if (!ThumbSpupportedExtensions.keyValuePairs.ContainsKey(Extension))
                return true;

            return false;
        }

        private ThumbResult GetResultFormartNotSupported()
        {
            Result.StatusCode = HttpStatusCode.NotAcceptable;
            Result.ContentType = "image/*";
            Result.File = ImagemMapedPath;

            return Result;
        }

        private bool ThumbAlreadyExist()
        {
            GenerateFullPathToThumbFile();

            if (File.Exists(ImagemThumbMapedPath))
                return true;

            return false;
        }

        public ThumbResult GetResultThumbAlreadyExist()
        {
            Result.StatusCode = HttpStatusCode.OK;
            Result.ContentType = ThumbSpupportedExtensions.keyValuePairs[Extension];
            Result.File = ImagemThumbMapedPath;

            return Result;
        }

        private void GenerateFullPathToThumbFile()
        {
            var physicalThumbDirectory = Path.Combine(PhysicalDirectory, "_thumb");

            if (!Directory.Exists(physicalThumbDirectory))
                Directory.CreateDirectory(physicalThumbDirectory);

            ImagemThumbMapedPath = Path.Combine(PhysicalDirectory, "_thumb", string.Format("{0}-w{1}h{2}{3}{4}", Imagem, Width, Heigth, (Crop.HasValue && Crop.Value ? "cropped" : ""), Extension));
        }

        private bool GenerateThumb()
        {
            try
            {
                var thumbnail = UtilImage.GenerateThumb(ImagemMapedPath, Width, Heigth, Crop.GetValueOrDefault(true));

                thumbnail.Save(
                    ImagemThumbMapedPath,
                    UtilImage.GetCodecInfo(),
                    UtilImage.GetCompressEncoder()
                );

                thumbnail.Dispose();

                return true;
            }
            catch(Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }   
        }

        private ThumbResult GetResultThumb()
        {
            Result.StatusCode = HttpStatusCode.OK;
            Result.ContentType = ThumbSpupportedExtensions.keyValuePairs[Extension];
            Result.File = ImagemThumbMapedPath;

            return Result;
        }
    }
}
