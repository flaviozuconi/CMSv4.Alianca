$(document).ready(function () {
    $(".knob").knob();
});

$("#btnExecutarNovaAnalise").click(function () {
    var codigoPagina = $(this).data("pagina");

    $.get('/cms/' + portalDiretorio + '/PageSpeed/RunAnalysis', { CodigoPagina: codigoPagina }, function (response) {

        if (NaoOcorreuErroCms(response) && NaoOcorreuErroPageSpeed(response)) {
            Successo(response);
        }
    });
});

function NaoOcorreuErroCms(response) {
    if (response.Sucesso != null && !response.Sucesso) {
        ExibirModalComMensagemDeErro(response.Mensagem);
        return false;
    }

    return true;
}

function NaoOcorreuErroPageSpeed(response) {
    if (response.Error != null && response.Error.error != null && response.Error.error.code > 0) {
        ExibirModalComMensagemDeErro(response.Error.error.message);
        return false;
    }

    return true;
}

function ExibirModalComMensagemDeErro(mensagem) {
    window.parent.$.fn.ModalPadrao({
        titulo: "Erro",
        mensagem: mensagem,
        exibirNaInicializacao: true
    });
}

function Successo(response) {
    AtualizarInputs(response.AnalysisResult);

    atualizarDataUltimaVerificacao();

    exibirDivResultadoEOcultarDivNenhumRegistro();
}

function AtualizarInputs(analysisResult) {

    $.get('/cms/' + portalDiretorio + '/PageSpeed/ScoreToHexaColor', {
        performanceScore:   analysisResult.LighthouseResult.Categories.Performance.Score,
        pwaScore:           analysisResult.LighthouseResult.Categories.Pwa.Score,
        accessibilityScore: analysisResult.LighthouseResult.Categories.Accessibility.Score,
        bestPracticesScore: analysisResult.LighthouseResult.Categories.BestPractices.Score,
        seoScore:           analysisResult.LighthouseResult.Categories.Seo.Score
    }, function (response) {

        RedrawGrafico("#knobPerformance", analysisResult.LighthouseResult.Categories.Performance.Score, response.performanceColor);
        RedrawGrafico("#knobPwa", analysisResult.LighthouseResult.Categories.Pwa.Score, response.pwaColor);
        RedrawGrafico("#knobAccessibility", analysisResult.LighthouseResult.Categories.Accessibility.Score, response.accessibilityColor);
        RedrawGrafico("#knobBestPractices", analysisResult.LighthouseResult.Categories.BestPractices.Score, response.bestPracticesColor);
        RedrawGrafico("#knobSeo", analysisResult.LighthouseResult.Categories.Seo.Score, response.seoColor);
        
    });
}

function RedrawGrafico(selector, score, color) {
    $(selector).val(score * 100)
        .trigger("change")
        .trigger("configure", {
            "fgColor": color,
            "inputColor": color
    });
}

function atualizarDataUltimaVerificacao() {
    var data = new Date();
    $("#pDataUltimaModificacao").text(data.toLocaleString());
}

function exibirDivResultadoEOcultarDivNenhumRegistro() {
    $("#divResultadoAnalise").show();
    $("#divAnaliseNaoRealizada").hide();
}
