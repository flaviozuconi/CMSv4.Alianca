$(function(){
	calendar();
	calendarHighligh();
});

function calendar() {
	if($(window).width() > 1200) {
		$('#calendarLeft').fullCalendar({
			header: {
			  left:   '',
			  center: 'title',
			  right:  ''
			},
			navLinks: false,
		    contentHeight:"auto",
		    firstDay: 1,
		    events: [
		    {
		      id: 'a',
		      title: 'my event',
		      start: '2020-11-04',
		    },
		    {
		      id: 'b',
		      title: 'my event 2',
		      start: '2020-11-04',
		    },
		    {
		      id: 'c',
		      title: 'my event 3',
		      start: '2020-11-05',
		    }
		  ]
		})
	}
	$('#calendarCenter').fullCalendar({
		header: {
		  left:   '',
		  center: 'title',
		  right:  ''
		},
	    contentHeight:"auto",
	    firstDay: 1,
	    events: [
	    {
	      id: 'a',
	      title: 'my event',
	      start: '2020-11-04',

	    },
	    {
	      id: 'b',
	      title: 'my event 2',
	      start: '2020-11-04',

	    },
	    {
	      id: 'c',
	      title: 'my event 3',
	      start: '2020-11-05',

	    }
	  ]
	})
	if($(window).width() > 1200) {
		$('#calendarRight').fullCalendar({
			header: {
			  left:   '',
			  center: 'title',
			  right:  ''
			},
			navLinks: false,
		    contentHeight:"auto",
		    firstDay: 1,
		    events: [
		    {
		      id: 'a',
		      title: 'my event',
		      start: '2020-11-04',
		    },
		    {
		      id: 'b',
		      title: 'my event 2',
		      start: '2020-11-04',
		    },
		    {
		      id: 'c',
		      title: 'my event 3',
		      start: '2020-11-05',
		    }
		  ]
		})
	}

	$('#calendarLeft').fullCalendar('prev');
	$('#calendarRight').fullCalendar('next');

	$('.calendar .btn-prev').click(function(e) {
		e.preventDefault();

		$('#calendarLeft').fullCalendar('prev');
		$('#calendarCenter').fullCalendar('prev');
		$('#calendarRight').fullCalendar('prev');

		calendarHighligh();
	})
	$('.calendar .btn-next').click(function(e) {
		e.preventDefault();

		$('#calendarLeft').fullCalendar('next');
		$('#calendarCenter').fullCalendar('next');
		$('#calendarRight').fullCalendar('next');

		calendarHighligh();
	})
}
function calendarHighligh() {
	$('.fc-content-skeleton > table > tbody > tr:first-child .fc-event-container').each(function() {
		var index = $(this).index();

		$(this).parents('.fc-content-skeleton').find('thead tr td').eq(index).find('.fc-day-number').addClass('tem-evento');
	})
}