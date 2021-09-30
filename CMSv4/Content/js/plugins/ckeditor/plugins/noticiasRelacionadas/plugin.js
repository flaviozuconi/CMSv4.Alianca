(function () {
    var o = {
        exec: function (p) {
            if (p.getData().indexOf("[noticiasRelacionadas]") == -1) {
                p.insertHtml("[noticiasRelacionadas]");
            }
        }
    };

    CKEDITOR.plugins.add('noticiasRelacionadas', {
        init: function (editor) {
            editor.addCommand('noticiasRelacionadas', o);
            editor.ui.addButton('noticiasRelacionadas', {
                toolbar: 'insert',
                label: 'Inserir Notícias Relacionadas',
                icon: this.path + 'noticiasRelacionadas.png',
                command: 'noticiasRelacionadas'
            });
        }
    });
})();