var statusAlert = $('#statusAlert');
var statusMessage = $('#statusMessage');
var statusRow = $('#statusRow');
function ClearStatus() {
	statusMessage.empty();
	statusAlert.removeClass('alert-danger', 'alert-success');
	statusRow.removeClass('d-flex').addClass('d-none');
}
function DisplayError(message) {
	statusMessage.html(message);
	statusAlert.removeClass('alert-success').addClass('alert-danger');
	statusRow.removeClass('d-none').addClass('d-flex');
}
function DisplayInfo(message) {
	statusMessage.html(message);
	statusAlert.removeClass('alert-danger').addClass('alert-success');
	statusRow.removeClass('d-none').addClass('d-flex');
}
$(function () {
	statusAlert.on('close.bs.alert', function (event) {
		event.preventDefault();
		ClearStatus();
	});
});
$(function () {
	$('form').submit(function (event) {
		event.preventDefault();
		ClearStatus();
		var action = $(this).attr('action');
		var errors = $('.field-validation-error');
		var formData = new FormData(this);
		var method = $(this).attr('method');
		if (errors.length == 0) {
			$.ajax({
				contentType: false,
				data: formData,
				processData: false,
				type: method,
				url: action,
				success: function (response) {
					var message = response.message;
					DisplayInfo(message);
					$('select').val('').trigger('change');
				},
				error: function (data) {
					var message = data.responseJSON.message;
					DisplayError(message);
				}
			});
		}
	});
});