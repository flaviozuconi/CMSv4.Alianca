// Date time picker
Date.parseDate = function (input, format) {
    return moment(input, format).toDate();
};
Date.prototype.dateFormat = function (format) {
    return moment(this).format(format);
};

// Validation
$.validator.setDefaults({
    highlight: function (element) {
        $(element).closest('.form-group').addClass('has-error');
    },
    unhighlight: function (element) {
        $(element).closest('.form-group').removeClass('has-error');
    },
    errorElement: 'span',
    errorClass: 'help-block',

    //Override do onkeyup padrão, para que seja possível desativar o keyup somente em campos especificpos
    onkeyup: function (element) {
        var element_id = $(element).attr('name');

        if (this.settings.rules[element_id].onkeyup !== false) {
            if (event.which === 9 && this.elementValue(element) === "") {
                return;
            } else if (element.name in this.submitted || element === this.lastElement) {
                this.element(element);
            }
        }
    }
});

$.validator.addMethod(
        "regex",
        function (value, element, regexp) {
            var re = new RegExp(regexp);
            return this.optional(element) || re.test(value);
        },
        $.validator.messages.remote
);

$.validator.addMethod(
        "comparedata",
        function (value, element, p) {
            var valcompare = $('#' + p.elementcompare).val();
            var expr = p.expr;            
            if (value == "")
                return true;
            var msg = "";
            if (valcompare == "")
                return true;

            var data1 = moment(value, "DD/MM/YYYY HH:mm").toDate();
            var data2 = moment(valcompare, "DD/MM/YYYY HH:mm").toDate();

            if (expr == '<') 
                return data1 < data2;                        
            if (expr == '>') 
                return data1 > data2;
            if (expr == '<=')
                return data1 <= data2;
            if (expr == '>=')
                return data1 >= data2;
            var msg = "teste2";
        },        
        function(params, element) {            
            return params.msg;
        }   
        
);

jQuery.validator.addMethod("greaterThan",
    function (value, element, params) {
        var vParam = removerMask($(params).val());

        value = removerMask(value);

        if (value && vParam) {
            value = toDate(value);
            vParam = toDate(vParam);

            return new Date(value) >= new Date(vParam);
        }

        return true;
    }, 'Data de Término deve ser maior ou igual a Data de Início.'
);


// Data Tables
$.extend($.fn.dataTable.defaults, {
    "dom": '<"top"fl><"table-toolbar">rt<"bottom"ip>',
    "paging": true,
    "lengthMenu": [[10, 20, 50, 100], [10, 20, 50, 100]],
    "pageLength": 50,
    "fnDrawCallback": function () {
        if ($('.tooltip-idioma').length) {
            try{
                $('.tooltip-idioma').tooltip();
            }
            catch (err) { }
        }

        scrollToTop();
    },
    "footerCallback": function () {
        if ($("#toolbar")) { $("#toolbar").appendTo("div.table-toolbar").show(); }
        if ($("#filter")) { $("#filter").appendTo("div.top").show(); }

        var botao = $('#excluir');

        if (botao) {
            //Validar em todas as paginações se há alguma checkbox selecionada
            var values = getAllValues('input[name="excluir"][type="checkbox"]:checked');

            //Em nenhuma paginação há um item selecionado
            if (values.length == 0) { botao.hide(); }

            $('input[name="excluir"][type="checkbox"]').change(function () {
                showExcluir($(this));
            });
        }
    },
    "stateSave": false,
    "serverSide": true,
    'language': { 'url': arquivotraducaodatatable },
    "drawCallback": function() {
        //Aplicar multi seleção pressionando a tecla shift
        $("#" + $(this).attr("id")).find('input[type=checkbox]').shiftcheckbox({
            //Chamado quando selecionar checkbox com a tecla shift
            onChange: function (checked) {
                showExcluir($(this));
            }
        });
    }
});

//Verificação no json antes de contruir as linhas para verificar se o usuário está logado.
$("table.table").on('xhr.dt', function (e, settings, json, xhr) {
    if ($(this).data('default-before-render') == false) {
        return;
    }

    var tableId = $(this).prop('id');
    var panelId = tableId + "_aviso_sessao";
    var divTable = $("#" + tableId).closest('.table-responsive');

    //Remover div antes da verificação, para caso o usuário realize o login em outra aba.
    $('#' + panelId).remove();

    if (json.ExpirouSessao != null && json.ExpirouSessao == true) {
        if (divTable != null && divTable.length > 0) {
            
            $("#" + tableId).closest('.table-responsive').before("<div id='" + panelId + "' class='col-md-12 alert alert-warning'>" + json.Mensagem + "</div>");
        }
        else {
            alert(json.Mensagem);
        }
    }

    $('#spinner').fadeOut();
});

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

function showExcluir(me) {
    var botao = $('#excluir');

    botao.hide();

    //Validar em todas as paginações se há alguma checkbox selecionada
    var values = getAllValues('input[name="excluir"][type="checkbox"]:checked');

    //Em nenhuma paginação há um item selecionado
    if (values.length > 0) { botao.show(); }

    if ($(me).is(":checked")) {
        $(me).parents('tr').addClass('selected');
    } else {
        $(me).parents('tr').removeClass('selected');
    }
}

// ACE EDITOR
$(function () {
    $('textarea[data-editor]').each(function () {
        var textarea = $(this);

        var mode = textarea.data('editor');

        var editDiv = $('<div>', {
            position: 'absolute',
            width: textarea.width(),
            height: textarea.height(),
            'class': textarea.attr('class')
        }).insertBefore(textarea);

        textarea.css('visibility', 'hidden');
        textarea.css('display', 'none');

        var editor = ace.edit(editDiv[0]);
        editor.setTheme("ace/theme/chrome");
        editor.getSession().setMode("ace/mode/" + mode);

        if (textarea.val() != "undefined" && textarea.val().length > 0) {
            editor.getSession().setValue(textarea.val());
        }

        // copy back to textarea on form submit...
        editor.on("blur", function () {
            textarea.val(escape(editor.getSession().getValue()));
        });

        textarea.closest('form').submit(function () {
            textarea.val(escape(editor.getSession().getValue()));
        });
    });
});

// CKEDITOR
$(document).ready(function () {
    var roxyFileman = '/Content/js/plugins/ckeditor/plugins/fileman/Index?integration=ckeditor';

    $('textarea[data-ckeditor]').each(function () {
        var sigla = getParameterByName("sigla", document.getElementById("flexInternaJs").src);

        var textarea = $(this);
        var portal = textarea.data('ckeditor');
        var IsEscape = $(this).attr("IsEscape");
        var plugins = textarea.data('plugins');
        var arrayPlugins = [];
        if (plugins != null && plugins != '') {
            arrayPlugins = plugins.split(',');
        }

        var editor = textarea.ckeditor(
        {
            language: sigla,
            height: textarea.height(),
            contentsCss: '/portal/' + portal + '/content/css/editor_html.css',            
            filebrowserBrowseUrl: roxyFileman,
            filebrowserImageBrowseUrl: roxyFileman,
            extraPlugins: "iframedialog,sourcedialog",
            
            allowedContent: true,
            autoParagraph: false,
            htmlEncodeOutput: false,
            entities: false,
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
                   { name: 'tools', items: ['UIColor', 'Maximize', 'ShowBlocks'] },

                   { name: 'plugins', items: arrayPlugins }
               ]

        });
		
		editor.editor.config.font_names = 'DINOT/dinot;' + editor.editor.config.font_names;
		editor.editor.config.skin = "moono-lisa";

        if (IsEscape == null) IsEscape == "true";

        if (IsEscape == "true") {
            textarea.closest('form').submit(function () {
                if ($(this).valid()) {
                    textarea.val(escape(textarea.val()));
                }
            });
        }
        
    });
});

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
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
            this.move(this.getPosition().x, window.parent.$(window.parent.document).scrollTop());
        };

        if (dialogName == "sourcedialog") {
            var contents = $(dialogDefinition.dialog.parts["contents"]);

            contents.attr("class", contents.attr("class") + " codigo-fonte"); //adicionar classe para definição de estilo da janela
        }
    });
} catch (e) { }

/*
*   LOG DE AUDITORIA
*/
function logAuditoria(funcionalidade, codigo, pularPortal) {
    var dataSend = { 'funcionalidade': funcionalidade, 'codigo': codigo, 'pularPortal': pularPortal };

    $.ajax({
        url: '/admin/auditoria',
        data: dataSend,
        success: function (data) {
            $('#logModal').empty();
            $('#logModal').html(data);
        },
        error: function (data) {
        }
    });

    return false;
}

/*
*   Preencher a caixa de texto de URL amigavel
*/
function UrlAmigavel(value) {

    var url = value.toLowerCase();

    url = url.replace(/á/g, "a");
    url = url.replace(/é/g, "e");
    url = url.replace(/í/g, "i");
    url = url.replace(/ó/g, "o");
    url = url.replace(/ú/g, "u");

    url = url.replace(/ã/g, "a");
    url = url.replace(/õ/g, "o");

    url = url.replace(/à/g, "a");
    url = url.replace(/è/g, "e");
    url = url.replace(/ì/g, "i");
    url = url.replace(/ò/g, "o");
    url = url.replace(/ù/g, "u");

    url = url.replace(/â/g, "a");
    url = url.replace(/ê/g, "e");
    url = url.replace(/î/g, "i");
    url = url.replace(/ô/g, "o");
    url = url.replace(/û/g, "u");

    url = url.replace(/ä/g, "a");
    url = url.replace(/ë/g, "e");
    url = url.replace(/ï/g, "i");
    url = url.replace(/ö/g, "o");
    url = url.replace(/ü/g, "u");

    url = url.replace(/ç/g, "c");

    url = url.replace(/[^a-zA-Z0-9-_\s]/g, "");
    url = url.replace(/_|\s/g, "-");
    url = url.replace(/-{2,}/g, "-");
    url = url.replace(/^-/g, "");
    url = url.replace(/-$/g, "");

    return url;
}

//function todosProdutos

//seleciona todos os checkboxes que tem o NAME igual ao parametro
function todos(className, attrName) {
    if (className) {
        $("." + className).prop("checked", true);
    }
    else {
        $("input[name*=" + attrName + "][type=checkbox]").prop("checked", true);
    }
}

//desseleciona todos os checkboxes que tem o NAME igual ao parametro
function nenhum(className, attrName) {
    if (className) {
        $("." + className).prop("checked", false);
    }
    else {
        $("input[name*=" + attrName + "][type=checkbox]").prop("checked", false);
    }
}

function toDate(valor) {
    if (!valor) {
        return "";
    }

    return new Date(valor.replace(/(\d{2})\/(\d{2})\/(\d{4}) (\d{2}):(\d{2})/, "$2/$1/$3 $4:$5"));
}

function removerMask(valor) {
    if (valor.indexOf("_") > -1) {
        return "";
    }

    return valor;
}

function executeFunctionByName(functionName, context /*, args */) {
    var args = Array.prototype.slice.call(arguments, 2);
    var namespaces = functionName.split(".");
    var func = namespaces.pop();
    for (var i = 0; i < namespaces.length; i++) {
        context = context[namespaces[i]];
    }
    return context[func].apply(context, args);
}

$(function () {
    function reposition() {
        var modal = $(this),
            dialog = modal.find('.modal-dialog');
        modal.css('display', 'block');
        dialog.css("margin-top", Math.max(0, $(window.parent.document).scrollTop()));
    }
    // Reposicionar a modal ao abrir
    $(document).on('show.bs.modal', '.modal', reposition);
});