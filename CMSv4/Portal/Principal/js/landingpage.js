$(function(){

  header();
  videoPlay();
	mapa();

	$('.anchor').click(function(e) {
		e.preventDefault();
		var item = $(this).attr('href');

		$('html,body').animate({scrollTop: $(item).offset().top},'slow');
	});
	$('.header .anchor').click(function() {
		$('.nav-toggle').removeClass('open');
    $('.header .nav').removeClass('open');
    $('body, .header').removeClass('opened-menu');
	});

	$('.nav-toggle').click(function(e) {
    e.preventDefault();

    $(this).toggleClass('open');
    $('.header .nav').toggleClass('open');
    $('body, .header').toggleClass('opened-menu');
  });

	$('.banner .owl-carousel').owlCarousel({
		items: 1,
		nav: true,
		dots: true,
		autoplay: true,
		loop: true,
		smartSpeed: 1000,
		autoplayTimeout: 8000,
		navText: ['<span></span>','<span></span>'],
		responsive: {
			0: {
				nav: false
			},
			768: {
				nav: true
			}
		}
	});

	if ($(window).width() < 768) {
		$('#servicos .owl-carousel').owlCarousel({
			items: 1,
			dots: true,
			autoplay: true,
			loop: true,
			smartSpeed: 1000,
			autoplayTimeout: 8000
		});
	}

	$(window).resize(function() {
		mapa();
	})
});

function header() {
	var lastScrollTop = 0;
	// if ($(window).width() >= 992) {
		$(window).scroll(function(event){
		  var st = $(this).scrollTop();
		  if (st > lastScrollTop){
		    $('.header, .lp-header').removeClass('show');
		  } else {
		    $('.header, .lp-header').addClass('show');
		  }
		  if (st === 0) {
		  	$('.header, .lp-header').removeClass('bg');
		  }
		  else {
		  	$('.header, .lp-header').addClass('bg');
		  }
		  lastScrollTop = st;
		});
	// }
}
function videoPlay() {
	$('.video .item-thumb').click(function() {
		var thumb = $(this);
		var video = $(this).siblings('video');

		thumb.addClass('fade');
		setTimeout(function(){
			thumb.addClass('hide');
		}, 600);
		video.get(0).play();
	})
}
function mapa() {
	if ($(window).width() >= 992) {
		$('.mapa .item').hover(function() {
			$(this).find('.item-txt').toggleClass('open');
		})
	}
	else {
		$('.mapa .info--txt').addClass('owl-carousel');

		$('.mapa .owl-carousel').owlCarousel({
			items: 1,
			nav: false,
			dots: true,
			autoplay: true,
			loop: true
		});
	}
}
function navLink() {
	$('.header .anchor').click(function(e) {
		$('.nav-toggle').toggleClass('open');
    $('.header .nav').toggleClass('open');
    $('body, .header').toggleClass('opened-menu');
	});
}