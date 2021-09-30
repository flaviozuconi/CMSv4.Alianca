using CMSv4.Model;

namespace CMSv4.BusinessLayer
{
    public class ThumbBuilder
    {
        private string PortalValue { get; set; }
        private string ModuloValue { get; set; }
        private string CodigoRegistroValue { get; set; }
        private string WidthValue { get; set; }
        private string HeigthValue { get; set; }
        private string ImagemValue { get; set; }
        private bool? CropValue { get; set; }

        public ThumbBuilder Portal(string portalValue)
        {
            PortalValue = portalValue;
            return this;
        }

        public ThumbBuilder Modulo(string moduloValue)
        {
            ModuloValue = moduloValue;
            return this;
        }

        public ThumbBuilder CodigoRegistro(string codigoRegistroValue)
        {
            CodigoRegistroValue = codigoRegistroValue;
            return this;
        }

        public ThumbBuilder Width(string widthValue)
        {
            WidthValue = widthValue;
            return this;
        }

        public ThumbBuilder Width(int widthValue)
        {
            WidthValue = widthValue.ToString();
            return this;
        }

        public ThumbBuilder Heigth(string heigthValue)
        {
            HeigthValue = heigthValue;
            return this;
        }

        public ThumbBuilder Heigth(int heigthValue)
        {
            HeigthValue = heigthValue.ToString();
            return this;
        }

        public ThumbBuilder Imagem(string imagemValue)
        {
            ImagemValue = imagemValue;
            return this;
        }

        public ThumbBuilder Crop(bool cropValue)
        {
            CropValue = cropValue;
            return this;
        }

        public ThumbResult Generate()
        {
            return new Thumb(PortalValue, ModuloValue, CodigoRegistroValue, WidthValue, HeigthValue, ImagemValue, CropValue)
                .Generate();
        }
    }
}
