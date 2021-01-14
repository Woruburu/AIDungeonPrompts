// ==UserScript==
// @name        /aidg/ Quick Prompts
// @namespace   https://prompts.aidg.club
// @match       https://play.aidungeon.io/*
// @grant       none
// @version     1.7
// @author      /aidg/
// @description Enables users to automatically import club prompts into AI Dungeon
// @downloadURL https://prompts.aidg.club/aidg.user.js
// @supportURL  https://github.com/Woruburu/AIDungeonPrompts/issues
// @homepageURL https://prompts.aidg.club
// @icon        data:image/png;base64,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
// ==/UserScript==

// Constant Variables

/**
 * Whether or not to log output. Set this to true for debugging purposes.
 * */
const enableLogs = false

/**
* The url fragment associated with the scenario edit page.
*/
const scenarioEditUrl = 'scenarioEdit';

/**
* The query selector for the menu bar.
*/
const menubarQuerySelector = 'div[style="display: flex; margin-right: 16px; margin-top: 4px;"]';

/**
* The text displayed in the window popup.
*/
const windowPromptText = 'Please enter the prompts club ID (the last part of the URL)';

/**
* The query selector for the title input field.
*/
const titleInputSelector = 'input[placeholder="Larry Saves The Princess"]';

/**
* The query selector for the description text area field.
*/
const descriptionTextAreaSelector = 'textarea[placeholder="Provide a brief description to help others get an idea of what to expect when playing your scenario."]';

/**
* The query selector for the prompt text area field.
*/
const promptTextAreaSelector = 'textarea[placeholder*="Enter the starting prompt for this scenario"]';

/**
* The query selector for the memory text area field.
*/
const memoryTextAreaSelector = 'textarea[placeholder*="Enter anything you want the AI to remember during the adventure, but don\'t want to show the player."]';

/**
* The query selector for the authors note input field.
*/
const authorsNoteInputSelector = 'input[placeholder*="Style hint:"]';

/**
* The query selector for the quests text area field.
*/
const questsTextAreaSelector = 'textarea[placeholder*="Add quests to be completed by the user."]';

/**
* The query selector for the added import button.
*/
const uploadButtonQuerySelector = 'div[data-aidg-import-button]';

/**
* The text inside the upload button.
 * I just copied this from the World Info download button, some users have reported this doesn't show correctly.
*/
const uploadButtonText = ''

/**
* The alert text when the prompt is not found.
*/
const notFoundMessage = 'Prompt not found, please ensure you entered the correct ID';

/**
* The alert text when the prompt is not found.
*/
const fetchBadStatusMessage = (statusCode) => `Unexpected status returned from server: ${statusCode}`;

/**
* The alert text when the prompt is not found.
*/
const somethingWentWrongMessage = (message) => `Something when wrong: ${statusCode}`;

// Mutable Variables

/**
* The timeout ID.
*/
let timeoutVal = null;

// Functions

/**
 * Writes a log to the console, if enableLogs is set to true.
 * @param {any} value - The value to log
 */
function consoleLog(value) {
	if (enableLogs) {
		console.log(value)
	}
}

/**
 * Sets input value in a way that causes React to update state.
 * See https://github.com/facebook/react/issues/10135#issuecomment-500929024 for more details.
 * @param {Element} input - The input element.
 * @param {string} value - The new value.
 */
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

/**
 * Like querySelector, but only returns the last element in the list.
 * This is necessary because for some reason, if you load the edit page first, navigate away, and then come back, every element is duplicated (??? what the FUCK Mormon) and we only want the latest one).
 * @param {string} selectors - The query selectors.
 * @returns {Element|null}
 */
function getLastQuerySelector(selectors) {
	if (!selectors) {
		return null;
	}
	var selection = document.querySelectorAll(selectors);
	return selection.length > 0 ? selection[selection.length - 1] : null;
}

/**
 * Returns true if the url is the scenario edit page, false otherwise.
 * @param {string} location - The current URL.
 * @returns {boolean}
 */
function locationIsEditPage(location) {
	if (!location || typeof location !== "string") {
		return false;
	}
	var url = location.split('/').pop();
	return url.includes(scenarioEditUrl);
}

/**
 * The event that is fired when the load button is clicked.
 */
function onLoadClick() {
	var value = prompt(windowPromptText);
	if (!value) {
		return;
	}
	fetch(`https://prompts.aidg.club/api/${value}`).then((response) => {
		if (response.status === 200) {
			return response.json();
		} else if (response.status === 404) {
			alert(notFoundMessage);
		} else {
			alert(fetchBadStatusMessage(response.status));
		}
	}).then((json) => {
		if (!json) {
			return;
		}
		const titleInput = getLastQuerySelector(titleInputSelector);
		const descriptionTextArea = getLastQuerySelector(descriptionTextAreaSelector);
		const promptTextArea = getLastQuerySelector(promptTextAreaSelector);
		const memoryTextArea = getLastQuerySelector(memoryTextAreaSelector);
		const authorsNoteInput = getLastQuerySelector(authorsNoteInputSelector);
		const questTextArea = getLastQuerySelector(questsTextAreaSelector);

		setReactInputValue(titleInput, json.title === null ? "" : json.title);
		// apparently leaving description blank just reverts it to a previous one (??? wtf mormon) so we put a blank space instead
		setReactInputValue(descriptionTextArea, json.description === null || json.description === "" ? " " : json.description);
		setReactInputValue(promptTextArea, json.promptContent === null ? "" : json.promptContent);
		setReactInputValue(memoryTextArea, json.memory === null ? "" : json.memory);
		setReactInputValue(authorsNoteInput, json.authorsNote === null ? "" : json.authorsNote);
		setReactInputValue(questTextArea, json.quests === null ? "" : json.quests);
	}).catch((error) => {
		alert(somethingWentWrongMessage(error.message));
	});
}

/**
 * A timeout loop that checks every second to see if the menu bar is loaded yet.
 */
function timeOut() {
	clearTimeout(timeoutVal);
	timeoutVal = setTimeout(function () {
		const menubar = getLastQuerySelector(menubarQuerySelector);
		if (!menubar) {
			consoleLog("Menu bar does not exist, restarting loop.");
			timeOut();
			return;
		}
		consoleLog("Menu bar found, adding button.");
		if (menubar.lastChild.dataset.aidgImportButton) {
			consoleLog("Button already exists, stopping.");
			return;
		}
		const clone = menubar.lastChild.cloneNode();
		const clone2 = menubar.lastChild.lastChild.cloneNode();
		const clone3 = menubar.lastChild.lastChild.lastChild.cloneNode();
		clone3.innerText = uploadButtonText;
		clone3.onclick = onLoadClick;
		clone2.append(clone3);
		clone.append(clone2);
		clone.dataset.aidgImportButton = true;
		menubar.append(clone);
	}, 1000)
};

/**
 * Either starts the menu bar timeout checker if the url is on the edit page, or cleans up if we have navigated away. If timeoutVal has value we know we need to clean up, otherwise we can skip that step.
 * @param {string} location - The current URL.
 */
function handleHistoryChange(location) {
	consoleLog("In handle history, location is: " + location);
	if (locationIsEditPage(location)) {
		consoleLog("On edit scenario page, starting timeout.");
		timeOut();
	} else if (!locationIsEditPage(location) && timeoutVal !== null) {
		consoleLog("Leaving page, clearing timeout and button.");
		clearTimeout(timeoutVal);
		timeoutVal = null;
		const buttons = document.querySelectorAll(uploadButtonQuerySelector);
		for (var i = 0; i < buttons.length; i++) {
			buttons[i].remove();
		}
	}
}

/**
 * Overrides the history pushState and replaceState so we can tell when the SPA has navigated to the edit scenario page.
 */
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

/**
 * Starts the timeout immediately if the scenario edit page was the first page loaded.
 */
handleHistoryChange(window.location.href);
