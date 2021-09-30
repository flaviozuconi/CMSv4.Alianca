/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    config.allowedContent = true;
    CKEDITOR.dtd.$removeEmpty['i'] = false;
    CKEDITOR.dtd.$removeEmpty['span'] = false;
    CKEDITOR.dtd.$removeEmpty['b'] = false;
    CKEDITOR.dtd.a.div = 1;
    CKEDITOR.dtd.a.h2 = 1;
    CKEDITOR.dtd.a.h1 = 1;
    CKEDITOR.dtd.a.h3 = 1;
    CKEDITOR.dtd.a.p = 1;
    CKEDITOR.dtd.a.ul = 1;
    CKEDITOR.dtd.a.li = 1;
    CKEDITOR.dtd.a.table = 1;
    CKEDITOR.dtd.a.b = 1;
    CKEDITOR.dtd.span.h4 = 1;
    CKEDITOR.dtd.a.i = 1;
    CKEDITOR.dtd.a.span = 1;
    CKEDITOR.dtd.span.h1 = 1;
    CKEDITOR.dtd.span.h2 = 1;
    CKEDITOR.dtd.span.h3 = 1;
    CKEDITOR.dtd.span.h4 = 1;
    CKEDITOR.dtd.span.h5 = 1;
    CKEDITOR.dtd.span.p = 1;
    CKEDITOR.dtd.span.a = 1;

	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
};
