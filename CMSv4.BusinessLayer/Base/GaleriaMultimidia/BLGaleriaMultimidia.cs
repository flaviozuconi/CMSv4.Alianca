using CMSv4.Model;
using Framework.Utilities;
using System;
using System.IO;
using System.Web;

namespace CMSv4.BusinessLayer
{
    public class BLGaleriaMultimidia : BLCRUD<MLGaleriaMultimidia>
    {

        public override decimal Salvar(MLGaleriaMultimidia model, string connectionString = "")
        {
            var portal = PortalAtual.Obter;
            var codigoUsuarioLogado = BLUsuario.ObterLogado().Codigo;

            model.Ativo = model.Ativo.GetValueOrDefault(false);
            model.CodigoPortal = !model.CodigoPortal.HasValue ? portal.Codigo : model.CodigoPortal;
            model.LogDataCadastro = model.LogDataCadastro.GetValueOrDefault(DateTime.Now);
            model.LogUsuarioCadastro = !model.LogUsuarioCadastro.HasValue ? codigoUsuarioLogado : model.LogUsuarioCadastro;

            if (model.Codigo.HasValue)
            {
                model.LogDataAlteracao = DateTime.Now;
                model.LogUsuarioAlteracao = codigoUsuarioLogado;
            }

            return base.Salvar(model, connectionString);
        }
    }

    public class BLGaleriaMultimidiaArquivo : BLCRUD<MLGaleriaMultimidiaArquivo>
    {

        public MLGaleriaMultimidiaArquivo Obter(decimal? id, string CodigoGaleria, string CodigoIdioma)
        {
            var portal = PortalAtual.Obter;
            var model = new MLGaleriaMultimidiaArquivo();

            if (id.HasValue)
            {
                model = Obter(id.Value, portal.ConnectionString);
            }
            else
            {
                model = new MLGaleriaMultimidiaArquivo();
                model.CodigoIdioma = PortalAtual.Obter.CodigoIdioma;
            }

            if (!string.IsNullOrWhiteSpace(CodigoGaleria))
                model.CodigoGaleria = Convert.ToDecimal(CodigoGaleria);

            if (!model.CodigoIdioma.HasValue && !string.IsNullOrWhiteSpace(CodigoIdioma))
                model.CodigoIdioma = Convert.ToDecimal(CodigoIdioma);

            if (!model.Data.HasValue)
                model.Data = DateTime.Now;

            return model;
        }

        public MLGaleriaMultimidiaArquivo Salvar(MLGaleriaMultimidiaArquivo model, HttpPostedFileBase imgThumb, bool? RemoverImg)
        {

            var portal = PortalAtual.Obter;

            var diretorioVirtual = $"{BLConfiguracao.Pastas.ModuloGaleria(portal.Diretorio)}/{model.CodigoGaleria}";
            var diretorio = HttpContext.Current.Server.MapPath(diretorioVirtual);


            if (!string.IsNullOrEmpty(model.Arquivo) &&
                !File.Exists(Path.Combine(diretorio, model.Arquivo.Replace("/", ""))))
                throw new Exception("Falha ao realizar upload do arquivo!");

            model.LogPreencher(model.Codigo.HasValue);
            model.Destaque = model.Destaque.GetValueOrDefault(false);

            if (!Directory.Exists(Path.Combine(diretorio, "thumb")))
                Directory.CreateDirectory(Path.Combine(diretorio, "thumb"));

            #region Obter imagem de miniatura através do upload
            if ((imgThumb?.ContentLength ?? 0) > 0 || RemoverImg.GetValueOrDefault())
            {
                /// Excluir a imagem antiga
                if (!string.IsNullOrEmpty(model.Imagem))
                {
                    var info = new FileInfo(Path.Combine(diretorio, "thumb", model.Imagem));

                    if (!string.IsNullOrEmpty(model.Imagem) && info.Exists)
                    {
                        try { info.Delete(); }
                        catch { }

                        BLReplicar.ExcluirArquivosReplicados($"{BLConfiguracao.Pastas.ModuloGaleria(portal.Diretorio)}/{model.CodigoGaleria}/thumb"
                                                            , Path.Combine(diretorio, "thumb", model.Imagem));
                    }
                }

                if ((imgThumb?.ContentLength ?? 0) > 0)
                {
                    model.Imagem = imgThumb.FileName;
                }
                else
                {
                    model.Imagem = null;
                }
            }
            #endregion

            #region Obter imagem de miniatura do youtube
            if (string.IsNullOrEmpty(model.Imagem) && imgThumb == null && !string.IsNullOrEmpty(model.YouTube))
            {
                try
                {
                    string url = model.YouTube;
                    var match = System.Text.RegularExpressions.Regex.Match(url, @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");

                    if (match.Success)
                    {
                        using (var client = new System.Net.WebClient())
                        {
                            string videoId = match.Groups[1].Value;

                            client.DownloadFile(new Uri(string.Format("http://img.youtube.com/vi/{0}/mqdefault.jpg", videoId)), Path.Combine(diretorio, "thumb", videoId + ".jpg"));
                            model.Imagem = string.Concat(videoId, ".jpg");
                        }

                        BLReplicar.Arquivo(Path.Combine(diretorio, "thumb", model.Imagem), Path.Combine(diretorioVirtual, "thumb"));
                    }
                }
                catch
                {
                }
            }
            #endregion

            model.Codigo = Salvar(model, portal.ConnectionString);

            if (imgThumb != null && imgThumb.ContentLength > 0)
            {
                imgThumb.SaveAs(Path.Combine(diretorio, "thumb", imgThumb.FileName));
                BLReplicar.Arquivo(Path.Combine(diretorio, "thumb", imgThumb.FileName), string.Concat(BLConfiguracao.Pastas.ModuloGaleria(portal.Diretorio), "/", model.CodigoGaleria, "/thumb"));
            }

            return model;
        }

        public HttpPostedFileBase UploadFile(MLGaleriaMultimidiaArquivo model, HttpPostedFileBase file, HttpFileCollectionBase files)
        {
            HttpPostedFileBase arquivo = null;
            var portal = PortalAtual.Obter;
            var diretorio = HttpContext.Current.Server.MapPath($"{BLConfiguracao.Pastas.ModuloGaleria(portal.Diretorio)}/{model.CodigoGaleria}");

            if (model.CodigoGaleria.HasValue)
            {
                var modelCategoria = Obter(model.CodigoGaleria.Value, portal.ConnectionString);

                if (!Directory.Exists(diretorio)) Directory.CreateDirectory(diretorio);

                for (int i = 0; i < files.Count; i++)
                {
                    arquivo = files[i];

                    if ((arquivo?.ContentLength ?? 0) > 0)
                    {
                        arquivo.SaveAs(Path.Combine(diretorio, Path.GetFileName(arquivo.FileName)));
                        BLReplicar.Arquivo(Path.Combine(diretorio, Path.GetFileName(arquivo.FileName)), $"{BLConfiguracao.Pastas.ModuloGaleria(portal.Diretorio)}/{model.CodigoGaleria}");
                    }
                }
            }
            else
                throw new Exception("Pasta não encontrada");

            return arquivo;
        }

    }

    public class BLGaleriaMultimidiaTipo : BLCRUD<MLGaleriaMultimidiaTipo> { }

}
