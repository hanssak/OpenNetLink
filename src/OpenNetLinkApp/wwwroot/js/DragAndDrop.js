
window.changeUrlRedirectionFlag = (mode) => {
	if (mode == "on") {
		$('#iUrlFlag').addClass("url_active");
	}
	else {
		$('#iUrlFlag').removeClass("url_active");
    }
}

window.changeModalBg = (nOpacity) => {
	$('.modal-backdrop.show').css('opacity', nOpacity);
}

window.changeModalZIndex = (nIdx) => {
	$('.modal-backdrop').css('z-index', nIdx);
}

window.changeModalFontColor = (color) => {
	$('.control-sidebar-dark').css('color', color);
}

window.getElementValue = (id) => {
	return $("#" + id).val();
}

window.openWindow = (url, name, width, height) => {
	var x = 0, y = 0;
	var dlgWidth = width;
	var dlgHeight = height;
	x = (window.screen.availWidth - dlgWidth) / 2;
	y = (window.screen.availHeight - dlgHeight) / 2;
	var win = window.open(url, name, "left=" + x + ",top=" + y + ",width=" + dlgWidth + ",height=" + dlgHeight + ",toolbar=0,menubar=0,resizable=No,status=1");
	win.focus();
}

window.alertMessage = (msg) => {
	alert(msg);
}

window.initLogIn = () => {
	$("#main-nav").css("display", "none");
	$("#left-sidebar").css("display", "none");
	$("#main-body").css("margin-left", "0");
	$("#main-body").css("margin-top", "0");
	$("#main-body").css("height", "500px");
	$("#main-footer").css("display", "none");
	
}

window.exitLogIn = () => {
	$("#main-nav").css("display", "");
	$("#left-sidebar").css("display", "");
	$("#main-body").css("margin-left", "250px");
	$("#main-body").css("margin-top", "calc(3rem)");
	$("#main-footer").css("display", "");
	//$("#main-body").css("height", "630px");

	var dirRightHeight = $("#divRightContent").css("height");
	var divRightUpper = $("#divRightUpperSide").css("height");
	var divRightBottom = $("#divRightBottomSide").css("height");
	var divRest = parseInt(divRightUpper.replace("px", "")) + parseInt(divRightBottom.replace("px", ""));
	$("#divDropFile").css("height", (parseInt(dirRightHeight.replace("px", "")) - (divRest + 7)) + "px");
}

window.closeProgressMessage = (id) => {
	$("#" + id).parent().parent().find("[type='button']").trigger("click");
}

window.updateProgressMessage = (id, message, progress) => {
	$("#" + id).html(message);
	$("#progress" + id).css("width", progress);
}

window.fireProgressMessage = (id, title, message) => {
	$(document).Toasts('create', {
		body: "<div id='" + id + "'>" + message + "</div><div class='progress progress-xs mb-2 mt-2 ' style='border-radius: 3px; width:320px;'><div id='progress" + id + "' class='progress-bar progress-bar-danger' style='width: 1%;  border-radius: 3px'></div></div>",
		title: title,
		icon: 'fas fa-file-export blue-txt mr-2 ',
		style: 'width:350px !important;',
	})
}

window.fireToastMessage = (type, title, message) => {
	var cls = "bg-success";
	if (type == "success")
		cls = "bg-success";
	else if (type == "info")
		cls = "bg-info";
	else if (type == "waring")
		cls = "bg-warning";
	else if (type == "error")
		cls = "bg-danger";

	var floattime = 3000;
	if (type == "waring" || type == "error")
		floattime = 7000;
	$(document).Toasts('create', {
		class: cls, 
		type: type,
		title: title,
		autohide: true,
		delay: floattime,
		body: message,
		icon: 'fas fa-envelope fa-lg',
	})
}

window.openPopUp = (popUpId) => {
	$("#" + popUpId).modal("show");
	$("#" + popUpId).focus();
}

window.closePopUp = (popUpId) => {
	$("#" + popUpId).modal("hide");
}

window.initApproveUI = () => {
	
	$("#datepicker").datepicker({
		autoclose: true,
		dateFormat: 'yy-mm-dd'
	})
	$("#datepicker2").datepicker({
		autoclose: true,
		dateFormat: "yy-mm-dd"
	})
	$("#datepicker3").datepicker({
		autoclose: true,
		dateFormat: "yy-mm-dd"
	})
	$("#datepicker4").datepicker({
		autoclose: true,
		dateFormat: "yy-mm-dd"
	})
}

window.stopClick = (message) => {
    /*$('input[type="file"]').click(function (event) {
        event.preventDefault();
	});*/
}

window.startClick = () => {
	$('input[type="file"]').trigger("click");
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

window.mouseDownIntervalCheck = (minuteTime) => {
	if (MouseTime == 0)
		return "true";
	console.log(Math.floor(+ new Date() / 1000) - MouseTime);
	console.log("INPUT TIME:" + minuteTime * 60);

	if (Math.floor(+ new Date() / 1000) - MouseTime > minuteTime * 60)
		return "false";
	else
		return "true";
}

window.adJustWindowsize = () => {

	$(window).resize(function () {
		var dirRightHeight = $("#divRightContent").css("height");
		var divRightUpper = $("#divRightUpperSide").css("height");
		var divRightBottom = $("#divRightBottomSide").css("height");
		var divRest = parseInt(divRightUpper.replace("px", "")) + parseInt(divRightBottom.replace("px", ""));
		$("#divDropFile").css("height", (parseInt(dirRightHeight.replace("px", "")) - (divRest+7)) + "px");
	});
}


var MouseTime = 0;
window.addMouseDown = (message) => {
    document.addEventListener('mousedown', function (e) {

		if (MouseTime == Math.floor(+ new Date() / 1000))
			return;
		MouseTime = Math.floor(+ new Date() / 1000);

		//console.log("MOUSE DOWN EVENT " + e.target.getAttribute('name') + " MouseTime:" + MouseTime);
		
		//결재자추가 팝업 GROUP STEP형 DIV 선택
		if (e.target.parentElement.getAttribute('name') != null) {
			if (e.target.parentElement.getAttribute('name').indexOf("TargetGropDiv") > -1) {
				clearDivSelections();
				addDivSelection(e.target.parentElement);
				clearTrTargetSelections(true);
				return;
			}
		}
		//결재자선택팝업 첫번째
		if (e.target.parentElement.getAttribute('name') == "trItem") {

			clearTrSelections();
			addTrSelection(e.target.parentElement, 1);
			return;
		}
		//결재자 선택팝업 두번째
		if (e.target.parentElement.getAttribute('name') == "trItem2") {

			clearTrSelections();
			addTrSelection(e.target.parentElement, 2);
			return;
		}
		//결재자 선택팝업 세번째
		if (e.target.parentElement.getAttribute('name') == "trItem3") {

			clearTrSelections();
			addTrSelection(e.target.parentElement, 3);
			return;
		}
		//대결자 검색 팝업
		if (e.target.parentElement.getAttribute('name') == "trItem4") {

			clearTrSelections();
			addTrSelection(e.target.parentElement, 4);
			return;
		}
		//결재자 지정 첫번째
		if (e.target.parentElement.getAttribute('name') == "trSelect") {

			clearTrTargetSelections(true);
			addTrTargetSelection(e.target.parentElement, 1);
			return;
		}
		//결재자 지정 두번째
		if (e.target.parentElement.getAttribute('name') == "trSelect2") {

			clearTrTargetSelections(true);
			addTrTargetSelection(e.target.parentElement, 2);
			return;
		}
		//결재자 지정 세번째
		if (e.target.parentElement.getAttribute('name') == "trSelect3") {
			//DIV선택 재조정
			clearDivSelections();
			addDivSelection(e.target.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement);
			//TR선택
			clearTrTargetSelections(true);
			addTrTargetSelection(e.target.parentElement, 3);
			return;
		}
		//대결자 지정
		if (e.target.parentElement.getAttribute('name') == "trSelect4") {

			clearTrTargetSelections(true);
			addTrTargetSelection(e.target.parentElement, 4);
			return;
		}


		if (e.target.getAttribute('draggable')) {
			//if the multiple selection modifier is not pressed 
			//and the item's grabbed state is currently false
			if (!hasModifier(e) && e.target.getAttribute('aria-grabbed') == 'false')
			{
				//clear all existing selections
				clearSelections();
				//then add this new selection
				addSelection(e.target);
				firstShift = e.target.getAttribute('label');
				//console.log("First SHIFT KEY:" + firstShift);
			}

			if (hasShitfKey(e) == true && e.target.getAttribute('aria-grabbed') == 'false') {
				secondShift = e.target.getAttribute('label');
				//console.log("Second SHIFT KEY:" + secondShift);
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
		//console.log("MOUSE UP EVENT");

		if (e.target.getAttribute('draggable') && hasModifier(e))
		{
			//if the item's grabbed state is currently true
			//console.log("MOUSE UP EVENT");
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
				//console.log("MOUSE UP EVENT");
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
var TrSelections =
{
	items: []
};
var TrTargetSelections =
{
	items: []
};
var DivSelections =
{
	items: []
};


function addDivSelection(item) {
	item.setAttribute('aria-grabbed', 'true');
	DotNet.invokeMethodAsync("OpenNetLinkApp", "ApproverDivSelect", item.getAttribute('value'));
	DivSelections.items.push(item);
}

function clearDivSelections(remove) {
	//if we have any selected items
	if (DivSelections.items.length) {
		//reset the grabbed state on every selected item
		for (var len = DivSelections.items.length, i = 0; i < len; i++) {
			DivSelections.items[i].setAttribute('aria-grabbed', 'false');
		}
		if (remove == true)
			DivSelections.items = [];
	}
}



window.adjustTargetSelection = () => {
	clearTrTargetSelections(false);
}

function addTrTargetSelection(item, index) {
	item.setAttribute('aria-grabbed', 'true');
	if (index == 1)
		DotNet.invokeMethodAsync("OpenNetLinkApp", "ApproverTargetSelect", item.getAttribute('value'));
	else if (index == 2)
		DotNet.invokeMethodAsync("OpenNetLinkApp", "ApproverTargetSelect2", item.getAttribute('value'));
	else if (index == 3)
		DotNet.invokeMethodAsync("OpenNetLinkApp", "ApproverTargetSelect3", item.getAttribute('value'));
	else if (index == 4)
		DotNet.invokeMethodAsync("OpenNetLinkApp", "ProxyTargetSelect", item.getAttribute('value'));

	TrTargetSelections.items.push(item);
}

function clearTrTargetSelections(remove) {
	//if we have any selected items
	
	if (TrTargetSelections.items.length) {
		//reset the owner reference
		
		//reset the grabbed state on every selected item
		for (var len = TrTargetSelections.items.length, i = 0; i < len; i++) {
			console.log("COUNT:" + TrTargetSelections.items[i].getAttribute("value"));
			TrTargetSelections.items[i].setAttribute('aria-grabbed', 'false');
		}
		if( remove == true )
			TrTargetSelections.items = [];
	}
}

function addTrSelection(item, index) {
	item.setAttribute('aria-grabbed', 'true');
	if (index == 1)
		DotNet.invokeMethodAsync("OpenNetLinkApp", "ApproverSearchSelect", item.getAttribute('value'));
	else if (index == 2)
		DotNet.invokeMethodAsync("OpenNetLinkApp", "ApproverSearchSelect2", item.getAttribute('value'));
	else if (index == 3) 
		DotNet.invokeMethodAsync("OpenNetLinkApp", "ApproverSearchSelect3", item.getAttribute('value'));
	else if (index == 4) 
		DotNet.invokeMethodAsync("OpenNetLinkApp", "ProxySearchSelect", item.getAttribute('value'));
	
		
	TrSelections.items.push(item);
}
function clearTrSelections() {
	//if we have any selected items
	if (TrSelections.items.length) {
		
		//reset the grabbed state on every selected item
		for (var len = TrSelections.items.length, i = 0; i < len; i++) {
			TrSelections.items[i].setAttribute('aria-grabbed', 'false');
		}
		console.log("CLEAR SELECTION : ALL");
		//DotNet.invokeMethodAsync("OpenNetLinkApp", "ClearPath");
		//then reset the items array		
		TrSelections.items = [];
	}
}
//function for selecting an item
function addSelection(item) {
	
	//set this item's grabbed state
	item.setAttribute('aria-grabbed', 'true');
	console.log("ADD SELECTION : " + item.getAttribute('value') + "  TYPE:" + item.getAttribute("label") );
	DotNet.invokeMethodAsync("OpenNetLinkApp", "AddPath", item.getAttribute('value'), item.getAttribute("label"));

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
		//if (selections.owner != e.target.parentNode) {
		//	e.preventDefault();
		//	return;
		//}

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