//Só utilizo como plugin para poder passar parametros do MVC para o JS, neste caso, diretório do portal,
//Este script não esta preparado para multiplas instâncias.

(function ($) {
    $.fn.VM2IconPicker = function (options) {

        // This is the easiest way to have default options.
        var settings = $.extend({
           
            selectorInput: "#Imagem",

            //Diretório do portal para chamadas ajax
            DiretorioPortal: "Principal"

        }, options);

        return this.each(function () {
            if (!$(this).hasClass("iconpicker")) {
                $(this).addClass("iconpicker")
            }

            //Fechar o popover quando clicar fora da div com os ícones
            $('body').on('click', function (e) {

                $('.iconpicker').each(function () {

                    //'is' for buttons that trigger popups
                    //'has' for icons within a button that triggers a popup
                    if (!$(this).is(e.target) && $(this).has(e.target).length === 0 && $('.popover').has(e.target).length === 0) {
                        $(this).popover('destroy');
                    }
                });
            });

            $(document).on('keyup', '#buscaIcone', function (e) {
                delay(function () {
                    listarIcons(1, $("#buscaIcone").val(), settings.DiretorioPortal);
                }, 400);
            });

            $(document).on('click', '.btn-arrow', function () {
                listarIcons(parseInt($("#iconPage").val()) + parseInt($(this).val()), $("#buscaIcone").val(), settings.DiretorioPortal);

                return false;
            });

            //Clique sobre o ícone
            $(document).on("click", ".btn-icon", function () {
                $(settings.selectorInput).val($(this).find("img").attr("src"));
                $(settings.selectorInput).popover("hide");

                return false;
            });

            //Clique para modificar tamanho de exibição
            $(document).on("change", "#sizeOptions :input", function () {
                listarIcons($("#iconPage").val(), $("#buscaIcone").val(), settings.DiretorioPortal, $(this).val());
            });

            //Quando ganhar foco no campo de icone, exibir popover com ajax
            $(document).on('focus', settings.selectorInput, function () {
                
                $.get("/cms/" + settings.DiretorioPortal + "/Util/ListarIconPack", {
                    search: "",
                    page: 1
                }, function (response) {
                    $(settings.selectorInput).popover('destroy').popover({
                        html: true,
                        placement: 'auto',
                        trigger: 'manual',
                        content: response,
                        template: '<div class="popover" style="min-width:300px">' +
                                        '<div class="arrow"></div>' +
                                        '<h3 class="popover-title"></h3>' +
                                        '<div><div id="sizeOptions" class="btn-group btn-group-xs" data-toggle="buttons" style="padding: 10px 14px 0px 15px;float: right;">' +
                                            '<label class="btn btn-white active" data-size="16">' +
                                                '<input type="radio" value="16" checked>16' +
                                            '</label>' +
                                            '<label class="btn btn-white" data-size="32">' +
                                                '<input type="radio" value="32">32' +
                                            '</label>' +
                                        '</div></div> ' +
                                        '<div style="padding: 9px 14px;" class="input-group"><input class="form-control search-control" id="buscaIcone" placeholder="Digite para procurar..." type="text">   <span class="input-group-addon"><i class="fa fa-search"></i></span></div>' +
                                        '<div class="popover-content"></div>' +
                                        '<div class="popover-footer"><input type="hidden" id="iconPage" value="1"/></div>' +
                                '</div>'
                    }).popover("show");
                });
            });
            
        });
    };

}(jQuery));

function listarIcons(page, search, diretorio, size) {
    if (size == null) {
        size = $("[data-size][class*=active]").data("size")
    }

    $.ajax({
        url: "/cms/" + diretorio + "/Util/ListarIconPack",
        loadingPanel: false,
        data: {
            search: $("#buscaIcone").val(),
            page: page,
            size: size
        },
        success: function (response) {
            $(".popover-content").html(response);
            $("#iconPage").val(page);
            $(".page-count").html(page);

            $(".btn-arrow").removeClass("disabled");

            var total = parseInt($(".page-total").html());

            if (page == total) {
                $("#btnIconNextPage").addClass("disabled");
            }

            if (page == 1) {
                $("#btnIconPrevPage").addClass("disabled");
            }
        }
    });
}

var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();