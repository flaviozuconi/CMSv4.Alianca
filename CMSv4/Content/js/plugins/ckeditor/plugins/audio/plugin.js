(function () {
    CKEDITOR.plugins.add('audio', {
        init: function (editor) {
            editor.addCommand('audioDialog', new CKEDITOR.dialogCommand('audioDialog'));
            editor.ui.addButton('audio', {
                toolbar: 'insert',
                label: 'Inserir Audio',
                command: 'audioDialog',
                icon: this.path + 'audio.png'
            });
        }
    });

    CKEDITOR.dialog.add('audioDialog', function (editor) {
        return {
            title: 'Áudio',
            minWidth: 400,
            minHeight: 100,
            contents:
            [
                {
                    id: 'general',
                    label: 'Settings',
                    elements:
                    [
                        {
                            type: 'select',
                            id: 'audio',
                            label: 'Selecione um Áudio',
                            style: 'width: 250px;',
                            items: $.getValues('/cms/Principal/ListaAdmin/ListarAudio'),
                            required: true,
                            commit: function (data) {
                                data.Guid = this.getValue();
                            }
                        }
                    ]
                }
            ],
            onOk: function (data) {
                var dialog = this, data = {};

                //Recebe as informações da popup no objeto data
                this.commitContent(data);

                //Criar tag do audio selecionado
                editor.insertText("[audio]" + String(data.Guid) + "[/audio]");
            },
            onShow: function () {
                if ($("#CodigoLista").val() == "") {
                    CKEDITOR.dialog.getCurrent().hide();
                    alert("É necessário finalizar o cadastro para adicionar um Áudio no conteúdo.");
                } 
            }
        };
    });
})();

jQuery.extend({
    getValues: function (url) {
        var array = [[]];

        $.ajax({
            url: url,
            type: 'POST',
            data: { "id": $("#CodigoLista").val(), "guid": $("#GUID").val() },
            dataType: 'json',
            async: false,
            success: function (element) {
                
                for (var i = 0; i < element.data.length; i++) {
                    array[i] = new Array(element.data[i].Titulo, String(element.data[i].Guid));
                }
                String.prototype.unquoted = function () { return this.replace(/(^")|("$)/g, '') }

                return array;
            },
        });

        return array;
    }
});

//CKEDITOR.on('dialogDefinition', function (e) {
//    var dialogName = e.data.name;
//    var dialogDefinition = e.data.definition;
//    var onShow = dialogDefinition.onShow;
//    dialogDefinition.onShow = function () {
//        var result = onShow.call(this);
//        this.move(this.getPosition().x, $(e.editor.container.$).position().top + 300);
//        return result;
//    }
//});