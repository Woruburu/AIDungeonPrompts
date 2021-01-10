document.addEventListener('DOMContentLoaded', () => {
	const addWiButton = document.getElementById('add-wi');
	const saveDraft = document.getElementById('save-draft');
	const addSub = document.getElementById('add-sub-scenario');
	ignoreSettingsOnClick(addWiButton);
	ignoreSettingsOnClick(saveDraft);
	//ignoreSettingsOnClick(addSub);
});

function ignoreSettingsOnClick(element) {
	if (element) {
		element.addEventListener('click', function () {
			const settngs = $.data($('form')[0], 'validator').settings;
			settngs.ignore = "*";
		});
	}
}
