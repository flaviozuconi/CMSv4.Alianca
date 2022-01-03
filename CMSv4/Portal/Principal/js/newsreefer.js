$("#btnNews").click(function(e){
  e.preventDefault();
  salvarNewsLetter($("#formNews"), $(this));
});

function salvarNewsLetter(form, callAction){
        //var form = $("#form-news");
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        console.log(token);
        var inputs = form.find("input, textarea");
        var invalid = inputValidation(inputs);
        
        if (!invalid) {
            var email = $("#emailNews").val();    
            callAction.html('Enviando');            
            callAction.attr("disabled","disabled");
            form.ajaxSubmit({
            //    $ajax({
                type: "POST",
                url: "/cms/principal/FaleConosco/Salvar",
                data: {nome : email , email : email, assunto : "News Reefer",urlpagina:"reefer",formulario:"ReeferNews"},
                dataType: "json",               
                //contentType:'application/x-www-form-urlencoded; charset=utf-8',
                success: function (data) { 
                    
                    if(data.Sucesso){
                      form.prepend("<div class='alert alert-success msg-contato'>" + "Download realizado!" + "</div>");                     
                      $("#btnDownload").attr("href","/portal/principal/arquivos/Transporte_de_Cargas_Resfriadas.pdf");
                      $('#btnDownload')[0].click();
                    }
                    else{
                        console.log(data);
                        form.prepend("<div class='alert alert-danger msg-contato'>" + "Não foi possível realizar seu cadastro!" + "</div>");
                    }
                },
                error: function (err,response) {
                    console.log(err);
                    console.log(response);
                    alert("Não foi possível realizar seu cadastro");
                },
                complete: function () {
                    callAction.html('Saiba mais baixando nosso guia');            
                    callAction.removeAttr("disabled");                                      
                }
            });
        }
}
