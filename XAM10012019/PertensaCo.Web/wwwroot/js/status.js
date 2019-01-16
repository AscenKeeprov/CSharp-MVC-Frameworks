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