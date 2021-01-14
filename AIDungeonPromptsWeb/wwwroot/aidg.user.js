// ==UserScript==
// @name        /aidg/ Quick Prompts
// @namespace   https://prompts.aidg.club
// @match       https://play.aidungeon.io/*
// @grant       none
// @version     1.5
// @author      Worble
// @description Enables users to automatically import cluib prompts into AI Dungeon
// @downloadURL https://prompts.aidg.club/aidg.user.js
// @supportURL  https://github.com/Woruburu/AIDungeonPrompts/issues
// @homepageURL https://prompts.aidg.club
// ==/UserScript==

function setReactInputValue(input, value) {
	const previousValue = input.value;
	input.value = value;

	const tracker = input._valueTracker;
	if (tracker) {
		tracker.setValue(previousValue);
	}

	// 'change' instead of 'input', see https://github.com/facebook/react/issues/11488#issuecomment-381590324
	input.dispatchEvent(new Event('change', { bubbles: true }));
}

function getLastQuerySelector(query) {
	var selection = document.querySelectorAll(query);
	return selection.length > 0 ? selection[selection.length - 1] : null;
}

function onLoadClick() {
	var value = prompt("Please enter the club ID (the last part of the URL)");
	if (!value) {
		return;
	}
	fetch(`https://prompts.aidg.club/api/${value}`).then((response) => {
		return response.json()
	}).then((json) => {
		const titleInput = getLastQuerySelector('input[placeholder="Larry Saves The Princess"]');
		const descriptionTextArea = getLastQuerySelector('textarea[placeholder="Provide a brief description to help others get an idea of what to expect when playing your scenario."]');
		const promptTextArea = getLastQuerySelector('textarea[placeholder*="Enter the starting prompt for this scenario"]');
		const memoryTextArea = getLastQuerySelector('textarea[placeholder*="Enter anything you want the AI to remember during the adventure, but don\'t want to show the player."]');
		const authorsNoteInput = getLastQuerySelector('input[placeholder*="Style hint:"]');;
		const questTextArea = getLastQuerySelector('textarea[placeholder*="Add quests to be completed by the user."]');

		setReactInputValue(titleInput, json.title === null ? "" : json.title);
		setReactInputValue(descriptionTextArea, json.description === null || json.description === "" ? " " : json.description);
		setReactInputValue(promptTextArea, json.promptContent === null ? "" : json.promptContent);
		setReactInputValue(memoryTextArea, json.memory === null ? "" : json.memory);
		setReactInputValue(authorsNoteInput, json.authorsNote === null ? "" : json.authorsNote);
		setReactInputValue(questTextArea, json.quests === null ? "" : json.quests);
	});
}

function locationIsEditPage(location) {
	var url = location.split('/').pop();
	return url.includes('scenarioEdit');
}

let timeoutVal = null;

function timeOut() {
	if (document.getElementById('aidg-import-button')) {
		console.log("Button already exists, stopping...");
		return;
	}
	timeoutVal = setTimeout(function () {
		const menubar = getLastQuerySelector('div[style="display: flex; margin-right: 16px; margin-top: 4px;"]');
		if (!menubar) {
			console.log("Menu bar does not exist, restarting...");
			timeOut();
			return;
		}
		console.log("Menu bar found, adding button.");
		const clone = menubar.lastChild.cloneNode();
		const clone2 = menubar.lastChild.lastChild.cloneNode();
		const clone3 = menubar.lastChild.lastChild.lastChild.cloneNode();
		clone3.innerText = '';
		clone3.onclick = onLoadClick;
		clone2.append(clone3);
		clone.append(clone2);
		clone.id = 'aidg-import-button';
		menubar.append(clone);
	}, 1000)
};

function handleHistoryChange(location) {
	console.log("In handle history, location is: " + location);
	if (locationIsEditPage(location) && timeoutVal === null) {
		console.log("On edit page, starting timeout");
		timeOut();
	} else if (!locationIsEditPage(location) && timeoutVal !== null) {
		console.log("Leaving page, clearing timeout and button");
		clearTimeout(timeoutVal);
		timeoutVal = null;
		const button = document.getElementById('aidg-import-button');
		if (button) {
			button.remove();
		}
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

handleHistoryChange(window.location.href);
