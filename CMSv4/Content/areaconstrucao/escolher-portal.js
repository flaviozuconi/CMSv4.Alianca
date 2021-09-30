$(function() {
	//vars gerais
	$.expr[":"].contains = $.expr.createPseudo(function(arg) {
    return function( elem ) {
      return $(elem).text().toUpperCase().indexOf(arg.toUpperCase()) >= 0;
    };
  });
	$('.portal--search').keyup(function() {
    var searchBox    = $('.portal--list');
    var searchString = $(this).val();
		
    searchBox.find('li').addClass('list-hidden');

    var optionFound = searchBox.find('span:contains(' + searchString + ')');
    optionFound.parents('li').removeClass('list-hidden');
		
  });
});
