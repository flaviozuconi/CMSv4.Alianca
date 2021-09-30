using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using System.Linq;
using System.Net.Mail;
using CMSv4.Model;
using CMSv4.BusinessLayer;
namespace CMSApp.Areas.Modulo.Controllers
{
    public class FaleConoscoController : ModuloBaseController<MLModuloFaleConoscoEdicao, MLModuloFaleConoscoHistorico, MLModuloFaleConoscoPublicado>
    {
        #region Index

        /// <summary>
        /// Área Pública / Apenas conteúdos publicados
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public override ActionResult Index(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                var portal = BLPortal.Atual;

                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var model = CRUD.Obter(new MLModuloFaleConoscoPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                if (model == null) model = new MLModuloFaleConoscoPublicado();

                ViewData["codigoPagina"] = codigoPagina;
                ViewData["repositorio"] = repositorio;

                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        #endregion

        #region Visualizar

        /// <summary>
        /// Área de Construção
        /// </summary>
        public override ActionResult Visualizar(decimal? codigoPagina, int? repositorio, bool? edicao, string codigoHistorico)
        {
            try
            {
                var portal = BLPortal.Atual;

                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var model = new MLModuloFaleConosco();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloFaleConoscoEdicao>(new MLModuloFaleConoscoEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloFaleConoscoHistorico>(new MLModuloFaleConoscoHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloFaleConoscoPublicado>(new MLModuloFaleConoscoPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null)
                {
                    var modelEdicao = new MLModuloFaleConoscoEdicao
                    {
                        CodigoPagina = codigoPagina,
                        Repositorio = repositorio,
                        NomeView = "Padrao",
                        NomeModelo = "Padrao",
                        Anonimo = false,
                        Titulo = "",
                        DataRegistro = DateTime.Now,
                        CodigoUsuario = BLUsuario.ObterLogado().Codigo
                    };

                    CRUD.Salvar<MLModuloFaleConoscoEdicao>(modelEdicao, portal.ConnectionString);

                    model = modelEdicao;
                }

                ViewData["codigoPagina"] = codigoPagina;
                ViewData["repositorio"] = repositorio;

                return PartialView("Index", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        #endregion

        #region Edição

        /// <summary>
        /// Editar
        /// </summary>
        public override ActionResult Editar(decimal? codigoPagina, int? repositorio, bool? edicao)
        {
            try
            {
                var portal = BLPortal.Atual;

                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var model = CRUD.Obter<MLModuloFaleConoscoEdicao>(new MLModuloFaleConoscoEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloFaleConoscoEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                var pastaModelos = BLConfiguracao.Pastas.ModuloFaleConoscoEmail(portal.Diretorio);
                var pastaFormularios = BLConfiguracao.Pastas.ModuloFaleConoscoForm(portal.Diretorio);

                List<string> lstModelos = new List<string>();
                List<string> lstFormularios = new List<string>();

                foreach (var item in Directory.GetFiles(Server.MapPath(pastaModelos), "*.htm"))
                {
                    if (item.IndexOf("_resposta", StringComparison.InvariantCultureIgnoreCase) == -1)
                        lstModelos.Add(Path.GetFileNameWithoutExtension(item));
                }

                foreach (var item in Directory.GetFiles(Server.MapPath(pastaFormularios), "*.cshtml"))
                {
                    //Scripts são gerados pelo sistemas e não devem ser exibidos para seleção
                    if(!System.IO.Path.GetFileName(item).StartsWith("Script"))
                        lstFormularios.Add(Path.GetFileNameWithoutExtension(item));
                }

                ViewData["modelos"] = lstModelos;
                ViewData["formularios"] = lstFormularios;

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        /// <summary>
        /// Salvar
        /// </summary>
        public override ActionResult Editar(MLModuloFaleConoscoEdicao model)
        {
            try
            {
                model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;
                model.DataRegistro = DateTime.Now;

                if (model.Anonimo == null)
                {
                    model.Anonimo = false;
                }

                if (model.EnviaEmail == null)
                {
                    model.EnviaEmail = false;
                }

                CRUD.Salvar(model, BLPortal.Atual.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Salvar

        /// <summary>
        /// Salvar
        /// </summary>
        [HttpPost, CheckPermission(global::Permissao.Publico)]
        public JsonResult Salvar(FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0) //validar extensao e tamanho do arquivo
                    {
                        bool isValid = true;

                        string[] extensoes_validas = { ".txt", ".doc", ".docx", ".pdf", ".jpg", ".wmv", ".mp4", ".mp3"};

                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            if (Request.Files[i].ContentLength > 9437184 || Array.IndexOf(extensoes_validas, Path.GetExtension(Request.Files[i].FileName).ToLower()) == -1)
                            {
                                isValid = false;
                                continue;
                            }
                        }

                        if (!isValid)
                        {
                            return Json(new { Sucesso = false, Mensagem = T("Os arquivos devem possuir extensão txt, doc, docx, pdf, jpg, wmv, mp4 ou mp3 e serem menores que 9MB") }, "text/html");
                        }
                    }

                    int repositorio = 0;
                    int ddd = 0;
                    decimal codigoPagina = 0;
                    decimal fone = 0;
                    decimal telefone = 0;
                    string prefixoassunto = "";
                    string fromconfig = null;
                    string _mensagem = T("E-mail enviado com sucesso!");

                    var outrosCampos = new XmlDocument();
                    var portal = BLPortal.Atual;
                    var destinatario = string.Empty;
                    var assunto = string.Empty;
                    var raiz = outrosCampos.CreateElement("faleconosco");
                    var xmlData = outrosCampos.CreateElement("data");
                    var xmlIP = outrosCampos.CreateElement("ip");

                    if (!string.IsNullOrEmpty(form["codigoPagina"]))
                        decimal.TryParse(form["codigoPagina"], out codigoPagina);

                    if (!string.IsNullOrEmpty(form["repositorio"]))
                        int.TryParse(form["repositorio"], out repositorio);

                    //obtem propriedades do módulo
                    var propriedades = CRUD.Obter(new MLModuloFaleConoscoPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                    //Em alguns casos, o módulo fale conosco pode ser chamado diretamente com Html.Action, nesse caso não teremos a propriedade do modulo
                    //Por isso é feita uma verificação para encontrar o nome do Formulário e do Modelo de Email dentro do FormCollaction
                    if(propriedades == null)
                    {
                        propriedades = new MLModuloFaleConoscoPublicado();
                        propriedades.NomeView = form["formulario"];
                        propriedades.EnviaEmail = true;
                    }

                    var formulario = CRUD.Obter(new MLFaleConoscoFormulario() { Nome = propriedades.NomeView, CodigoPortal = portal.Codigo }, portal.ConnectionString);

                    if (formulario == null) formulario = new MLFaleConoscoFormulario();

                    #region Definir assunto

                    if (!string.IsNullOrEmpty(form["assunto"])) //o formulario deve possuir um assunto
                    {
                        assunto = form["assunto"];
                        
                    }
                    else if(!string.IsNullOrWhiteSpace(formulario.Assunto))
                    {
                        assunto = formulario.Assunto;
                    }
                    else
                    {
                        return Json(new { Sucesso = false, Mensagem = T("Assunto não informado") }, "text/html");
                    }

                    #endregion

                    #region Definir destinatário

                    //O destinatário pode ser definido dentro do Form.
                    if (!string.IsNullOrWhiteSpace(form["destinatario"])) //o formulario deve possuir um destinatario
                    {
                        destinatario = BLEncriptacao.DesencriptarAes(form["destinatario"]);
                    }
                    //Se for novo formulário, utilizar as informações salvas na base de dados.
                    else if (!string.IsNullOrWhiteSpace(formulario.EmailDestinatario))
                    {
                        destinatario = formulario.EmailDestinatario;
                    }
                    //Não tem destinatario, não pode enviar e-mail.
                    else
                    {
                        return Json(new { Sucesso = false, Mensagem = T("Destinatário não informado") }, "text/html");
                    }

                    #endregion

                    xmlData.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    xmlIP.InnerText = ObterIPUsuario();

                    outrosCampos.AppendChild(raiz);

                    raiz.AppendChild(xmlData);
                    raiz.AppendChild(xmlIP);

                    if (!string.IsNullOrEmpty(form["ddd"]))
                        int.TryParse(form["ddd"].Replace("(", "").Replace(")", ""), out ddd);

                    if (!string.IsNullOrEmpty(form["fone"]))
                        decimal.TryParse(form["fone"].Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""), out fone);

                    if (!string.IsNullOrEmpty(form["telefone"]))
                        decimal.TryParse(form["telefone"].Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""), out telefone);

                    if (!string.IsNullOrEmpty(form["prefixoassunto"]))
                        prefixoassunto = form["prefixoassunto"].ToString();

                      if (!string.IsNullOrEmpty(form["fromconfig"]))
                        fromconfig = form["fromconfig"].ToString();


                    //obtem html de email para envio

                    #region Se o arquivo não existir, criar

                    var modeloEmailArquivo = Server.MapPath(BLConfiguracao.Pastas.ModuloFaleConoscoEmail(portal.Diretorio) + "/" + form["modelo"] + ".htm");

                    if(!System.IO.File.Exists(modeloEmailArquivo))
                    {
                        var modeloEmail = CRUD.Obter(new MLFaleConoscoModeloEmail() { Nome = form["modelo"], CodigoPortal = portal.Codigo }, portal.ConnectionString);

                        if(modeloEmail != null && !string.IsNullOrWhiteSpace(modeloEmail.Conteudo))
                        {
                            using (var arquivo = new StreamWriter(modeloEmailArquivo))
                            {
                                arquivo.Write(modeloEmail.Conteudo.Unescape());
                                arquivo.Close();
                            }
                        }
                    }

                    #endregion

                    var email = BLUtilitarios.ObterConteudoArquivo(BLConfiguracao.Pastas.ModuloFaleConoscoEmail(portal.Diretorio) + "/" + form["modelo"] + ".htm");
                    
                    //caso o modelo possua um email de resposta este será enviado para o remetente
                    var emailResposta = BLUtilitarios.ObterConteudoArquivo(BLConfiguracao.Pastas.ModuloFaleConoscoEmail(portal.Diretorio) + "/" + form["modelo"] + "_resposta.htm");

                    #region Preencher campos email
                    foreach (var item in form.AllKeys)
                    {
                        int t = 0;
                        if (Int32.TryParse(item,out t))
                            continue;
                        if (item == "arquivo[]") //as chaves [] dão erro aqui
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(email))
                        {
                            email = email.Replace("%%" + item + "%%", form[item]);
                        }
                        else
                        {
                            email += item.ToLower() + ":" + form[item] + "<br/>";
                        }

                        var xmlItem = outrosCampos.CreateElement(item);

                        xmlItem.InnerText = form[item];

                        raiz.AppendChild(xmlItem);
                    }

                    email = Regex.Replace(email, "%%.*%%", "");
                    #endregion
                    
                    #region Gravar na base de dados
                    //somente entrar quando Anonimo for igual a 'false'
                    if (!propriedades.Anonimo.GetValueOrDefault())
                    {
                        CRUD.Salvar(new MLFaleConoscoPadrao
                                    {
                                        CodigoPortal = portal.Codigo,
                                        DataCadastro = DateTime.Now,
                                        Nome = form["nome"],
                                        Email = form["email"],
                                        Mensagem = form["mensagem"],
                                        Assunto = assunto,
                                        Destinatario = destinatario,
                                        DDD = ddd,
                                        Fone = fone > 0 ? fone : telefone,
                                        XML = Beautify(outrosCampos),
                                        NomeView = form["arquivo"],
                                        CodigoPagina = codigoPagina,
                                        Repositorio = repositorio
                                    }, portal.ConnectionString);
                    }
                    #endregion

                    #region Email para Admin configurado no layout do formulário
                    //somente entrar quando EnviaEmail for igual a 'true'
                    if (propriedades.EnviaEmail.GetValueOrDefault())
                    {
                        var enviarPara = new List<string>();

                        enviarPara.AddRange(destinatario.Split(','));

                        foreach (var item in form.AllKeys)
                        {
                            email = email.Replace("[" + item.ToUpper() + "]", form[item].ToUpper());
                            email = email.Replace("%%" + item.ToUpper() + "%%", form[item].ToUpper());
                        }

                        email = Regex.Replace(email, @"\[(.*?)\]", "-");

                        BLEmail.Enviar(
                            (string.IsNullOrEmpty(prefixoassunto) ? portal.Nome : prefixoassunto) + " | " + assunto + " | " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                            enviarPara,
                            (!string.IsNullOrWhiteSpace(formulario.EmailsCopia) ? formulario.EmailsCopia.Split(',').ToList() : null),
                            null,
                            email,
                            Request.Files,
                            sendReplyTo: form["email"],
                            throwException: true,useDefaultConfigFrom:fromconfig);
                    }
                    
                    #endregion

                    #region Email para remetente do formulário
                    string destinatarioResposta = form["email"];

                    //somente entrar quando EnviaEmail for igual a 'true'
                    //existir no diretório um arquivo "*_RESPOSTA.html"
                    //possuir email de quem enviou o formulário
                    if (propriedades.EnviaEmail.GetValueOrDefault() && !string.IsNullOrEmpty(emailResposta) && !string.IsNullOrEmpty(destinatarioResposta))
                    {
                        foreach (var item in form.AllKeys)
                        {
                            emailResposta = emailResposta.Replace("%%" + item + "%%", form[item]);
                        }

                        emailResposta = Regex.Replace(emailResposta, "%%.*%%", "");

                        BLEmail.Enviar(portal.Nome + " | " + assunto, destinatarioResposta, emailResposta, sendReplyTo: "", throwException: true, useDefaultConfigFrom: fromconfig);
                    }
                    #endregion

                    return Json(new { Sucesso = true, Mensagem = T(_mensagem) }, "text/html");
                }

                return Json(new { Sucesso = false, Mensagem = T("Erro no formulário") }, "text/html");
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { Sucesso = false, Mensagem = ex.Message }, "text/html");
            }
        }
        #endregion

        #region Sucesso

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Sucesso()
        {
            return PartialView("Sucesso");
        }

        #endregion

        #region Erro
        
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Erro()
        {
            return PartialView("Erro");
        }

        #endregion

        #region Script

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloFaleConosco model)
        {
            try
            {
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Renderizar

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Renderizar(MLModuloFaleConosco model)
        {
            var pasta = BLConfiguracao.Pastas.ModuloFaleConoscoForm(BLPortal.Atual.Diretorio);
            var portal = BLPortal.Atual;
            var modelFormulario = CRUD.Obter(new MLFaleConoscoFormulario() { Nome = model.NomeView, CodigoPortal = portal.Codigo }, portal.ConnectionString);

            ViewData["modelo"] = model.NomeModelo;
            ViewData["codigoPagina"] = model.CodigoPagina;
            ViewData["repositorio"] = model.Repositorio;

            var arquivo = Path.Combine(pasta, model.NomeView + ".cshtml");
            var arquivoFisico = Server.MapPath(arquivo);

            #region Script

            var arquivoScript = Path.Combine(pasta, "Script" + model.NomeView + ".cshtml");
            var arquivoScriptFisico = Server.MapPath(arquivoScript);

            if (!Directory.Exists(Server.MapPath(pasta)))
                Directory.CreateDirectory(Server.MapPath(pasta));

            //Utilizar script personalizado
            if (modelFormulario != null && !string.IsNullOrWhiteSpace(modelFormulario.Script))
            {
                if (!System.IO.File.Exists(arquivoScriptFisico))
                {
                    try
                    {
                        // Salvar no disco
                        using (var f = System.IO.File.CreateText(arquivoScriptFisico))
                        {
                            f.Write(modelFormulario.Script.Unescape());
                            f.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        ApplicationLog.ErrorLog(ex);
                    }
                }

                var script = BLConteudoHelper.RenderViewToString(this, arquivoScript, string.Empty, model);
                BLConteudo.AdicionarJavaScript(script, false);
            }
            //Utilizar default se o formulário não tiver script customizado
            else
            {
                var script = BLConteudoHelper.RenderPartialViewToString(this, "Modulo", "FaleConosco", "Script", model);
                BLConteudo.AdicionarJavaScript(script, false);
            }

            #endregion

            if (System.IO.File.Exists(arquivoFisico))
                return View(arquivo);

            if(modelFormulario != null && !string.IsNullOrWhiteSpace(modelFormulario.Conteudo))
            {
                try
                {
                    // Salvar no disco
                    using (var f = new StreamWriter(arquivoFisico))
                    {
                        f.Write(modelFormulario.Conteudo.Unescape());
                        f.Close();
                    }

                    return View(arquivo);
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                }
            }

            return View();
        }

        #endregion

        private string ObterIPUsuario()
        {
            var context = System.Web.HttpContext.Current;
            var ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');

                if (addresses.Length != 0)
                    return addresses[0];
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        private string Beautify(XmlDocument doc)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };

                using (XmlWriter writer = XmlWriter.Create(sb, settings))
                {
                    doc.Save(writer);
                }

                return sb.ToString();
            }
            catch
            {
                return doc.InnerXml;
            }
        }
    }
}
