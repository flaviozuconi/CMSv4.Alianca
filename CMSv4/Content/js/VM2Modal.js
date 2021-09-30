/*! Copyright 2012, Ben Lin (http://dreamerslab.com/)
 * Licensed under the MIT License (LICENSE.txt).
 *
 * Version: 1.0.19
 *
 * Requires: jQuery >= 1.2.3
 */
(function ($) {
    $.fn.ModalPadrao = function (options) {

        // This is the easiest way to have default options.
        var settings = $.extend({
            //ID da modal que será criada
            modalId: "modalGlobal",

            //Html padrão da modal
            modalHtml:
                '<div class="modal modal-centered modal-flex fade in" id="modalGlobal" tabindex="-1" role="dialog" aria-labelledby="modalAdminGlobal" aria-hidden="false">' +
                    '<div class="modal-dialog modal-dialog-centered">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>' +
                                '<h4 class="modal-title" id="modalGlobalH4"></h4>' +
                            '</div>' +
                            '<div class="modal-body" id="modalGlobalBody"></div>' +
                            '<div class="modal-footer" id="modalGlobalFooter"></div>' +
                        '</div>' +
                    '</div>' +
                '</div>'
            ,

            //element de onde será adicionada a modal
            appendTo: $(window.parent.document.body),

            //Título da modal
            titulo: "Confirmação",

            //Mensagem que será exibida no body da modal
            mensagem: "Tem certeza que deseja executar esta operação?",

            //Tipo do aviso, podendo ser: success, warning ou danger
            tipo: "warning",

            //Define se a moda será exibida após completar a inialização
            exibirNaInicializacao: false,

            //Json com os botões que serão adicionados no rodapé como opção da modal
            buttons: {
                "Fechar": {
                    id: "btnFechar",
                    texto: "Fechar",
                    classe: "btn-default",
                    callback: function () {           
                        $(settings.appendTo).find("#" + settings.modalId).modal('hide');
                    }
                }
            }
        }, options);

        if (settings.exibirNaInicializacao) {
            inicializar(settings);
        }
        else {
            return this.each(function () {
                $(this).on('click', function () {
                    inicializar(settings);
                });

                // Exibir modal no centro da tela.
                //$(document).on('show.bs.modal',  "#" + settings.modalId, reposition);
            });
        }
        
    };

}(jQuery));

function inicializar(settings) {
    var selector = $(settings.appendTo);

    selector.append(settings.modalHtml);

    //Definir o título H4 na modal
    selector.find("#" + settings.modalId + "H4").html(settings.titulo);

    //Definir o tipo da mensagem e gerar html
    switch (settings.tipo) {
        case "success":
            selector.find("#" + settings.modalId + "Body").html(gerarHtmlSucesso(settings.mensagem));
            break;

        case "warning":
            selector.find("#" + settings.modalId + "Body").html(gerarHtmlAviso(settings.mensagem));
            break;

        case "danger":
            selector.find("#" + settings.modalId + "Body").html(gerarHtmlError(settings.mensagem));
            break;
    }

    //Adicionar os botões no footer
    $.each(settings.buttons, function (index, item) {
        selector.find("#" + settings.modalId + "Footer").append(
            '<button id="' + item.id + '" class="btn ' + item.classe + '">' + item.texto + '</button>'
        );

        selector.find("#" + item.id).click(item.callback);
    });

    //Exibir modal após configuração
    selector.find("#" + settings.modalId).modal('show');

    //Remover a modal do body após fechar.
    selector.find("#" + settings.modalId).on('hidden.bs.modal', function () {
        $(this).remove();
    });
}

function gerarHtmlSucesso(mensagem) {
    return "<div class='alert alert-success'><i class='fa fa-check'></i> " + mensagem + "</div>"
}

function gerarHtmlError(mensagem) {
    return "<div class='alert alert-danger' ><i class='fa fa-warning'></i> " + mensagem + "</div>";
}

function gerarHtmlAviso(mensagem) {
    return "<div class='alert alert-warning' ><i class='fa fa-warning'></i> " + mensagem + "</div>";
}