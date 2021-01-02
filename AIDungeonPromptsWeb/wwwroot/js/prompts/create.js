document.addEventListener('DOMContentLoaded', () => {
	const addWiButton = document.getElementById('add-wi');
	addWiButton.addEventListener('click', function () {
		const settngs = $.data($('form')[0], 'validator').settings;
		settngs.ignore = "*";
	});
});
