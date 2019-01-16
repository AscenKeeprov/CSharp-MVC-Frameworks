$(function () {
	$('.checkbox').each(function () {
		if ($(this).is(':checked')) $(this).val(true);
		else $(this).val(false);
	})
	$('.checkbox').change(function () {
		var checkboxName = $(this).attr('name');
		var checkcircle = $(this).siblings('label')
			.children('span').filter('.checkcircle');
		var hiddenCheckbox = $('input[type=hidden][name=' + checkboxName + ']');
		if ($(this).is(':checked')) {
			$(this).val(true);
			hiddenCheckbox.val(true);
			checkcircle.removeClass('fa-circle');
			checkcircle.addClass('fa-times-circle');
		} else {
			$(this).val(false);
			hiddenCheckbox.val(false);
			checkcircle.removeClass('fa-times-circle');
			checkcircle.addClass('fa-circle');
		}
	});
});