(function () {
    var o = {
        exec: function (p) {
            if (p.getData().indexOf("[galeria]") == -1) {
                p.insertHtml("[galeria]");
            }
        }
    };

    CKEDITOR.plugins.add('galeria', {
        init: function (editor) {
            editor.addCommand('galeria', o);
            editor.ui.addButton('galeria', {
                toolbar: 'insert',
                label: 'Inserir Galeria',
                icon: this.path + 'galeria.png',
                command: 'galeria'
            });
        }
    });
})();