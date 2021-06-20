document.addEventListener('DOMContentLoaded', () => {
	$('#delete-form').on('submit', (event) => {
		event.preventDefault();
		$('#confirm-delete-modal').modal('show');
	})

	const copyNaiJsonButton = $('#get-nai-json');
	copyNaiJsonButton.on('click', async (_event) => {
		try {
			const promptId = copyNaiJsonButton.data("id");
			if (promptId) {
				const response = await fetch(`/${promptId}/nai-scenario`);
				const json = await response.text();
				await navigator.clipboard.writeText(json);
				const currentText = copyNaiJsonButton.text();
				copyNaiJsonButton.text("Copied!");
				setTimeout(() => {
					copyNaiJsonButton.text(currentText);
				}, 1000)
			}
		} catch(err){
			alert(`There was an error copying to clipboard: ${err}`);
		}
	})
});
