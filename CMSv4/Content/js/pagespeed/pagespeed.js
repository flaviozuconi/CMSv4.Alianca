$(function () {
  $('.knob').knob();
  $('.pill-btns a').click(function() {
    $('.category--collapse').collapse('toggle');
    $('.pill-btns a').toggleClass('active');
  })
});
