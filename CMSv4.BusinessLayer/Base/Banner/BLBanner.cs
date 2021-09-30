using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace CMSv4.BusinessLayer
{
    public class BLBanner : BLCRUD<MLBanner>
    {
        #region Excluir

        public override int Excluir(List<string> ids, string connectionString = "")
        {
            var retorno = base.Excluir(ids, connectionString);

            foreach (var codigoBanner in ids)
            {
                var id = Convert.ToDecimal(codigoBanner);
                var diretorioVirtualBanner = ObterDiretorio(id);

                var diretorioFisicoBanner = HttpContextFactory.Current.Server.MapPath(diretorioVirtualBanner);
                var diretorioFisicoBannerInfo = new DirectoryInfo(diretorioFisicoBanner);

                var diretorioFisicoBannerThumb = HttpContextFactory.Current.Server.MapPath(Path.Combine(diretorioVirtualBanner, "_thumb"));
                var diretorioFisicoBannerThumbInfo = new DirectoryInfo(diretorioFisicoBanner);

                foreach (var file in diretorioFisicoBannerThumbInfo.GetFiles())
                {
                    try
                    {
                        file.Delete();
                        BLReplicar.ExcluirArquivosReplicados(file.FullName);
                    }
                    catch { }
                }

                foreach (var file in diretorioFisicoBannerInfo.GetFiles())
                {
                    try
                    {
                        file.Delete();
                        BLReplicar.ExcluirArquivosReplicados(file.FullName);
                    }
                    catch  {}
                }

                try
                {
                    Directory.Delete(diretorioFisicoBanner, true);
                    BLReplicar.ExcluirDiretoriosReplicados(diretorioVirtualBanner);
                }
                catch {}
            }

            return retorno;
        }

        #endregion

        #region Obter Diretório Banner

        /// <summary>
        /// Normalizar o diretório utilizado para salvar arquivos no módulo banner
        /// </summary>
        /// <param name="CodigoBanner"></param>
        /// <returns></returns>
        public static string ObterDiretorio(decimal CodigoBanner)
        {
            return string.Concat(BLConfiguracao.Pastas.ModuloBanner(PortalAtual.Obter.Diretorio), "/", CodigoBanner).Replace("//", "/");
        }

        #endregion

        #region Salvar Parcial

        public override decimal SalvarParcial(MLBanner model, string connectionString = "")
        {
            var portal = PortalAtual.Obter;

            try
            {
                model.Ativo = model.Ativo.GetValueOrDefault(false);
                model.CodigoPortal = portal.Codigo;
                model.Codigo = base.SalvarParcial(model, portal.ConnectionString);

                if (model.Codigo.HasValue)
                    BLCachePortal.LimparCache(string.Format("banner-{0}-{1}", model.Codigo, BLIdioma.CodigoAtual));

                return model.Codigo.GetValueOrDefault(0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
