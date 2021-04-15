document.addEventListener('DOMContentLoaded', () => {
	$('#delete-form').on('submit', (event) => {
		event.preventDefault();
		$('#confirm-delete-modal').modal('show');
	})
});
