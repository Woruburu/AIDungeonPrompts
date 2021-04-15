document.addEventListener('DOMContentLoaded', () => {
	const deleteBtn = document.getElementById('sub-scenario-container');
	showModel(deleteBtn);
});

function showModel(element) {
	if (element) {
		element.addEventListener('click', function (event) {
			const target = event.target;
			if (target.classList.contains('btn-delete')) {
				event.preventDefault();
				$('#prompt-delete-modal').modal('show');
				document.getElementById('prompt-delete-modal-confirm').formAction = target.formAction;
			}
		});
	}
}
