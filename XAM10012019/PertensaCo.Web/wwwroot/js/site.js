$(function () {
	if ($('#sidebar').length == 0) {
		$('.sidebar-toggler').remove();
	} else {
		$('.navbar-brand').remove();
	}
});
$(function () {
	$('.sidebar-toggler').click(function () {
		window.scrollTo(0, 0)
	});
});
$(function () {
	$('.nav-button').click(function () {
		$('#filing')[0].play();
		var icon = $(this).children('.nav-button-icon');
		if ($(this).hasClass('collapsed') || $(this).attr('aria-expanded') == 'false') {
			icon.addClass('fa-rotate-90');
		} else {
			icon.removeClass('fa-rotate-90');
		}
	});
});
$(function () {
	$('#errorAlert').on('close.bs.alert', function () {
		$('#errorRow').remove();
	});
});
$(function () {
	$('#infoAlert').on('close.bs.alert', function () {
		$('#infoRow').remove();
	});
});