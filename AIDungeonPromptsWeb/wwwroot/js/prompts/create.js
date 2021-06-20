document.addEventListener('DOMContentLoaded', () => {
	const addWiButton = document.getElementById('add-wi');
	const uploadWiButton = document.getElementById('upload-wi');
	const wiDeleteBtns = document.getElementsByClassName('world-info-delete-btn');
	ignoreSettingsOnClick(addWiButton);
	ignoreSettingsOnClick(uploadWiButton);
	for (let i = 0; i < wiDeleteBtns.length; i++) {
		ignoreSettingsOnClick(wiDeleteBtns[i]);
	}
});

function ignoreSettingsOnClick(element) {
	if (element) {
		element.addEventListener('click', function () {
			const settings = $.data($('form')[0], 'validator').settings;
			settings.ignore = "*";
		});
	}
}
