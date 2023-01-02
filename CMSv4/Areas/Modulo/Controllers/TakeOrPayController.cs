using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;
using CMSv4.Model.Base;
using CMSv4.BusinessLayer;
using System.Globalization;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class TakeOrPayController : ModuloBaseController<MLModuloTakeOrPayEdicao, MLModuloTakeOrPayHistorico, MLModuloTakeOrPayPublicado>
    {
        #region Index
        /// <summary>
        /// Área Pública / Apenas conteúdos publicados
        /// </summary>
        public override ActionResult Index(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                // Visualizar Publicado
                var model = CRUD.Obter<MLModuloTakeOrPayPublicado>(new MLModuloTakeOrPayPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);

                return PartialView(model);
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
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var portal = PortalAtual.Obter;
                var model = CRUD.Obter<MLModuloTakeOrPayEdicao>(new MLModuloTakeOrPayEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloTakeOrPayEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                ViewData["modelos"] = CRUD.Listar(new MLFaleConoscoModeloEmail());

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
        public override ActionResult Editar(MLModuloTakeOrPayEdicao model)
        {
            try
            {
                //model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;
                model.DataRegistro = DateTime.Now;

                CRUD.Salvar(model, PortalAtual.Obter.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Excluir

        /// <summary>
        /// Excluir
        /// </summary>
        public override ActionResult Excluir(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                CRUD.Excluir<MLModuloTakeOrPayEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.Obter.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
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
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var portal = PortalAtual.Obter;
                var model = new MLModuloTakeOrPay();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloTakeOrPayEdicao>(new MLModuloTakeOrPayEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (model == null) model = new MLModuloTakeOrPay { CodigoPagina = codigoPagina, Repositorio = repositorio };
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloTakeOrPayHistorico>(new MLModuloTakeOrPayHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloTakeOrPayPublicado>(new MLModuloTakeOrPayPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null || (model != null && !model.DataRegistro.HasValue))
                {
                    var usuario = BLUsuario.ObterLogado();

                    model = new MLModuloTakeOrPayEdicao();
                    model.Email = string.Empty;
                    model.Link = string.Empty;
                    model.Repositorio = repositorio;
                    model.CodigoPagina = codigoPagina;
                    model.DataRegistro = DateTime.Now;

                    if (usuario != null)
                    {
                        //model.CodigoUsuario = usuario.Codigo;
                    }

                    CRUD.Salvar(model as MLModuloTakeOrPayEdicao, portal.ConnectionString);
                }

                return PartialView("index", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        #endregion

        //publico

        #region isValidNumeroProposta
        /// <summary>
        /// Verifica se a proposta existe
        /// </summary>
        /// <param name="numeroProposta"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult isValidNumeroProposta(decimal? numeroProposta)
        {
            return new JsonResult() { Data = new { Sucess = CRUD.Obter(new MLProgramacaoProposta { NumeroProposta = numeroProposta }) != null } };
        }
        #endregion

        #region SalvarEtapa2
        /// <summary>
        /// Salva a etapa 2
        /// </summary>
        /// <param name="lstNumeroProposta"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult SalvarEtapa2(string lstNumeroProposta, MLTakeOrPayEmbarqueCerto model)
        {
            try
            {
                model.DataCadastro = DateTime.Now;

                var codigoEtapa2 = CRUD.SalvarParcial(model);

                CRUD.Excluir(new MLTakeOrPayEmbarqueCertoXProposta { CodigoTakeOrPayEmbarqueCerto = codigoEtapa2 });

                foreach (var item in string.IsNullOrEmpty(lstNumeroProposta) ? new List<string>() : lstNumeroProposta.Trim(',').Split(',').ToList())
                {
                    if (item != null)
                    {
                        CRUD.Salvar(new MLTakeOrPayEmbarqueCertoXProposta
                        {
                            DataCadastro = DateTime.Now,
                            CodigoTakeOrPayEmbarqueCerto = codigoEtapa2,
                            NumeroProposta = decimal.Parse(item)
                        });
                    }
                }

                return new JsonResult() { Data = new { Sucess = true, Codigo = codigoEtapa2 } };
            }
            catch(Exception ex)
            {
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion

        #region SalvarEtapa3
        /// <summary>
        /// Salva a Etapa 3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult SalvarEtapa3(MLTakeOrPayEmbarqueCerto model)
        {
            try
            {
                return new JsonResult() { Data = new { Sucess = CRUD.SalvarParcial(model) > 0 } };
            }
            catch
            {
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion

        #region AdicionarContainer
        /// <summary>
        /// Adicionar Container
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult AdicionarContainer(MLTakeOrPayEmbarqueCertoXContainers model)
        {
            try
            {
                var POD = BLTakeOrPay.CarregarValorAdicional(model.PortoDestino);

                model.TarifaAdicional = POD?.TarifaAdicional;
                model.ValorTarifa = POD?.ValorTarifa;
                model.Penalidade = POD?.Penalidade;
                model.ValorPenalidade = POD?.ValorPenalidade;

                return new JsonResult() { Data = new { Sucess = true, Codigo = CRUD.Salvar(model) }};
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion

        #region AdicionarContainerHistorico
        /// <summary>
        /// Adicionar Container
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult AdicionarContainerHistorico(MLTakeOrPayEmbarqueCertoXContainersHistorico model)
        {
            try
            {
                return new JsonResult() { Data = new { Sucess = true, Codigo = CRUD.Salvar(model) } };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion

        #region ExcluirContainer
        /// <summary>
        /// Excluir Container
        /// </summary>
        /// <param name="CodigoTabela"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ExcluirContainer(decimal? CodigoTabela)
        {
            try
            {
                return new JsonResult() { Data = new { Sucess = CRUD.Excluir(new MLTakeOrPayEmbarqueCertoXContainers { Codigo = CodigoTabela }) > 0 } };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion

        #region ExcluirContainerHistorico
        /// <summary>
        /// Excluir Container
        /// </summary>
        /// <param name="CodigoTabela"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ExcluirContainerHistorico(decimal? CodigoTabela)
        {
            try
            {
                return new JsonResult() { Data = new { Sucess = CRUD.Excluir(new MLTakeOrPayEmbarqueCertoXContainersHistorico { Codigo = CodigoTabela }) > 0 } };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion

        #region ExcluirTodosContainers
        /// <summary>
        /// Excluir todos os containers
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ExcluirTodosContainers(decimal? Codigo)
        {
            try
            {
                return new JsonResult() { Data = new { Sucess = CRUD.Excluir(new MLTakeOrPayEmbarqueCertoXContainers { CodigoEmbarqueCerto = Codigo }) > 0 } };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion

        #region ListarNaviosAutocomplete
        /// <summary>
        /// Listar Navios Autocomplete
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ListarNaviosAutocomplete(bool? isTipoReserva)
        {
            try
            {
                var retorno = new List<string> { "<option value=\"\">Selecione</option>" };

                var lista = BLTakeOrPay.ListarNavioViagem();

                if (isTipoReserva == true)
                    retorno.AddRange(lista.Select(x => $"<option value=\"{x.NavioViagem?.Split('/')[0].Trim()}\">{x.NavioViagem?.Split('/')[0].Trim()}</option>").Distinct(StringComparer.CurrentCultureIgnoreCase));
                else
                    retorno.AddRange(lista.Select(x => $"<option value=\"{x.NavioViagem.Trim()}\">{x.NavioViagem.Trim()}</option>").Distinct(StringComparer.CurrentCultureIgnoreCase));

                return new JsonResult() { Data = new { Sucess = true, autocomplete = retorno } };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion

        #region CarregarPortos
        /// <summary>
        /// CarregarPortos
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult CarregarPortos()
        {
            try
            {
                var list = CRUD.Listar(new MLProgramacaoNavio());
                var portoOrigem = BLTakeOrPay.FormarOption(list.Select(x => x.Origem.Trim())?.ToList());
                var portoDestino = BLTakeOrPay.FormarOption(list.Select(x => x.Destino.Trim())?.ToList());
                
                return new JsonResult() { Data = new { Sucess = true, PortoOrigem = portoOrigem, PortoDestino = portoDestino } };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }

        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult CarregarPortosDestino(string PortoOrigem)
        {
            try
            {
                var portoDestino = new List<string>();

                if(string.IsNullOrEmpty(PortoOrigem))
                    portoDestino = BLTakeOrPay.FormarOption(CRUD.Listar(new MLProgramacaoNavio())?.Select(x => x.Destino.Trim()).ToList());
                else
                    portoDestino = BLTakeOrPay.FormarOption(CRUD.Listar(new MLProgramacaoNavio { Origem = PortoOrigem })?.Select(x => x.Destino.Trim()).ToList());

                return new JsonResult() { Data = new { Sucess = true, PortoDestino = portoDestino } };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }

        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult CarregarPortosOrigem(string PortoDestino)
        {
            try
            {
                var portoOrigem = new List<string>();

                if (string.IsNullOrEmpty(PortoDestino))
                    portoOrigem = BLTakeOrPay.FormarOption(CRUD.Listar(new MLProgramacaoNavio())?.Select(x => x.Origem.Trim()).ToList());
                else
                    portoOrigem = BLTakeOrPay.FormarOption(CRUD.Listar(new MLProgramacaoNavio { Destino = PortoDestino })?.Select(x => x.Origem.Trim()).ToList());

                return new JsonResult() { Data = new { Sucess = true, PortoOrigem = portoOrigem } };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion

        #region ListarDeadLine
        /// <summary>
        /// Listar Deadline
        /// </summary>
        /// <param name="NavioViagem"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ListarDeadLine(string NavioViagem)
        {
            try
            {
                var lst = BLTakeOrPay.ObterNavios(NavioViagem);

                return new JsonResult() {
                    Data = new
                    {
                        Sucess = true,

                        PortoOrigem = BLTakeOrPay.FormarOption(lst?.Origem?.Select(x => x.Origem).ToList()),
                        PortoDestino = BLTakeOrPay.FormarOption(lst?.Destino?.Select(x => x.Destino).ToList())
                    }
                };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }

        /// <summary>
        /// ListarDeadLineOrigem
        /// </summary>
        /// <param name="NavioViagem"></param>
        /// <param name="PortoDestino"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ListarDeadLineOrigem(string NavioViagem, string PortoDestino)
        {
            try
            {
                var lst = BLTakeOrPay.ObterNavios(NavioViagem, null, PortoDestino);

                return new JsonResult()
                {
                    Data = new
                    {
                        Sucess = true,

                        PortoOrigem = BLTakeOrPay.FormarOption(lst?.Origem?.Select(x => x.Origem).ToList())
                    }
                };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }

        /// <summary>
        /// ListarDeadLineDestino
        /// </summary>
        /// <param name="NavioViagem"></param>
        /// <param name="PortoOrigem"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ListarDeadLineDestino(string NavioViagem, string PortoOrigem)
        {
            try
            {
                var lst = BLTakeOrPay.ObterNavios(NavioViagem, PortoOrigem, null);

                return new JsonResult()
                {
                    Data = new
                    {
                        Sucess = true,

                        PortoDestino = BLTakeOrPay.FormarOption(lst?.Destino?.Select(x => x.Destino).ToList())
                    }
                };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion 

        #region ListarDeadLineHistorico
        /// <summary>
        /// Listar Deadline
        /// </summary>
        /// <param name="NavioViagem"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ListarDeadLineHistorico(string NavioViagem)
        {
            try
            {
                var lst = BLTakeOrPay.ObterNaviosHistorico(NavioViagem);

                var RIG = lst.Find(x => (new CultureInfo("pt-BR").CompareInfo).IndexOf(x.Origem, new Portos().RIG, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0);
                var IBB = lst.Find(x => (new CultureInfo("pt-BR").CompareInfo).IndexOf(x.Origem, new Portos().IBB, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0);
                var IOA = lst.Find(x => (new CultureInfo("pt-BR").CompareInfo).IndexOf(x.Origem, new Portos().IOA, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0);
                var SSZ = lst.Find(x => (new CultureInfo("pt-BR").CompareInfo).IndexOf(x.Origem, new Portos().SSZ, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0);
                var SPB = lst.Find(x => (new CultureInfo("pt-BR").CompareInfo).IndexOf(x.Origem, new Portos().SPB_1, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0) ?? lst.Find(x => (new CultureInfo("pt-BR").CompareInfo).IndexOf(x.Origem, new Portos().SPB_2, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0);
                var VIX = lst.Find(x => (new CultureInfo("pt-BR").CompareInfo).IndexOf(x.Origem, new Portos().VIX, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0);
                var SSA = lst.Find(x => (new CultureInfo("pt-BR").CompareInfo).IndexOf(x.Origem, new Portos().SSA, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0);
                var SUA = lst.Find(x => (new CultureInfo("pt-BR").CompareInfo).IndexOf(x.Origem, new Portos().SUA, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0);
                var PEC = lst.Find(x => (new CultureInfo("pt-BR").CompareInfo).IndexOf(x.Origem, new Portos().PEC, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0);

                return new JsonResult()
                {
                    Data = new
                    {
                        Sucess = true,

                        DeadlineRIG = RIG?.Deadline.Value.ToString("dd/MM") ?? "",
                        DeadlineIBB = IBB?.Deadline.Value.ToString("dd/MM") ?? "",
                        DeadlineIOA = IOA?.Deadline.Value.ToString("dd/MM") ?? "",
                        DeadlineSSZ = SSZ?.Deadline.Value.ToString("dd/MM") ?? "",
                        DeadlineSPB = SPB?.Deadline.Value.ToString("dd/MM") ?? "",
                        DeadlineVIX = VIX?.Deadline.Value.ToString("dd/MM") ?? "",
                        DeadlineSSA = SSA?.Deadline.Value.ToString("dd/MM") ?? "",
                        DeadlineSUA = SUA?.Deadline.Value.ToString("dd/MM") ?? "",
                        DeadlinePEC = PEC?.Deadline.Value.ToString("dd/MM") ?? "",

                        DeadlineRIGAtivo = RIG?.DeadlineAtiva,
                        DeadlineIBBAtivo = IBB?.DeadlineAtiva,
                        DeadlineIOAAtivo = IOA?.DeadlineAtiva,
                        DeadlineSSZAtivo = SSZ?.DeadlineAtiva,
                        DeadlineSPBAtivo = SPB?.DeadlineAtiva,
                        DeadlineVIXAtivo = VIX?.DeadlineAtiva,
                        DeadlineSSAAtivo = SSA?.DeadlineAtiva,
                        DeadlineSUAAtivo = SUA?.DeadlineAtiva,
                        DeadlinePECAtivo = PEC?.DeadlineAtiva
                    }
                };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion 

        #region CarregarValorAdicional
        /// <summary>
        /// CarregarValorAdicional
        /// </summary>
        /// <param name="PortoDestino"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult CarregarValorAdicional(string PortoDestino)
        {
            try
            {
                var POD = BLTakeOrPay.CarregarValorAdicional(PortoDestino);

                return new JsonResult()
                {
                    Data = new
                    {
                        Sucess = true,
                        Valor = POD?.TarifaAdicional ?? 0
                    }
                };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion 

        #region SomarContainerSemana
        /// <summary>
        /// Somar container quando for semanal
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult SomarContainerSemana(decimal? Codigo)
        {
            try
            {
                var listaContainers = CRUD.Listar(new MLTakeOrPayEmbarqueCertoXContainers { CodigoEmbarqueCerto = Codigo });

                // Soma todas as unidades dos diferentes portos
                var unidades = listaContainers.Sum(x => x.Unidades).GetValueOrDefault(0);

                // Soma a tonelada média de todos os containers
                var toneladas = listaContainers.Sum(x => x.TonelagemMedia * x.Unidades).GetValueOrDefault(0);

                // Realiza o calculo do TEU (se o container for 20' -> número de unidades *1, se for 40' -> número de unidades * 2)
                var TEU = listaContainers.Sum(x => x.Unidades * (x.TamanhoContainer == BLConfiguracao.Pastas.ModuloTakeOrPayContainer20() ? 1 : 2)).GetValueOrDefault(0);

                return new JsonResult(){
                    Data = new
                    {
                        Sucess = true,
                        UnidadesSemana = unidades,
                        ToneladasSemana = toneladas,
                        TEUSemana = TEU,
                        UnidadesMes = unidades * 4,
                        ToneladasMes = toneladas * 4,
                        TEUMes = TEU * 4
                    }
                };
            }
            catch
            {
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion

        #region SomarContainerMes
        /// <summary>
        /// Somar container quando for mensal
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult SomarContainerMes(decimal? Codigo)
        {
            try
            {
                var listaContainers = CRUD.Listar(new MLTakeOrPayEmbarqueCertoXContainers { CodigoEmbarqueCerto = Codigo });

                // Soma todas as unidades dos diferentes portos
                var unidades = listaContainers.Sum(x => x.Unidades).GetValueOrDefault(0);

                // Soma a tonelada média de todos os containers
                var toneladas = listaContainers.Sum(x => x.TonelagemMedia * x.Unidades).GetValueOrDefault(0);

                // Realiza o calculo do TEU (se o container for 20' -> número de unidades *1, se for 40' -> número de unidades * 2)
                var TEU = listaContainers.Sum(x => x.Unidades * (x.TamanhoContainer == BLConfiguracao.Pastas.ModuloTakeOrPayContainer20() ? 1 : 2)).GetValueOrDefault(0);

                return new JsonResult()
                {
                    Data = new
                    {
                        Sucess = true,
                        UnidadesMes = unidades,
                        ToneladasMes = toneladas,
                        TEUMes = TEU
                    }
                };
            }
            catch
            {
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion

        #region CarregarTermos
        /// <summary>
        /// Retorna os termos preenchidos
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult CarregarTermos(decimal? Codigo)
        {
            try
            {
                var modelCompleta = BLTakeOrPay.ObterCompleto(Codigo.GetValueOrDefault(0));

                var termos = BLTakeOrPay.PreencherTermos(modelCompleta, modelCompleta.lstProposta, modelCompleta.lstContainer);

                return new JsonResult()
                {
                    Data = new
                    {
                        Sucess = !string.IsNullOrEmpty(termos),
                        Termos = termos
                    }
                };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { Sucess = false } };
            }
        }
        #endregion

        #region FinalizarReserva
        /// <summary>
        /// Finalizar Reserva
        /// </summary>
        /// <param name="Codigo"></param>
        /// <param name="modelModulo"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult FinalizarReserva (decimal? Codigo, MLModuloTakeOrPayPublicado modelModulo)
        {
            modelModulo = CRUD.Obter(modelModulo);

            var modelEmbarqueCerto = CRUD.Obter(new MLTakeOrPayEmbarqueCerto { Codigo = Codigo });

            modelEmbarqueCerto.TermoAceito = true;
            CRUD.SalvarParcial(modelEmbarqueCerto);

            return new JsonResult()
            {
                Data = new
                {
                    Sucess = BLTakeOrPay.EnviarEmail(Codigo, modelModulo)
                }
            };
        }

        #endregion

        #region BuscarCep
        /// <summary>
        /// Buscar CEP
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult BuscarCep(string Cep)
        {
            var objCep = new MLCep();
            var url = "https://viacep.com.br/ws/{0}/json";

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

                var request = (HttpWebRequest)WebRequest.Create(string.Format(url, Cep));

                var data = Encoding.ASCII.GetBytes("");

                request.Method = "GET";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                var response = (HttpWebResponse)request.GetResponse();

                objCep = JsonConvert.DeserializeObject<MLCep>(new StreamReader(response.GetResponseStream()).ReadToEnd());
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false });
            }

            if (objCep.uf == null) return Json(new { success = false });

            return Json(new { success = true, localidade = objCep.localidade, bairro = objCep.bairro, logradouro = objCep.logradouro, uf = objCep.uf });
        }

        #endregion

        //Redireciona para as Views

        #region Cadastro
        /// <summary>
        /// Retorna a view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult Cadastro(MLModuloTakeOrPay model)
        {
            return View("Cadastro", model);
        }

        #endregion

        #region Etapa1
        /// <summary>
        /// Retorna a view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult Etapa1(MLModuloTakeOrPay model)
        {
            return View("Etapa1", model);
        }

        #endregion

        #region SelecPropostas
        /// <summary>
        /// Retorna a view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult SelecPropostas(MLModuloTakeOrPay model)
        {
            return View("SelecPropostas", model);
        }

        #endregion

        #region ScriptCadastro
        /// <summary>
        /// Retorna o script
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptCadastro(MLModuloTakeOrPay model)
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

        #region DadosCadastrais
        /// <summary>
        /// Retorna a view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult DadosCadastrais(MLModuloTakeOrPay model)
        {
            return View("DadosCadastrais", model);
        }

        #endregion

        #region ScriptCadastro
        /// <summary>
        /// Retorna o script
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptDadosCadastrais(MLModuloTakeOrPay model)
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

        #region TipoReserva
        /// <summary>
        /// Retorna a view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult TipoReserva(MLModuloTakeOrPay model)
        {
            return View("TipoReserva", model);
        }

        #endregion

        #region ScriptTipoReserva
        /// <summary>
        /// Retorna o script
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptTipoReserva(MLModuloTakeOrPay model)
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

        #region TipoReserva
        /// <summary>
        /// Retorna a view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult DistribuicaoCarga(MLModuloTakeOrPay model)
        {
            return View("DistribuicaoCarga", model);
        }

        #endregion

        #region Etapa4_1
        /// <summary>
        /// Retorna a view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult Etapa4_1(MLModuloTakeOrPay model)
        {
            return View("Etapa4_1", model);
        }

        #endregion

        #region Etapa4_2
        /// <summary>
        /// Retorna a view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult Etapa4_2(MLModuloTakeOrPay model)
        {
            return View("Etapa4_2", model);
        }

        #endregion

        #region ScriptTipoReserva
        /// <summary>
        /// Retorna o script
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptDistribuicaoCarga(MLModuloTakeOrPay model)
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

        #region ConclusaoReserva
        /// <summary>
        /// Retorna a view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult ConclusaoReserva(MLModuloTakeOrPay model)
        {
            return View("ConclusaoReserva", model);
        }

        #endregion

        #region ScriptConclusaoReserva
        /// <summary>
        /// Retorna o script
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptConclusaoReserva(MLModuloTakeOrPay model)
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

        #region Conclusao
        /// <summary>
        /// Retorna a view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermissionAttribute(global::Permissao.Publico)]
        public ActionResult Conclusao(MLModuloTakeOrPay model)
        {
            return View("Conclusao", model);
        }

        #endregion
    }
}