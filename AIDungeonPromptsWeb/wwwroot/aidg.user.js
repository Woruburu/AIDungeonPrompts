// ==UserScript==
// @name        /aidg/ quick prompt addon
// @namespace   Violentmonkey Scripts
// @match       https://play.aidungeon.io/main/scenarioEdit
// @grant       none
// @version     1.0
// @author      Worble
// @description 13/01/2021, 17:58:05
// ==/UserScript==
function onLoadClick() {
	var value = prompt("Please enter the club ID (the last part of the URL)");
	if (!value) {
		return;
	}
	fetch(`https://localhost:5001/api/${value}`).then((response) => {
		return response.json()
	}).then((json) => {
		const allInputs = document.querySelectorAll('input');
		const allTextAreas = document.querySelectorAll('textarea');
		const titleInput = allInputs[0];
		const descriptionTextArea = allTextAreas[0];
		const promptTextArea = allTextAreas[1];
		const memoryTextArea = allTextAreas[2];
		const authorsNoteInput = allInputs[1];
		const questTextArea = allTextAreas[3];
		titleInput.value = json.title;
		descriptionTextArea.value = json.description;
		promptTextArea.value = json.promptContent;
		memoryTextArea.value = json.memory;
		authorsNoteInput.value = json.authorsNote;
		questTextArea.value = json.quests;
	});
}

function timeOut() {
	setTimeout(function () {
		console.log("1");
		const menubar = document.querySelector('div[style="display: flex; margin-right: 16px; margin-top: 4px;"]');
		if (!menubar) {
			timeOut();
			return;
		}
		const clone = menubar.lastChild.cloneNode();
		const clone2 = menubar.lastChild.lastChild.cloneNode();
		const clone3 = menubar.lastChild.lastChild.lastChild.cloneNode();
		clone3.innerText = '';
		clone3.onclick = onLoadClick;
		clone2.append(clone3);
		clone.append(clone2);
		menubar.append(clone);
	}, 1000)
};

timeOut();
