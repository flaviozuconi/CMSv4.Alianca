 $(document).ready(function () {
    //abre o campo de pesquisa resoluções < 768px
    $('.search-header .icon-search').click(function(){
	    $('.search-header').addClass('open');
        $('.search-header .form-control').focus();
    });
    //fecha o campo de pesquisa
    $('.search-header .search-submit').click(function(){
	    $('.search-header').removeClass('open');
    });


    //rola a pagina para o topo
    $('.go-top').click(function(){
        $('html, body').animate({scrollLeft:0}, 'slow');
        return false;
    });


    //adiciona classe no elemento pai do menu principal quando o filho está ativo
    $('.navbar .dropdown').on('shown.bs.dropdown', function () {
        $(this).parents('.navbar').addClass('active');
    });
    //remove classe no elemento pai do menu principal quando o filho está ativo
    $('.navbar .dropdown').on('hidden.bs.dropdown', function () {
        $(this).parents('.navbar').removeClass('active');
    });
    //para correções de posicionamento do link
    $('.nav-justified>li').click(function(){
        var link = $(this).children('a').attr('href');
        window.location.href = link;
    });


    //step carousel 
    var owl = $('.step-carousel--container');
    owl.owlCarousel({
        items      : 3, //3 items above 1000px browser width
        //itemsDesktop : [1000,5], //5 items between 1000px and 901px
        //itemsDesktopSmall : [900,3], // betweem 900px and 601px
        //itemsTablet: [600,2], //2 items between 600 and 0
        //itemsMobile : false // itemsMobile disabled - inherit from itemsTablet option
        pagination : false,
        responsive : false
    });
    //navegação step carousel
    $('.step-carousel--right').click(function(){
        owl.trigger('owl.next');
    });
    $('.step-carousel--left').click(function(){
        owl.trigger('owl.prev');
    });


    //define altura do controle do carousel de fotos (album-carousel) a partir da altura das imagens dos itens
    $('.album-carousel .container-controls').height(parseInt($('.album-carousel .item img').outerHeight()));;


    //plugins para calendário
    if ($('.calendar-datepicker').length) {
        $('.calendar-datepicker').datepicker({
            showOtherMonths: true,
            selectOtherMonths: true,
            onSelect: function (selectedDate) {
            }
        });
    }

    //scroll eventos do calendário
    $('.calendar-events').mCustomScrollbar({
        scrollInertia: 300
    });

    //tooltip para botões da página resultado--interna
    $('[data-toggle="tooltip"]').tooltip()
});