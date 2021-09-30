/*
*   Inclui novo módulo em repositório da página
*/
function novoModulo(pag, rep, mod, portal, sender) {
    if (mod == 0) return;

    var loadingId = "loading_" + pag + "_" + rep + "_" + mod;
    if (sender) { //adiciona texto 'carregando'
        $(sender).attr("disabled", "disabled");
        $(sender).prepend("<option id='" + loadingId + "' value='' selected>Carregando...</option>");
    }

    $.post('/cms/' + portal + '/pagina/novomodulo', { codigoPagina: pag, repositorio: rep, codigoModulo: mod },
        function (response) {
            $('#cms-ac-rep-' + rep).replaceWith(response);

            for (var name in CKEDITOR.instances) { CKEDITOR.instances[name].destroy(); }

            RefreshEditor();
        }
    ).fail(function () {
        if (sender) { //remove texto 'carregando'
            $(sender).removeAttr("disabled");
            $("#" + loadingId).remove();
            sender.selectedIndex = 0;
        }
    });
}

/*
*   Salvar o conteúdo do módulo in line no onblur do editor html
*/
function salvarModuloInline(pag, rep, urlModulo, portal, conteudo) {
    // primeiro remove o conteúdo do módulo
    $.post('/cms/' + portal + '/' + urlModulo + '/editar', { codigoPagina: pag, repositorio: rep, conteudo: conteudo },
        function (response, opts) {
            // depois remove a referência do módulo na página
        }
    );
}

/*
*   Salva o conteúdo do Módulo
*/
function salvarModulo(toolbarParent, urlModulo, urlPortal) {
    $("#propriedade").validate();

    if ($("#propriedade").valid() == false)
        return false;

    var dataSend = $('#propriedade').serialize();
    var form = $('#propriedade');

    var pag = $('input[name=CodigoPagina]', form).val();
    var rep = $('input[name=Repositorio]', form).val();

    $.post('/cms/' + urlPortal + '/' + urlModulo + '/editar', dataSend,
         function (responseEditar) {
             //Atualiza o repositório do módulo
             $.post('/cms/' + urlPortal + '/' + urlModulo + '/visualizar', { codigoPagina: pag, repositorio: rep, edicao: true },
                function (response) {
                    var toolbaroptions = $('.cms-ac-toolbar', $(toolbarParent));
                    $(toolbarParent).html(toolbaroptions[0].outerHTML + response);

                    $('.modal').modal("hide");
                    $('#propriedade').removeData('validator');
                    $('#propertiesModal .modal-content').empty();

                    if (responseEditar.bannerAutoPlay != null && responseEditar.bannerTempo != null && responseEditar.repositorio != null) {
                        $('#carousel-example-generic_' + responseEditar.repositorio).carousel({ interval: responseEditar.bannerTempo });

                        if (responseEditar.bannerAutoPlay == "false") {
                            $('#carousel-example-generic_' + responseEditar.repositorio).carousel('pause');
                        }
                    }

                    //Execução exclusica de cada módulo
                    if (responseEditar.modulo != null) {
                        //Recarrega o script após salvar a propriedade do módulo
                        if (responseEditar.modulo == "enquete") {

                            var dataSend =
                                {
                                    "Repositorio": responseEditar.Repositorio,
                                    "VotarRestrito": responseEditar.VotarRestrito,
                                    "CodigoEnquete": responseEditar.CodigoEnquete,
                                    "CodigoPagina": responseEditar.CodigoPagina,
                                    "ResultadoRestrito": responseEditar.ResultadoRestrito
                                }

                            $.ajax({
                                type: "POST",
                                url: "/cms/" + responseEditar.portaldiretorio + "/Enquete/ScriptAreaConstrucao",
                                data: dataSend,
                                success: function (data) {
                                    $("body").append(data);
                                }
                            });
                        }
                        else if (responseEditar.modulo == "faq") {
                            $.ajax({
                                type: "POST",
                                url: "/cms/" + responseEditar.portaldiretorio + "/Faq/Script",
                                data: {
                                    "CodigoPagina": responseEditar.CodigoPagina,
                                    "Repositorio": responseEditar.Repositorio,
                                    "Categorias": responseEditar.Categorias
                                },
                                success: function (data) {
                                    $("body").append(data);
                                }
                            });
                        }
                    }
                }
            );
         }
     );

    return false;
}

/*
*   Abre a janela de popup para edição do módulo. Necessita da JWindow
*/
function editarModulo(anchor, pag, rep, urlModulo, urlPortal) {
    var toolbarParent = $(anchor).parents('.cms-ac-container')[0];

    if ($(anchor).hasClass("disabled")) {
        return;
    }

    var topModal = $(anchor).offset().top;

    $(anchor).addClass("disabled");

    $.get('/cms/' + urlPortal + '/pagina/propriedades', { codigoPagina: pag, repositorio: rep, urlModulo: urlModulo },
        function (response, opts) {
            $("#propertiesModal").css({ top: topModal + 'px' });
            $("#propertiesModal").modal("show");

            //backdrop desabilitado, descomentar caso reativado. função remove elemento de fundo da modal
            //$("#propertiesModal .modal-backdrop").remove();

            $('#propertiesModal .modal-content').html(response);
            $('#contentPropertiesModal').html("Carregando...");

            $.get('/cms/' + urlPortal + '/' + urlModulo + '/editar', { codigopagina: pag, repositorio: rep }, function (data) {
                $('#contentPropertiesModal').html(data);

                console.log(data);

                $('#salvarModulo').on("click", function () {
                    salvarModulo(toolbarParent, urlModulo, urlPortal);
                });

                //backdrop desabilitado, descomentar caso reativado. função remove elemento de fundo da modal
                $("#propertiesModal .modal-backdrop").remove();
                //move o backdrop para trás da modal principal
                $("#propertiesModal .modal-backdrop").insertAfter($("#propertiesModal"));

                $(anchor).removeClass("disabled");
                //$("#standardModalLabel").html($("#standardModalLabel").html() + " (" + urlModulo + ")");

            }).fail(function () {
                $(anchor).removeClass("disabled");
            });
        }
    ).fail(function () {
        $(anchor).removeClass("disabled");
    });
}

/*
*   Remove o módulo na tabela de EDICAO e da tabela de conteúdo (se houver)
*/
function removerModulo(anchor, pag, rep, urlModulo, portal) {
    var toolbarParent = $(anchor).parents('.cms-ac-container')[0];

    // primeiro remove o conteúdo do módulo
    $.post('/cms/' + portal + '/' + urlModulo + '/excluir', { codigoPagina: pag, repositorio: rep },
        function (response, opts) {
            // depois remove a referência do módulo na página
            $.post('/cms/' + portal + '/pagina/removermodulo', { codigoPagina: pag, repositorio: rep },
                function (response) {
                    $(toolbarParent).replaceWith(response);
                }
            );
        }
    );
}

/*
*   Salvar Página
*/
function salvarPagina(botao, portal, formpopup) {
    var form = botao.parents('form')[0];

    botao.addClass("disabled");

    if ($(form).valid()) {
        $(form).find('textarea[data-editor]').each(function () {
            $(this).val(unescape($(this).val())); /*remover escape do evento blur das textareas*/
            $(this).val(escape($(this).val())); /*enviar para o servidor com escape*/
        });

        $.post(
            form.action,
            $(form).serialize(),
            function (response) {
                var tipo = 'success';
                var msg = 'Página Salva com Sucesso!';
                if (!response.success) {
                    tipo = 'error'
                    msg = response.msg;
                } else {
                    if (formpopup == undefined) {
                        var body = $('iframe').contents().find('body');

                        screenShot(body, portal, $("input[name=Codigo]", form).val());
                    } else {
                        formpopup.submit();
                    }
                }

                botao.removeClass("disabled");
                Mensagem(msg, tipo);
            }
        );
    }
    else {
        botao.removeClass("disabled");
    }

    return false;
}

/*
*   Publicar Página
*/
function publicarPagina(botao, url) {
    var form = botao.parents('form')[0];

    botao.addClass("disabled");

    if ($(form).valid()) {
        $(form).find('textarea[data-editor]').each(function () {
            $(this).val(unescape($(this).val())); /*remover escape do evento blur das textareas*/
            $(this).val(escape($(this).val())); /*enviar para o servidor com escape*/
        });

        $.post(
            form.action,
            $(form).serialize(),
            function (response) {
                if (response.success == false) {
                    Mensagem(response.msg, 'error');
                    botao.removeClass("disabled");
                }
                else {
                    Mensagem('A página foi salva com sucesso e será publicada!', 'success');
                    window.location = url;
                }
            }
        );
    }
    else {
        botao.removeClass("disabled");
    }

    return false;
}

/*
*   Plugin para exibição de mensagens no canto inferior direito da aplicação
*/
function Mensagem(msg, tipo) {
    try {
        window.parent.Mensagem(msg, tipo);
    }
    catch (err) {
        Messenger.options = {
            extraClasses: 'messenger-fixed messenger-on-bottom messenger-on-right',
            theme: 'flat'
        }
        Messenger().post({
            message: msg,
            type: tipo,
            showCloseButton: false,
            hideAfter: 4
        });
    }
}

/*
*   Recuperar uma página de publicação para edição
*/
var recuperarPaginaPublicada = function (portal, pag) {
    $.post('/cms/' + portal + '/pagina/editar', { id: pag, recuperar: true },
        function (response) { location.reload(); }
    );

    return false;
}

/*
*   Recuperar uma página de publicação para edição
*/
var recuperarPaginaHistorico = function (portal, pag) {
    $.post('/cms/' + portal + '/pagina/editarhistorico',
        {
            id: pag
        },
        function (response) {
            location.reload();
        }
    );

    return false;
}

/*
*   Mover módulo na página
*/
var clipboardRepositorioRecortar = 0;
var urlmoduloOrigem = '';
var toolbarOrigem = null;

function recortarModulo(repositorio, urlModulo, anchor, codigomodulo) {
    toolbarOrigem = $(anchor).parents('.cms-ac-container')[0];

    clipboardRepositorioRecortar = repositorio;
    urlmoduloOrigem = urlModulo;

    $('[rel=colar]').each(function (i) { $(this).attr('style', 'display: auto'); });
    $('[rel=recortar]').each(function (i) { $(this).attr('style', 'display: none!important'); });

    return false;
}

function colarModulo(codigoPagina, repositorioDestino, portal, anchor, urlModulo) {
    if (clipboardRepositorioRecortar > 0) {
        var toolbarParent = $(anchor).parents('.cms-ac-container')[0];

        // primeiro remove o conteúdo do módulo
        $.post('/cms/' + portal + '/pagina/movermodulo', { codigoPagina: codigoPagina, repositorioOrigem: clipboardRepositorioRecortar, repositorioDestino: repositorioDestino },
            function (response, opts) {
                /*
                    Post para atualizar o Repositorio de Destino com o Novo Modulo
                */
                $.post('/cms/' + portal + '/pagina/modulo', { codigoPagina: codigoPagina, repositorio: repositorioDestino, urlModulo: urlmoduloOrigem },
                    function (responsetool) {
                        if (urlModulo == '') {
                            $('#cms-ac-rep-' + repositorioDestino).replaceWith(responsetool);
                        } else {
                            $(toolbarParent).replaceWith(responsetool);
                        }
                        for (var name in CKEDITOR.instances) { CKEDITOR.instances[name].destroy(); }
                        RefreshEditor();
                    }
                );

                /*
                    Post para atualizar o repositorio de Origem com o Novo Modulo
                */
                if (urlModulo != '') {
                    //Caso exista um módulo no lugar
                    $.post('/cms/' + portal + '/pagina/modulo', { codigoPagina: codigoPagina, repositorio: clipboardRepositorioRecortar, urlModulo: urlModulo },
                       function (responsetool) {
                           $(toolbarOrigem).replaceWith(responsetool);
                           for (var name in CKEDITOR.instances) { CKEDITOR.instances[name].destroy(); }
                           RefreshEditor();
                       }
                   );
                }
                else {
                    //Caso não exista um módulo
                    $.get('/cms/' + portal + '/pagina/novomodulo',
                        { codigoPagina: codigoPagina, repositorio: clipboardRepositorioRecortar },
                        function (responsetool) { $(toolbarOrigem).replaceWith(responsetool); }
                    );
                }
            }
        );
    }

    $('[rel=colar]').each(function (i) { $(this).attr('style', 'display: none!important'); });
    $('[rel=recortar]').each(function (i) { $(this).attr('style', 'display: auto'); });

    return false;
}

/*
*   Remove os caracteres inválidos de uma URL para ser usada como url amigavel
*/
function seoURL(url) {
    url = url.replace(/[^a-zA-Z0-9-_\s]/g, "");
    url = url.replace(/_|\s/g, "");

    return url;
}

// CKEDITOR
function loadCKEditorDiv(obj) {
    var portal = obj.data('portal');
    var pagina = obj.data('pagina');
    var rep = obj.data('rep');
    var roxyFileman = '/Content/js/plugins/ckeditor/plugins/fileman/Index?integration=ckeditor';

    var editor = obj.ckeditor(
    {
        height: obj.height(),
        contentsCss: '/portal/' + portal + '/content/css/editor_html.css',

        filebrowserBrowseUrl: roxyFileman,
        filebrowserImageBrowseUrl: roxyFileman,

        extraPlugins: "iframedialog,sourcedialog",

        allowedContent: true,
        htmlEncodeOutput: false,
        entities: false,
        autoParagraph: false,
        tabSpaces: 6,

        toolbar:
           [
               { name: "mode", items: ["Sourcedialog"] },
               { name: 'clipboard', groups: ['clipboard', 'undo'], items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', 'Undo', 'Redo'] },
               { name: 'insert', items: ['CreatePlaceholder', 'Image', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'Iframe'] },
               { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
               { name: 'basicstyles', groups: ['basicstyles', 'cleanup'], items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', 'RemoveFormat'] },
               { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align'], items: ['NumberedList', 'BulletedList', 'Outdent', 'Indent', 'Blockquote', 'CreateDiv', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
               '/',
               { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
               { name: 'colors', items: ['TextColor', 'BGColor'] },
               { name: 'tools', items: ['UIColor', 'Maximize', 'ShowBlocks'] }
           ]
    });

    editor.editor.config.font_names = 'DINOT/dinot;' + editor.editor.config.font_names;
    editor.editor.config.skin = "moono-lisa";

    editor.on('blur', function () {
        if (editor.editor.checkDirty()) {
            salvarModuloInline(pagina, rep, "html", portal, editor.editor.getData());
            editor.editor.resetDirty();
        }
    });

    obj.closest('form').submit(function () {
        obj.val(escape(obj.val()));
    });
}

function fixInlineEditor(id) {
    loadCKEditorDiv($("#" + id));
}

function RefreshEditor() {
    try {
        var divEditor = $('div[data-ckeditor]');

        divEditor.each(function () {
            loadCKEditorDiv($(this));
        });
    } catch (e) { }
}

try {
    CKEDITOR.disableAutoInline = true;
    CKEDITOR.on('instanceReady', function (ev) {
        var editor = ev.editor;

        editor.filter.allowedContent = true;

        var blockTags = ['div', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'p', 'pre', 'ul', 'li', 'table', 'td', 'tr', 'blockquote', 'a', 'span'];
        var rules = {
            indent: true,
            breakBeforeOpen: true,
            breakAfterOpen: true,
            breakBeforeClose: true,
            breakAfterClose: true
        };

        for (var i = 0; i < blockTags.length; i++) {
            ev.editor.dataProcessor.writer.setRules(blockTags[i], rules);
        }
    });
    CKEDITOR.on('dialogDefinition', function (e) {
        var dialogName = e.data.name;

        //O dialog pode já ter um evento onshow, persistir o evento para não perder a funcionalidade padrão do dialog
        var dialogDefinition = e.data.definition;
        var defaultOnShow = dialogDefinition.onShow;

        dialogDefinition.onShow = function () {

            //Chamar a função padrão do dialog
            if (defaultOnShow != null) {
                defaultOnShow.call(this, arguments);
            }

            //Centralizar o dialog na tela
            this.move(this.getPosition().x, $(e.editor.container.$).offset().top);
        };

        if (dialogName == "sourcedialog") {
            var contents = $(dialogDefinition.dialog.parts["contents"]);

            contents.attr("class", contents.attr("class") + " codigo-fonte"); //adicionar classe para definição de estilo da janela
        }
    });
} catch (e) { }

/*
*   Abre a janela de popup para edição do módulo
*/
function showHistorico(pag, urlPortal) {
    $('#construcaoModal').empty();
    $.post('/cms/' + urlPortal + '/pagina/historico',
         {
             id: pag
         },
         function (response, opts) {
             $('#construcaoModal').html(response);
         }
     );
}

/*
*   Plugin para capturar a área visível da página no momento que houver uma atualização na página
*/
function screenShot(doc, portalDiretorio, pag) {
    try {
        setTimeout(function () {
            html2canvas(doc, {
                onrendered: function (canvas) {
                    $.post('/cms/' + portalDiretorio + '/pagina/ScreenShot', {
                        file: canvas.toDataURL(),
                        pagina: pag
                    }, function (response, opts) {
                        Mensagem("Thumb da página gerada com sucesso!", "success");
                    });
                }
            });
        }, 1000);
    } catch (e) {
    }
}

/*
*   Evento para aceitar somente comandos numericos no input
*   Uso onkeypress="return somenteNumeros(event)"
*/
function somenteNumeros(e) {
    var key = window.event ? e.keyCode : e.which;
    var keychar = String.fromCharCode(key);
    var vrRetorno = false;

    if (key != 8 && key != 0) {
        goodChars = "0123456789";
        if (goodChars.indexOf(keychar) != -1) {
            vrRetorno = true;
        }
    }
    else {
        vrRetorno = true;
    }
    return vrRetorno;
}

/*
*   Evento para quando a modal de propriedades do módulo tiver fechado
*/
$(document).ready(function () {
    var roxyFileman = '/Content/js/plugins/ckeditor/plugins/fileman/Index?integration=ckeditor';

    RefreshEditor();

    var textAreaEditor = $('textarea[data-ckeditor]');
    if (textAreaEditor.length > 0) {
        textAreaEditor.each(function () {
            var textarea = $(this);
            var portal = textarea.data('ckeditor');

            var editor = textarea.ckeditor(
            {
                height: textarea.height(),
                contentsCss: '/portal/' + portal + '/content/css/editor_html.css',
                filebrowserBrowseUrl: roxyFileman,
                filebrowserImageBrowseUrl: roxyFileman
            });

            textarea.closest('form').submit(function () {
                textarea.val(escape(textarea.val()));
            });
        });
    }

    $("#propertiesModal").on("hidden.bs.modal", function () {
        // Remover plugins de datepicker que ficaram sobrando na página
        $(this).nextAll(".xdsoft_datetimepicker").remove();
    });

    $("body").addClass("areaedicao");
});