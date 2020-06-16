
window.stopClick = (message) => {
    $('input[type="file"]').click(function (event) {
        event.preventDefault();
	});
}

window.InitDragAndDrop = (message) => {
	if(
		!document.querySelectorAll
		||
		!('draggable' in document.createElement('span'))
		||
		window.opera
	) { return; }

	//get the collection of draggable items and add their draggable attributes
	for (var
		items = document.querySelectorAll('[data-draggable="item"]'),
		len = items.length,
		i = 0; i < len; i++) {
		items[i].setAttribute('draggable', 'true');
		items[i].setAttribute('aria-grabbed', 'false');
		items[i].setAttribute('tabindex', '0');
	}
}

window.addMouseDown = (message) => {
    document.addEventListener('mousedown', function (e) {
		console.log("MOUSE DOWN EVENT");
		if (e.target.getAttribute('draggable')) {
			//if the multiple selection modifier is not pressed 
			//and the item's grabbed state is currently false
			if (!hasModifier(e) && e.target.getAttribute('aria-grabbed') == 'false')
			{
				//clear all existing selections
				clearSelections();
				//then add this new selection
				addSelection(e.target);
				firstShift = e.target.getAttribute('title');
			}

			if (hasShitfKey(e) == true) {
				secondShift = e.target.getAttribute('title');

				console.log("First SHIFT KEY:" + firstShift);
				console.log("Second SHIFT KEY:" + secondShift);
				if ((firstShift != secondShift) &&  firstShift > 0 && secondShift > 0) {
					clearSelections();
					ShiftSelection(firstShift, secondShift);
                }
			}
		}
		//else [if the element is anything else]
		//and the selection modifier is not pressed 
		else if (!hasModifier(e)) {
			//clear all existing selections
			clearSelections();
			firstShift = 0;
			secondShift = 0;
		}

    }, false);
}

function ShiftSelection(firstShift, secondShift) {

	var tempFirst = firstShift;
	var tempSecond = secondShift;

	tempFirst--;
	tempSecond--;
	if (tempFirst > tempSecond) {
		var temp = tempFirst;
		tempFirst = tempSecond;
		tempSecond = temp;
    }

	for (var
		items = document.querySelectorAll('[data-draggable="item"]'),
		len = items.length,
		i = 0; i < len; i++) {

		if (i >= tempFirst && i <= tempSecond) {
			addSelection(items[i]);
        }
		//items[i].setAttribute('draggable', 'true');
		//items[i].setAttribute('aria-grabbed', 'false');
		//items[i].setAttribute('tabindex', '0');
	}
}


window.addMouseUp = (message) => {
    document.addEventListener('mouseup', function (e) {
		console.log("MOUSE UP EVENT");

		if (e.target.getAttribute('draggable') && hasModifier(e))
		{
			//if the item's grabbed state is currently true
			if (e.target.getAttribute('aria-grabbed') == 'true') {
				//unselect this item
				if (hasShitfKey(e) != true && hasCtrlKey(e) != true ) {
					removeSelection(e.target);
					//if that was the only selected item 
					//then reset the owner container reference			
					if (!selections.items.length) {
						selections.owner = null;
					}
				}
			}
			//else [if the item's grabbed state is false]
			else {
				//add this additional selection
				addSelection(e.target);
			}
		}
    }, false);
}

var firstShift = 0;
var secondShift = 0;
var selections =
{
	items: [],
	owner: null
};

//function for selecting an item
function addSelection(item) {
	//if the owner reference is still null, set it to this item's parent
	//so that further selection is only allowed within the same container
	if (!selections.owner) {
		selections.owner = item.parentNode;
	}

	//or if that's already happened then compare it with this item's parent
	//and if they're not the same container, return to prevent selection
	else if (selections.owner != item.parentNode) {
		return;
	}

	//set this item's grabbed state
	item.setAttribute('aria-grabbed', 'true');
	console.log("ADD SELECTION : " + item.getAttribute('value'));
	DotNet.invokeMethodAsync("OpenNetLinkApp", "AddPath", item.getAttribute('value'));

	//add it to the items array
	selections.items.push(item);
}

//function for unselecting an item
function removeSelection(item) {
	//reset this item's grabbed state
	item.setAttribute('aria-grabbed', 'false');
	console.log("REMOVE SELECTION : " + item.getAttribute('value'));
	DotNet.invokeMethodAsync("OpenNetLinkApp", "RemovePath", item.getAttribute('value'));
	//then find and remove this item from the existing items array
	for (var len = selections.items.length, i = 0; i < len; i++) {
		if (selections.items[i] == item) {
			selections.items.splice(i, 1);
			break;
		}
	}
}

//function for resetting all selections
function clearSelections() {
	//if we have any selected items
	if (selections.items.length) {
		//reset the owner reference
		selections.owner = null;

		//reset the grabbed state on every selected item
		for (var len = selections.items.length, i = 0; i < len; i++) {
			selections.items[i].setAttribute('aria-grabbed', 'false');
		}
		console.log("CLEAR SELECTION : ALL");
		DotNet.invokeMethodAsync("OpenNetLinkApp", "ClearPath");
		//then reset the items array		
		selections.items = [];
	}
}

function hasModifier(e) {
	return (e.ctrlKey || e.metaKey || e.shiftKey);
}

function hasShitfKey(e) {
	return e.shiftKey;
}

function hasCtrlKey(e) {
	return e.ctrlKey;
}

window.addDragStart = (message) => {
	document.addEventListener('dragstart', function (e) {
		console.log("DRAG START EVENT");
		//if the element's parent is not the owner, then block this event
		if (selections.owner != e.target.parentNode) {
			e.preventDefault();
			return;
		}

		//[else] if the multiple selection modifier is pressed 
		//and the item's grabbed state is currently false
		if
			(
			hasModifier(e)
			&&
			e.target.getAttribute('aria-grabbed') == 'false'
		) {
			//add this additional selection
			addSelection(e.target);
		}

		//we don't need the transfer data, but we have to define something
		//otherwise the drop action won't work at all in firefox
		//most browsers support the proper mime-type syntax, eg. "text/plain"
		//but we have to use this incorrect syntax for the benefit of IE10+
		e.dataTransfer.setData('text', '');

	}, false);
}