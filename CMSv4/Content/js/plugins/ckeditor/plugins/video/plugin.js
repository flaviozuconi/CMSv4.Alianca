(function () {
    CKEDITOR.plugins.add('video', {
        init: function (editor) {
            editor.addCommand('videoDialog', new CKEDITOR.dialogCommand('videoDialog'));
            editor.ui.addButton('video', {
                toolbar: 'insert',
                label: 'Inserir Vídeo',
                command: 'videoDialog',
                icon: this.path + 'video.png'
            });
        }
    });

    CKEDITOR.dialog.add('videoDialog', function (editor) {
        return {
            title: 'Vídeo',
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
                            label: 'Selecione um Vídeo',
                            style: 'width: 250px;',
                            items: $.getVideos('/cms/Principal/ListaAdmin/ListarVideo'),
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

                //Criar tag do VIDEO selecionado
                editor.insertText("[video]" + String(data.Guid) + "[/video]");
            },
            onShow: function () {
                if ($("#CodigoLista").val() == "") {
                    alert("É necessário finalizar o cadastro para adicionar vídeos no conteúdo.");
                    CKEDITOR.dialog.getCurrent().hide();
                }
            }
        };
    });
})();

jQuery.extend({
    getVideos: function (url) {
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
            }
        });

        return array;
    }
});