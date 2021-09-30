/**
 * @license Copyright (c) 2003-2014, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.dialog.add('sourcedialog', function (editor) {
    var aEdit = null;

	return {
		title: editor.lang.sourcedialog.title,
		minWidth: 700,
		minHeight: 520,
		onOk: (function (data) {
		    CKEDITOR.instances[editor.name].setData(aEdit.getSession().getValue());
		}),
		contents: [ {
			id: 'main',
			label: editor.lang.sourcedialog.title,
			elements:
            [
                {  
				    type : 'iframe',
				    src: '/Content/js/plugins/ckeditor/plugins/sourcedialog/samples/sourcedialog.html',
				    width : '700px',
				    height: '480px',
				    onContentLoad: function () {
                        //Define o texto no Ace Editor do dialog
				        document.getElementById(this._.frameId).contentWindow.aceEditor.getSession().setValue(CKEDITOR.instances[editor.name].getData(), 1);
				        aEdit = document.getElementById(this._.frameId).contentWindow.aceEditor;
				        aEdit.navigateFileEnd();
				    }
                }
            ]
		} ]
	};
});