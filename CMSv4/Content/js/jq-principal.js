
///-------mensgem global ---------------
/*
    type= 
        Error
	    Success
	    Warning
	    Danger
	    OK
*/

function message_box(type, mensagem) {
    //   e.prop("disabled", true);
    $("body").prepend("<div class='message " + type + "'>" +
                        "<h3>Mensagem</h3>" +
                        "<div class='content_message'>" +
                        mensagem +
                        "</div>" +
                        "</div>");
    $(".message").slideDown("300").delay(2500).slideUp("300", function () {
        $(this).remove();
        // e.prop("disabled", false);
    });
}



function MascaraMoedaDinamica(objTextBox, SeparadorMilesimo, SeparadorDecimal, e, casaDecimais) {

    var sep = 0;
    var key = '';
    var i = j = 0;
    var len = len2 = 0;
    var strCheck = '0123456789';
    var aux = aux2 = '';


    if (navigator.appName == 'Microsoft Internet Explorer') {
        var whichCode = e.keyCode;
    } else if (navigator.appName == 'Netscape') {
        var whichCode = e.which;
    } else {
        var whichCode = (window.Event) ? e.which : e.keyCode;
    }

    if ((whichCode == 13) || (whichCode == 0) || (whichCode == 8)) return true;

    key = String.fromCharCode(whichCode); // Valor para o código da Chave
    if (strCheck.indexOf(key) == -1) return false; // Chave inválida
    len = objTextBox.value.length;
    for (i = 0; i < len; i++)
        if ((objTextBox.value.charAt(i) != '0') && (objTextBox.value.charAt(i) != SeparadorDecimal)) break;

    aux = '';

    for (; i < len; i++)
        if (strCheck.indexOf(objTextBox.value.charAt(i)) != -1) aux += objTextBox.value.charAt(i);
    aux += key;
    len = aux.length;


    if (casaDecimais == 0) {
        if (len == 0) objTextBox.value = '';
    } else if (casaDecimais == 1) {

        if (len == 0) objTextBox.value = '';
        if (len == 1) objTextBox.value = '0' + SeparadorDecimal + aux;
    } else if (casaDecimais == 2) {

        if (len == 0) objTextBox.value = '';
        if (len == 1) objTextBox.value = '0' + SeparadorDecimal + '0' + aux;
        if (len == 2) objTextBox.value = '0' + SeparadorDecimal + aux;
    } else if (casaDecimais == 3) {

        if (len == 0) objTextBox.value = '';
        if (len == 1) objTextBox.value = '0' + SeparadorDecimal + '0' + '0' + aux;
        if (len == 2) objTextBox.value = '0' + SeparadorDecimal + '0' + aux;
        if (len == 3) objTextBox.value = '0' + SeparadorDecimal + aux;
    } else if (casaDecimais == 4) {

        if (len == 0) objTextBox.value = '';
        if (len == 1) objTextBox.value = '0' + SeparadorDecimal + '0' + '0' + '0' + aux;
        if (len == 2) objTextBox.value = '0' + SeparadorDecimal + '0' + '0' + aux;
        if (len == 3) objTextBox.value = '0' + SeparadorDecimal + '0' + aux;
        if (len == 4) objTextBox.value = '0' + SeparadorDecimal + aux;
    } else if (casaDecimais == 5) {

        if (len == 0) objTextBox.value = '';
        if (len == 1) objTextBox.value = '0' + SeparadorDecimal + '0' + '0' + '0' + '0' + aux;
        if (len == 2) objTextBox.value = '0' + SeparadorDecimal + '0' + '0' + '0' + aux;
        if (len == 3) objTextBox.value = '0' + SeparadorDecimal + '0' + '0' + aux;
        if (len == 4) objTextBox.value = '0' + SeparadorDecimal + '0' + aux;
        if (len == 5) objTextBox.value = '0' + SeparadorDecimal + aux;
    }


    if (len > casaDecimais) {
        aux2 = '';
        for (j = 0, i = (len - (casaDecimais + 1)) ; i >= 0; i--) {
            if (j == 3) {
                aux2 += SeparadorMilesimo;
                j = 0;
            }
            aux2 += aux.charAt(i);
            j++;
        }
        objTextBox.value = '';
        len2 = aux2.length;
        //alert(len2);
        for (i = (len2 - 1) ; i >= 0; i--)
            objTextBox.value += aux2.charAt(i);
        objTextBox.value += SeparadorDecimal + aux.substr(len - casaDecimais, len);






    }
    return false;
}

function MascaraMoeda(objTextBox, SeparadorMilesimo, SeparadorDecimal, e) {
    var sep = 0;
    var key = '';
    var i = j = 0;
    var len = len2 = 0;
    var strCheck = '0123456789';
    var aux = aux2 = '';
    //var whichCode = (window.Event) ? e.which : e.keyCode;
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var whichCode = e.keyCode;
    } else if (navigator.appName == 'Netscape') {
        var whichCode = e.which;
    }
    if ((whichCode == 13) || (whichCode == 0) || (whichCode == 8)) return true;
    key = String.fromCharCode(whichCode); // Valor para o código da Chave
    if (strCheck.indexOf(key) == -1) return false; // Chave inválida
    len = objTextBox.value.length;
    for (i = 0; i < len; i++)
        if ((objTextBox.value.charAt(i) != '0') && (objTextBox.value.charAt(i) != SeparadorDecimal)) break;
    aux = '';
    for (; i < len; i++)
        if (strCheck.indexOf(objTextBox.value.charAt(i)) != -1) aux += objTextBox.value.charAt(i);
    aux += key;
    len = aux.length;
    if (len == 0) objTextBox.value = '';
    if (len == 1) objTextBox.value = '0' + SeparadorDecimal + '0' + aux;
    if (len == 2) objTextBox.value = '0' + SeparadorDecimal + aux;
    if (len > 2) {
        aux2 = '';
        for (j = 0, i = len - 3; i >= 0; i--) {
            if (j == 3) {
                aux2 += SeparadorMilesimo;
                j = 0;
            }
            aux2 += aux.charAt(i);
            j++;
        }
        objTextBox.value = '';
        len2 = aux2.length;
        for (i = len2 - 1; i >= 0; i--)
            objTextBox.value += aux2.charAt(i);
        objTextBox.value += SeparadorDecimal + aux.substr(len - 2, len);
    }
    return false;
}

function MascaraNumero(objTextBox, e) {
    var sep = 0;
    var key = '';
    var i = j = 0;
    var len = len2 = 0;
    var strCheck = '0123456789';
    var aux = aux2 = '';
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var whichCode = e.keyCode;
    } else if (navigator.appName == 'Netscape') {
        var whichCode = e.which;
    } else {
        var whichCode = (window.Event) ? e.which : e.keyCode;
    }
    
    if ((whichCode == 13) || (whichCode == 0) || (whichCode == 8)) return true;
    key = String.fromCharCode(whichCode); // Valor para o código da Chave
    if (strCheck.indexOf(key) == -1) return false; // Chave inválida
    else {
            return true;
        
    }

}



function validaData(campo, valor) {
    var date = valor;
    var ardt = new Array;
    var ExpReg = new RegExp("(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[12][0-9]{3}");
    ardt = date.split("/");
    erro = false;
    if (date.search(ExpReg) == -1) {
        erro = true;
    }
    else if (((ardt[1] == 4) || (ardt[1] == 6) || (ardt[1] == 9) || (ardt[1] == 11)) && (ardt[0] > 30))
        erro = true;
    else if (ardt[1] == 2) {
        if ((ardt[0] > 28) && ((ardt[2] % 4) != 0))
            erro = true;
        if ((ardt[0] > 29) && ((ardt[2] % 4) == 0))
            erro = true;
    }
    if (erro) {
        message_box("Error", valor + " não é uma data válida!!!");
        campo.focus();
        campo.value = "";
        return false;
    }
    return true;
}


function setCookie(c_name, value, exdays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + exdays);
    var c_value = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
    document.cookie = c_name + "=" + c_value;
}

function getCookie(c_name) {
    var i, x, y, ARRcookies = document.cookie.split(";");
    for (i = 0; i < ARRcookies.length; i++) {
        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
        x = x.replace(/^\s+|\s+$/g, "");
        if (x == c_name) {
            return unescape(y);
        }
    }
}

