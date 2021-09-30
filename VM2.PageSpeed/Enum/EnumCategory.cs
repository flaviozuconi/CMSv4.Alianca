namespace VM2.PageSpeed
{
    public enum EnumCategory
    {
        Accessibility,
        BestPractices,
        Performance,
        Pwa,
        Seo,
    }

    public static class EnumCategoryExntesion
    {
        public static string ToQueryStringParameter(this EnumCategory category)
        {
            var retorno = "";

            switch (category)
            {
                case EnumCategory.Accessibility:
                    retorno = "accessibility";
                    break;
                case EnumCategory.BestPractices:
                    retorno = "best-practices";
                    break;
                case EnumCategory.Performance:
                    retorno = "performance";
                    break;
                case EnumCategory.Pwa:
                    retorno = "pwa";
                    break;
                case EnumCategory.Seo:
                    retorno = "seo";
                    break;
            }

            return retorno;
        }
    }
}
