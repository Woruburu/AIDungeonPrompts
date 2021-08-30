document.addEventListener('DOMContentLoaded', () => {
	const logOutButton = document.getElementById('log-out-button');
	logOutButton.addEventListener('click', function (event) {
		if (logOutButton.dataset.transient === 'False') {
			return;
		}

		event.preventDefault();
		$('#log-out-modal').modal('show');
	});
});
