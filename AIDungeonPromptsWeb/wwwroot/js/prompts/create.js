document.addEventListener('DOMContentLoaded', (event) => {
	var addWiButton = document.getElementById('add-wi');
	addWiButton.addEventListener('click', function () {
		var settngs = $.data($('form')[0], 'validator').settings;
		settngs.ignore = "*";
	});
});
