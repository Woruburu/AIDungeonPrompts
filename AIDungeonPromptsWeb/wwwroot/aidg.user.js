// ==UserScript==
// @name        /aidg/ quick prompt addon
// @namespace   prompts.aidg.club/aidg.user.js
// @match       https://play.aidungeon.io/*
// @grant       none
// @version     1.1
// @author      Worble
// @description 13/01/2021, 17:58:05
// ==/UserScript==

function setReactInputValue(input, value) {
	const previousValue = input.value;

	// eslint-disable-next-line no-param-reassign
	input.value = value;

	const tracker = input._valueTracker;
	if (tracker) {
		tracker.setValue(previousValue);
	}

	// 'change' instead of 'input', see https://github.com/facebook/react/issues/11488#issuecomment-381590324
	input.dispatchEvent(new Event('change', { bubbles: true }));
}

function onLoadClick() {
	var value = prompt("Please enter the club ID (the last part of the URL)");
	if (!value) {
		return;
	}
	fetch(`https://prompts.aidg.club/api/${value}`).then((response) => {
		return response.json()
	}).then((json) => {
		const allInputs = document.querySelectorAll('input');
		const allTextAreas = document.querySelectorAll('textarea');
		const titleInput = allInputs[0];
		const descriptionTextArea = allTextAreas[0];
		const promptTextArea = allTextAreas[1];
		const memoryTextArea = allTextAreas[2];
		const authorsNoteInput = allInputs[2];
		const questTextArea = allTextAreas[3];

		setReactInputValue(titleInput, json.title);
		setReactInputValue(descriptionTextArea, json.description);
		setReactInputValue(promptTextArea, json.promptContent);
		setReactInputValue(memoryTextArea, json.memory);
		setReactInputValue(authorsNoteInput, json.authorsNote);
		setReactInputValue(questTextArea, json.quests);
	});
}

function locationIsEditPage(location) {
	var url = location.split('/').pop();
	return url.includes('scenarioEdit');
}

let timeoutVal = null;
let buttonExists = false;

function timeOut() {
	if (buttonExists) {
		return;
	}
	timeoutVal = setTimeout(function () {
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
		buttonExists = true;
	}, 1000)
};

function handleHistoryChange(location) {
	if (locationIsEditPage(location) && timeoutVal === null) {
		timeOut();
	} else if (!locationIsEditPage(location) && (timeoutVal !== null || buttonExists === true)) {
		clearTimeout(timeoutVal);
		timeoutVal = null;
		buttonExists = false;
	}
}

(function (history) {
	var pushState = history.pushState;
	history.pushState = function (state) {
		handleHistoryChange(arguments[2]);
		return pushState.apply(history, arguments);
	};
	var replaceState = history.replaceState;
	history.replaceState = function (state) {
		handleHistoryChange(arguments[2]);
		return replaceState.apply(history, arguments);
	}
})(window.history);

if (locationIsEditPage(window.location.href)) {
	timeOut();
}
