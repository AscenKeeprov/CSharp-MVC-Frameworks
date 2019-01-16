$(function () {
	$('select').change(function () {
		$(this).children('option').each(function () {
			if ($(this).is(':selected')) {
				$(this).attr('selected', 'selected');
			} else {
				$(this).removeAttr('selected');
			}
		});
	});
});
$(function () {
	$('option').click(function () {
		if ($(this).is(':selected')) {
			$(this).attr('selected', 'selected');
		} else {
			$(this).removeAttr('selected');
		}
	});
});