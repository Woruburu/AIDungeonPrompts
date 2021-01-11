document.addEventListener('DOMContentLoaded', () => {
	const addWiButton = document.getElementById('add-wi');
	const uploadWiButton = document.getElementById('upload-wi');
	const saveDraft = document.getElementById('save-draft');
	ignoreSettingsOnClick(addWiButton);
	ignoreSettingsOnClick(saveDraft);
	ignoreSettingsOnClick(uploadWiButton);
});

function ignoreSettingsOnClick(element) {
	if (element) {
		element.addEventListener('click', function () {
			const settngs = $.data($('form')[0], 'validator').settings;
			settngs.ignore = "*";
		});
	}
}
