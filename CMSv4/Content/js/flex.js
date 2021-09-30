// Flex Admin Custom JavaScript Document

//Sidebar Toggle
$("#sidebar-toggle").click(function (e) {
    e.preventDefault();
    $(".navbar-side").toggleClass("collapsed");
    $("#page-wrapper").toggleClass("collapsed");
});

//Portlet Icon Toggle
$(".portlet-widgets .fa-chevron-down, .portlet-widgets .fa-chevron-up").click(function () {
    $(this).toggleClass("fa-chevron-down fa-chevron-up");
});


//Portlet Refresh Icon
(function ($) {
    $.fn.extend({

        addTemporaryClass: function (className, duration) {
            var elements = this;
            setTimeout(function () {
                elements.removeClass(className);
            }, duration);

            return this.each(function () {
                $(this).addClass(className);
            });
        }
    });

    $.fn.buscaEndereco = function (options) {
        return this.each(function () {
            var $thisCep = $(this);

            $thisCep.focusout(function () {
                if ($(this).val() != "" && $(this).val() != "__.___-___") {
                    $.ajax({
                        url: "http://apps.widenet.com.br/busca-cep/api/cep.json",
                        data: { code: $thisCep.val() },
                        beforeSend: function () {
                            //channelingLoad(e);
                        },
                        success: function (data) {
                            $(options.Logradouro).val(data.address);
                            $(options.Bairro).val(data.district);
                            $(options.Cidade).val(data.city);
                            $(options.Estado).val(data.state);

                            //Após preencher os campos, realizada validação no form
                            //Para remover as classes de validação error
                            $(options.Form).valid();
                        },
                        error: function (err) {
                        }
                    });
                }
            });
        });
    };

})(jQuery);

$("a i.fa-refresh").click(function () {
    $(this).addTemporaryClass("fa-spin fa-spinner", 2000);
});

//Easing Script for Smooth Page Transitions
$(function () {
    $('.page-content').addClass('page-content-ease-in');
});

//Tooltips
$(function () {

    // Tooltips for sidebar toggle and sidebar logout button
    $('.tooltip-sidebar-toggle, .tooltip-sidebar-logout').tooltip({
        selector: "[data-toggle=tooltip]",
        container: "body"
    })

})

// Get all CheckedBox Values
function getAllValues(selector) {
    var ids = [];

    try {
        var grid = $("#lista").dataTable();
        $(selector, grid.fnGetNodes()).each(function () {
            ids.push($(this).val());
        });
    }
    catch (err) { }

    return ids;
}

// LOADING PANEL
$(document).ajaxSend(function (event, jqxhr, settings) {
    if (settings.loadingPanel != null && settings.loadingPanel == false) {
        return;
    }

    $("#spinner").height($(document).height());
    $("#spinner").show();
});
$(document).ajaxComplete(function (event, jqxhr, settings) {
    $("#spinner").fadeOut();
});
$(document).ajaxError(function () {
    $("#spinner").fadeOut();
});

window.onbeforeunload = function () {
    $("#spinner").height($(document).height());
    $("#spinner").show();
}

// FORM DISABLE
function disableForm() {
    $('input, textarea, select').prop('disabled', 'disabled');

    try {
        for (var name in CKEDITOR.instances) { CKEDITOR.instances[name].setReadOnly(); }
    } catch (e) { }
}

// REMOVER ACENTO DE PALAVRAS
function RemoverAcentos(newStringComAcento) {
    var string = newStringComAcento;
    var mapaAcentosHex = {
        a: /[\xE0-\xE6]/g,
        A: /[\xC0-\xC6]/g,
        e: /[\xE8-\xEB]/g,
        E: /[\xC8-\xCB]/g,
        i: /[\xEC-\xEF]/g,
        I: /[\xCC-\xCF]/g,
        o: /[\xF2-\xF6]/g,
        O: /[\xD2-\xD6]/g,
        u: /[\xF9-\xFC]/g,
        U: /[\xD9-\xDC]/g,
        c: /\xE7/g,
        C: /\xC7/g,
        n: /\xF1/g,
        N: /\xD1/g,
    };

    for (var letra in mapaAcentosHex) {
        var expressaoRegular = mapaAcentosHex[letra];
        string = string.replace(expressaoRegular, letra);
    }

    return string;
}

//Função para causar delay na chamada do evento para que vários keyup não execute diversas vezes a mesma função
var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();

function scrollToTop(top) {
    if (window.parent) {
        if (top) {
            window.parent.$("body, html").animate({ scrollTop: top }, 200);
        }
        else {
            window.parent.$("body, html").animate({ scrollTop: 0 }, 200);
        }
    }
}