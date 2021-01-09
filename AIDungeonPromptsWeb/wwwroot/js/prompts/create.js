document.addEventListener('DOMContentLoaded', () => {
	const addWiButton = document.getElementById('add-wi');
	const saveDraft = document.getElementById('save-draft');
	ignoreSettingsOnClick(addWiButton);
	ignoreSettingsOnClick(saveDraft);
});

function ignoreSettingsOnClick(element) {
	element.addEventListener('click', function () {
		const settngs = $.data($('form')[0], 'validator').settings;
		settngs.ignore = "*";
	});
}
