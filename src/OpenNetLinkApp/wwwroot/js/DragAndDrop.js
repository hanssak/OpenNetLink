
window.loginCursorChangeDefault = () => {
    $("#divLoginMain").css("cursor", "default");
    $("#loginId").css("cursor", "default");
    $("#loginPw").css("cursor", "default");
    $("#btnLogin").css("cursor", "default");
    $("#btnLoginCancel").css("cursor", "default");
    $("#btnLoginViewBack").css("cursor", "default");

    //로그인 중, PW에 Enter 이벤트 방지 (btnLogin 버튼은 razor에서 따로 관리하므로 설정하지 않음)
    $("#loginPw").attr("disabled", false);
}

window.loginCursorChange = () => {
    $("#divLoginMain").css("cursor", "wait");
    $("#loginId").css("cursor", "wait");
    $("#loginPw").css("cursor", "wait");
    $("#btnLogin").css("cursor", "wait");
    $("#btnLoginCancel").css("cursor", "wait");
    $("#btnLoginViewBack").css("cursor", "wait");

    //로그인 중, PW에 Enter 이벤트 방지 (btnLogin 버튼은 razor에서 따로 관리하므로 설정하지 않음)
    $("#loginPw").attr("disabled", true);
    
}

window.initCapaChart = (nUse, nRest) => {

    var capaChartData = {
        labels: ['사용량', '남은용량'],
        datasets: [
            {
                label: '사용량',
                backgroundColor: ['#13bef5', '#dfdfdf', '#f39c12', '#00c0ef', '#3c8dbc', '#d2d6de'],
                data: [nUse, nRest]
            }
        ]
    }

    var ele = $('#myCapacityChart').get(0);
    if (ele == null) return;
    var barChartCanvas = ele.getContext('2d');
    if (barChartCanvas == null) return;
    var barChartData = jQuery.extend(true, {}, capaChartData)

    var barChartOptions = {
        legend: { display: false },
        responsive: false,
        maintainAspectRatio: false,
        datasetFill: false,
        animation: {
            duration: 1,
            onComplete: function () {
                var chartInstance = this.chart,
                    ctx = chartInstance.ctx;
                ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontSize, Chart.defaults.global.defaultFontStyle, Chart.defaults.global.defaultFontFamily);
                ctx.textAlign = 'center';
                ctx.textBaseline = 'bottom';

                this.data.datasets.forEach(function (dataset, i) {
                    var meta = chartInstance.controller.getDatasetMeta(i);
                    meta.data.forEach(function (bar, index) {
                        var data = parseInt((dataset.data[0] * 100) / (dataset.data[0] + dataset.data[1]));
                        if (index == 0)
                            ctx.fillText(data + "% 사용중", bar._model.x, bar._model.y + 5);
                    });
                });
            }
        }
    }

    var barChart = new Chart(barChartCanvas, {
        type: 'doughnut',
        data: barChartData,
        options: barChartOptions
    })

}

window.initClipBoardCountChart = (nMax, nUse) => {

    var ele = document.getElementById("myClicpboardCountChart");
    if (ele == null) return;
    var ctx = ele.getContext('2d');
    if (ctx == null) return;

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['건수(회)'],
            datasets: [
                {
                    barThickness: 12,
                    label: '사용건',
                    data: [nUse],
                    backgroundColor: '#e6002a'
                }, {
                    barThickness: 12,
                    label: '허용건',
                    data: [nMax],
                    backgroundColor: '#0056d8'
                }]
        },
        options: {
            responsive: false,
            legend: {
                display: false // place legend on the right side of chart
            },
            scales: {
                xAxes: [{
                    stacked: true // this should be set to make the bars stacked
                }],
                yAxes: [{
                    stacked: true // this also..
                }]
            }
        }
    });
}

window.initClipBoardChart = (nMax, nUse) => {

    //var ctx = document.getElementById("myClicpboardChart").getContext('2d');
    var ele = document.getElementById("myClicpboardChart");
    if (ele == null) return;
    var ctx = ele.getContext('2d');
    if (ctx == null) return;

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['용량(MB)'],
            datasets: [
                {
                    barThickness: 12,
                    label: '사용량',
                    data: [nUse],
                    backgroundColor: '#e6002a'
                }, {
                    barThickness: 12,
                    label: '허용량',
                    data: [nMax],
                    backgroundColor: '#0056d8'
                }]
        },
        options: {
            responsive: false,
            legend: {
                display: false // place legend on the right side of chart
            },
            scales: {
                xAxes: [{
                    stacked: true // this should be set to make the bars stacked
                }],
                yAxes: [{
                    stacked: true // this also..
                }]
            }
        }
    });


    /*var clipChartData = {
        labels: ['용량(MB)', '건수'],
        datasets: [
            {
                label: '전송량',
                backgroundColor: 'rgba(160,141,188,0.9)',
                borderColor: 'rgba(160,141,188,0.8)',
                pointRadius: false,
                pointColor: '#3b8bba',
                pointStrokeColor: 'rgba(160,141,188,1)',
                pointHighlightFill: '#fff',
                pointHighlightStroke: 'rgba(160,141,188,1)',
                data: [inCnt, outCnt]
            }
        ]
    }

    var barChartCanvas = $('#myClicpboardChart').get(0).getContext('2d')
    var barChartData = jQuery.extend(true, {}, clipChartData)

    var barChartOptions = {
        scales: {
            yAxes: [{
                ticks: {
                    //beginAtZero: true
                    display: false
                }
            }]
        },
        legend: { display: false },
        responsive: false,
        maintainAspectRatio: false,
        datasetFill: false,
        animation: {
            duration: 1,
            onComplete: function () {
                var chartInstance = this.chart,
                    ctx = chartInstance.ctx;
                ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontSize, Chart.defaults.global.defaultFontStyle, Chart.defaults.global.defaultFontFamily);
                ctx.textAlign = 'center';
                ctx.textBaseline = 'bottom';

                this.data.datasets.forEach(function (dataset, i) {
                    var meta = chartInstance.controller.getDatasetMeta(i);
                    meta.data.forEach(function (bar, index) {
                        var data = dataset.data[index];
                        ctx.fillText(data, bar._model.x, bar._model.y + 14);
                    });
                });
            }
        }
    }

    var barChart = new Chart(barChartCanvas, {
        type: 'bar',
        data: barChartData,
        options: barChartOptions
    })*/
}

window.initTransferCountChart = (nMax, nUse) => {

    //var ctx = document.getElementById("myTransferCountChart").getContext('2d');
    var ele = document.getElementById("myTransferCountChart");
    if (ele == null) return;
    var ctx = ele.getContext('2d');
    if (ctx == null) return;

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['건수(회)'],
            datasets: [
                {
                    barThickness: 12,
                    label: '사용건',
                    data: [nUse],
                    backgroundColor: '#e6002a'
                }, {
                    barThickness: 12,
                    label: '허용건',
                    data: [nMax],
                    backgroundColor: '#0056d8'
                }]
        },
        options: {
            responsive: false,
            legend: {
                display: false // place legend on the right side of chart
            },
            scales: {
                xAxes: [{
                    stacked: true // this should be set to make the bars stacked
                }],
                yAxes: [{
                    stacked: true // this also..
                }]
            }
        }
    });
}

window.initTransferChart = (nMax, nUse) => {

    //var ctx = document.getElementById("myTransferChart").getContext('2d');
    var ele = document.getElementById("myTransferChart")
    if (ele == null) return;
    var ctx = ele.getContext('2d');
    if (ctx == null) return;

    var chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['용량(MB)'],
            datasets: [
                {
                    barThickness: 12,
                    label: '사용량',
                    data: [nUse],
                    backgroundColor: '#e6002a'
                }, {
                    barThickness: 12,
                    label: '허용량',
                    data: [nMax],
                    backgroundColor: '#0056d8'
                }]
        },
        options: {
            responsive: false,
            legend: {
                display: false // place legend on the right side of chart
            },
            scales: {
                xAxes: [{
                    stacked: true // this should be set to make the bars stacked
                }],
                yAxes: [{
                    stacked: true // this also..
                }]
            }
        }
    });


    /*var transChartData = {
        labels: ['용량(MB)', '건수'],
        datasets: [
            {
                label: '전송량',
                backgroundColor: 'rgba(60,141,188,0.9)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                pointColor: '#3b8bba',
                pointStrokeColor: 'rgba(60,141,188,1)',
                pointHighlightFill: '#fff',
                pointHighlightStroke: 'rgba(60,141,188,1)',
                data: [inCnt, outCnt]
            }
        ]
    }
    var transChartData = {
        labels: ['용량(MB)'],
        datasets: [
            {
                type: 'bar',
                label: ["cost1", "cost2", "cost3", "cost4"],
                data: [3, 2, 6, 3],
                stack: "standing costs",
                backgroundColor: ['#f56954', '#00a65a', '#f39c12', '#00c0ef', '#3c8dbc', '#d2d6de']
            }
        ]
    }

    var barChartCanvas = $('#myTransferChart').get(0).getContext('2d')
    var barChartData = jQuery.extend(true, {}, transChartData)

    var barChartOptions = {
        scales: {
            xAxes: [{
                stacked: true
            }],
            yAxes: [{
                ticks: {
                    //beginAtZero: true
                    display: false
                },
                stacked: true
            }]
        },
        legend: { display: false },
        responsive: false,
        maintainAspectRatio: false,
        datasetFill: false,
        animation: {
            duration: 1,
            onComplete: function () {
                var chartInstance = this.chart,
                    ctx = chartInstance.ctx;
                ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontSize, Chart.defaults.global.defaultFontStyle, Chart.defaults.global.defaultFontFamily);
                ctx.textAlign = 'center';
                ctx.textBaseline = 'bottom';

                this.data.datasets.forEach(function (dataset, i) {
                    var meta = chartInstance.controller.getDatasetMeta(i);
                    meta.data.forEach(function (bar, index) {
                        var data = dataset.data[index];
                        ctx.fillText(data, bar._model.x, bar._model.y + 11);
                    });
                });
            }
        }
    }

    var barChart = new Chart(barChartCanvas, {
        type: 'bar',
        data: barChartData,
        options: barChartOptions
    })*/
}

var nTransferUIIndex = 1;  //Transfer 화면을 두개운용하는데 첫번째는 1, 두번째는 2
var nTargetInput = 0;
var nFIndex = 1;

window.updateFirstTransferUIIndex = () => {
    nTransferUIIndex = 1;
}

window.updateSecondTransferUIIndex = () => {
    nTransferUIIndex = 2;
}

window.initTargetInputNumber = () => {
    nTargetInput = 0;
}

window.getTargetInputNumber = () => {
    nTargetInput++;
    if (nTargetInput > 10)
        nTargetInput = 1;
    return nTargetInput;
}

window.getTargetInputNumberNoIncrease = () => {
    return nTargetInput;
}

window.showElement = (id) => {
    $("#" + id).css("display", "block");
}

window.hideElement = (id) => {
    $("#" + id).attr("display", "none");
}

window.ondropInput = (id) => {
    elem = document.getElementById(id);
    var fileList = Array.prototype.map.call(elem.files, function (file) {
        var result = {
            id: nFIndex++,
            lastModified: new Date(file.lastModified).toISOString(),
            name: file.name,
            size: file.size,
            type: file.type,
            relativePath: file.fileName
        };
        Object.defineProperty(result, 'blob', { value: file });
        return result;
    });

    //if (nTransferUIIndex == 1)
    //	DotNet.invokeMethodAsync("OpenNetLinkApp", "InsertDrop");
    //else
    //	DotNet.invokeMethodAsync("OpenNetLinkApp", "NotifyChange_New", fileList);
}

window.removeAllFileList = (id) => {
    $("#" + id).val("");
    document.getElementById(id).value = null;
    return 0;
}

window.reprotHandFileList = () => {
    elem = document.getElementById("fileInput");
    elem._blazorFilesById = {};
    var fileList = Array.prototype.map.call(elem.files, function (file) {
        var result = {
            id: nFIndex++,
            lastModified: new Date(file.lastModified).toISOString(),
            name: file.name,
            size: file.size,
            type: file.type,
            relativePath: file.fileName
        };
        elem._blazorFilesById[result.id] = result;
        // Attach the blob data itself as a non-enumerable property so it doesn't appear in the JSON
        Object.defineProperty(result, 'blob', { value: file });
        return result;
    });
    //alert("NotifyChange2 is called..!" + fileList.length);

    if (nTransferUIIndex == 1)
        DotNet.invokeMethodAsync("OpenNetLinkApp", "NotifyChange2", fileList);
    else
        DotNet.invokeMethodAsync("OpenNetLinkApp", "NotifyChange2_New", fileList);
}

window.refreshListPopUp = (path) => {
    DotNet.invokeMethodAsync("OpenNetLinkApp", "JSLoadListFilesPopUp", path);
}

window.refreshListPopUpForSingleSelect = (path) => {
    DotNet.invokeMethodAsync("OpenNetLinkApp", "JSLoadListFilesPopUpForSingleSelect", path);
}

window.addFileToDropZone = (path) => {
    if (nTransferUIIndex == 1) //FileTransferUI2 화면
        DotNet.invokeMethodAsync("OpenNetLinkApp", "JSaddFileToDropZone", path);
    else                        //FileTransferUI 화면
        DotNet.invokeMethodAsync("OpenNetLinkApp", "JSaddFileToDropZone2", path);
}

window.addFileToDropZoneForSingleSelect = (path) => {
    DotNet.invokeMethodAsync("OpenNetLinkApp", "JSaddFileToDropZoneForSingleSelect", path);
}

window.refreshList = (path) => {
    if (nTransferUIIndex == 1)
        DotNet.invokeMethodAsync("OpenNetLinkApp", "JSLoadListFiles", path);
    else
        DotNet.invokeMethodAsync("OpenNetLinkApp", "JSLoadListFiles2", path);
}

window.appendHtml = (id, val) => {
    $('#' + id).html(val);
}
window.appendTextArea = (val) => {
    $('#logTracer').append(val + "<br>");
    $("#logTracer").scrollTop($("#logTracer")[0].scrollHeight);
}

window.changeFocus = (id) => {

    var element = document.getElementById(id);
    element.focus();
}

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

window.changeLeftSideBarZIndex = (nIdx) => {
    $('#left-sidebar').css('z-index', nIdx);
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

window.setElementValue = (id, value) => {
    $("#" + id).val(value);
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

window.beforeLogOut = () => {
    console.log($(".control-sidebar").css("display"));
    if ($(".control-sidebar").css("display") == "block") {
        $("#toggleRightSideBar").trigger("click");
    }
}

window.OpenControlSide = () => {
    console.log($(".control-sidebar").css("display"));
    if ($(".control-sidebar").css("display") != "block" &&
        $(".control-sidebar").css("display") != "inline" &&
        $(".control-sidebar").css("display") != "inline-block") {
        $("#toggleRightSideBar").trigger("click");
    }
}

window.CloseControlSide = () => {
    console.log($(".control-sidebar").css("display"));
    if ($(".control-sidebar").css("display") == "block") {
        $("#toggleRightSideBar").trigger("click");
    }
}

window.initLogIn = () => {
    $("#main-nav").css("display", "none");
    $("#left-sidebar").css("display", "none");
    $("#main-body").css("margin-left", "0");
    $("#main-body").css("margin-top", "0");
    $("#main-body").css("height", "500px");
    $("#main-footer").css("display", "none");
}

/*window.initWelcome = () => {
    $("#main-nav").css("display", "");
    $("#left-sidebar").css("display", "");
    $("#main-body").css("margin-left", "0");
    $("#main-body").css("margin-top", "calc(3rem)");
    $("#main-footer").css("display", "");
}*/

window.initPageLeft = () => {
    $("#main-body").css("margin-left", "250px");
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
    if (divRightUpper != null && divRightBottom != null) {
        var divRest = parseInt(divRightUpper.replace("px", "")) + parseInt(divRightBottom.replace("px", ""));
        $("#divDropFile").css("height", (parseInt(dirRightHeight.replace("px", "")) - (divRest + 7)) + "px");
    }
}

window.closeProgressMessageOnScreenLock = (id) => {
    $("#DownloadProgress").modal("hide");
    $("#downProgressRate").css("width", "1%");
}

window.closeProgressMessage = (id) => {
    /*$("#" + id).parent().parent().find("[type='button']").trigger("click");*/
    $("#left-sidebar").css("z-index", 1101);
    $("#main-nav").css("z-index", 1100);
    $("#DownloadProgress").modal("hide");
    $("#downProgressRate").css("width", "1%");
}

window.updateProgressMessage = (id, message, progress) => {
    /*$("#" + id).html(message);
    $("#progress" + id).css("width", progress);*/
    $("#downProgressMessage").html(message);
    $("#downProgressRate").css("width", progress);
}

window.fireProgressMessage = (id, title, message) => {
    /*$(document).Toasts('create', {
        body: "<div id='" + id + "'>" + message + "</div><div class='progress progress-xs mb-2 mt-2 ' style='border-radius: 3px; width:320px;'><div id='progress" + id + "' class='progress-bar progress-bar-danger' style='width: 1%;  border-radius: 3px'></div></div>",
        title: title,
        icon: 'fas fa-file-export blue-txt mr-2 ',
        style: 'width:350px !important;',
    })*/
    $("#left-sidebar").css("z-index", 0);
    $("#main-nav").css("z-index", 0);
    $("#downProgressMessage").html(message);
    $("#DownloadProgress").modal("show");
}

window.fireToastMessage = (type, title, message) => {
    var cls = "bg-success";
    if (type == "success")
        cls = "bg-success";
    else if (type == "info")
        cls = "bg-info";
    else if (type == "warning")
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
var zIndex = 1101;
window.openPopUp = (popUpId) => {
	//여기 인덱스를 강제로 내리는것은 문제있는 코드인데 일단 정확한 사유를 몰라 그냥 둠 2021/03/08 YKH
	//
	if (popUpId == "PopUpLogIn" || popUpId == "GPKIPopUp" || popUpId == "modal-pwchange-sidebar" || popUpId == "modal-pwchangedefaultpw-sidebar"
        || popUpId == "ProxyApprover" || popUpId == "ProxyApproverTreePopUp" || popUpId == "SecurityApproverSelectPopUp" || popUpId == "PopUpSelectClipType" ) {
		$("#left-sidebar").css("z-index", 2202);
		$("#main-nav").css("z-index", 2202);
    }
    else if (popUpId == "HeaderUIApporveAfterAlert" || popUpId == "HeaderUIApporveAfterMyCountAlert" || popUpId == "HeaderUIUpdateStartAlert")
	{
		$("#main-nav").css("z-index", 2203);
    }
    else if (popUpId == "modal-displaylock") {
        $("#left-sidebar").css("z-index", 0);
        $("#main-nav").css("z-index", 0);
    }
    zIndex = zIndex + 1;
    $("#" + popUpId).css("z-index", zIndex);
    $("#" + popUpId).modal("show");
    $("#" + popUpId).focus();
}

window.closePopUp = (popUpId) => {

    $("#left-sidebar").css("z-index", 1101);
    $("#main-nav").css("z-index", 1100);
    $("#" + popUpId).modal("hide");
}

window.closeAllPopup = () => {
    $("#modal-certificate").modal("hide");
    $("#PopUpSelectClipType").modal("hide");
    $("#DownloadProgress").modal("hide");
    $("#modal-dropprogress").modal("hide");
    $("#modal-envloading").modal("hide");
    $("#FileSelectPopUp").modal("hide");
    $("#FileSelectPopUpForSingleSelect").modal("hide");
    $("#modal-googleotp").modal("hide");
    $("#modal-default").modal("hide");
    $("#hanssak-otp").modal("hide");
    $("#modal-displaylock").modal("hide");
    $("#GPKIPopUp").modal("hide");
    $("#modal-mail").modal("hide");
    $("#modal-pcurl").modal("hide");
    $("#modal-securitynetwork").modal("hide");
    $("#MainLogIn").modal("hide");
    $("#modal-offline").modal("hide");
    $("#modal-OTPPopUp").modal("hide");
    $("PopUpLogIn").modal("hide");
    $("#ProxyApprover").modal("hide");
    $("#modal-pwchange-main").modal("hide");
    $("#modal-pwchange-sidebar").modal("hide");
    $("#modal-pwchangedefaultpw-main").modal("hide");
    $("#modal-pwchangedefaultpw-sidebar").modal("hide");
    $("#modal-pwchangeuser").modal("hide");
    $("#modal-selectsavefolder").modal("hide");
    $("#modal-alert-main").modal("hide");
    $("#modal-alert-header").modal("hide");
    $("#modal-alert-header-updatepolicy").modal("hide");
    $("#modal-afterapprovealert-header").modal("hide");
    $("#modal-alert-popuplogin").modal("hide");
    $("#ApporveAfterAlert").modal("hide");
    $("#ContinueFileTrans").modal("hide");
    $("#DashBoardContinueFileTransAlert").modal("hide");
    $("#HeaderUIApporveAfterAlert").modal("hide");
    $("#HeaderUIApporveAfterMyCountAlert").modal("hide");
    $("#HeaderUIUpdateStartAlert").modal("hide");    
    $("#SGBasicSelect").modal("hide");
    $("#modal-capcha").modal("hide");
    $("#SGConfirm").modal("hide");
    $("#SGCustomSelect").modal("hide");
    $("#modalDetailReject").modal("hide");
    $("#modal-pdf").modal("hide");
    $("#modalReject").modal("hide");
    $("#modalSecureReject").modal("hide");
    $("#Transfer_Denied").modal("hide");
    $("#modal-transprogress").modal("hide");
    $("#modal-UpdatePopUp").modal("hide");
    $("#modal-virusreport").modal("hide");
    $("#ClipboardApprovePopUp").modal("hide");
    $("#ClipboardManagePopUp").modal("hide");
    $("#MailApprovePopUp").modal("hide");
    $("#MailManagePopUp").modal("hide");
    $("#PcurlApprovePopUp").modal("hide");
    $("#PcurlManagePopUp").modal("hide");
    $("#ApprovePopUp").modal("hide");
    $("#ApproverSelect_PopUp").modal("hide");
    $("#ApproverSelect_StepPopUp").modal("hide");
    $("#ApproveExtApproverSelect_PopUp").modal("hide");
    $("#ApproverSelect_TreePopUp").modal("hide");
    $("#ReceiverSelect_PopUp").modal("hide");
    $("#TransPopUp").modal("hide");
    $("#modal-selectreceivefolder").modal("hide");

    $("#ZipPreviewPopUp").modal("hide");
    $("#TransPopUp").modal("hide");
    $("#SecurityPopUp").modal("hide");
    $("#SecurityConfirm").modal("hide");
    $("#SecurityApproverSelectPopUp").modal("hide");
    $("#ReceiverSelect_PopUp").modal("hide");
    $("#modal-pcurlavailablelist").modal("hide");
    $("#modal-pcurlusingregist").modal("hide");
    $("#PublicBoardView_PopUp").modal("hide");
    $("#ViewDetail_PopUp").modal("hide");
    $("#ClipboardApprovePopUp").modal("hide");
    $("#ClipboardManagePopUp").modal("hide");
}

window.initTransferUIPosition = () => {
    $("#selectDestNetWork").css("position", "relative");
    $("#selectDestNetWork").css("top", "0px");
}

window.initApproveUIPosition = () => {
    $("#selectApprKindValue").css("position", "relative");
    $("#selectApprKindValue").css("top", "0px");
}

window.initTransManageUIPosition = () => {
    $("#selectTransKindValue").css("position", "relative");
    $("#selectTransKindValue").css("top", "0px");
}

window.initDatePicker = (sId, eId) => {
    $("#" + sId).datepicker({
        monthNames: ['1월', '2월', '3월', '4월', '5월', '6월', '7월', '8월', '9월', '10월', '11월', '12월'],
        dayNamesMin: ['일', '월', '화', '수', '목', '금', '토'],
        dayNames: ['일요일', '월요일', '화요일', '수요일', '목요일', '금요일', '토요일'],
        autoclose: true,
        dateFormat: 'yy-mm-dd'
    })
    $("#" + eId).datepicker({
        monthNames: ['1월', '2월', '3월', '4월', '5월', '6월', '7월', '8월', '9월', '10월', '11월', '12월'],
        dayNamesMin: ['일', '월', '화', '수', '목', '금', '토'],
        dayNames: ['일요일', '월요일', '화요일', '수요일', '목요일', '금요일', '토요일'],
        autoclose: true,
        dateFormat: "yy-mm-dd"
    })
}

function stopClickOpen(e) {
    e.preventDefault();
}

window.stopClick = (message) => {
    /*$('input[type="file"]').click(function (event) {
        event.preventDefault();
    });*/
}

window.startClick = () => {
    var clickNum = nTargetInput + 1;
    $("#fileInputTrans").trigger("click");
}

window.InitDragAndDrop = (message) => {
    if (
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
    //console.log(Math.floor(+ new Date() / 1000) - MouseTime);
    //console.log("INPUT TIME:" + minuteTime * 60);

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
        if (divRightUpper != null && divRightBottom != null) {
            var divRest = parseInt(divRightUpper.replace("px", "")) + parseInt(divRightBottom.replace("px", ""));
            $("#divDropFile").css("height", (parseInt(dirRightHeight.replace("px", "")) - (divRest + 7)) + "px");
        }
    });
}
//F5 키 눌러서 화면고침 금지(WebView화면고침에러)
window.addKeyDown = () => {
    document.addEventListener('keydown', function (e) {
        var kcode = event.keyCode;
        //backspace 방지
        if (kcode == 8) { //backspace
            if (e.target.nodeName != "INPUT" && e.target.nodeName != "TEXTAREA" && e.target.nodeName != "SELECT") {
                event.returnValue = false;
            }
            if (e.target.tagName == "SELECT") {
                event.returnValue = false;
            }
            if (e.target.tagName == "INPUT" && e.target.getAttribute("readonly") == "readonly") {
                event.returnValue = false;
            }
        }
        if (kcode == 116) {	//F5
            //console.log("F5 not allowed.");
            event.returnValue = false;
        }
    }, false);
}

window.initMouseClick = () => {
    MouseTime = Math.floor(+ new Date() / 1000);
}

//var MouseTime = 0;
var MouseTime = Math.floor(+ new Date() / 1000);
window.addMouseDown = (message) => {
    document.addEventListener('mousedown', function (e) {


        if (MouseTime == Math.floor(+ new Date() / 1000))
            return;
        MouseTime = Math.floor(+ new Date() / 1000);
        console.log("MOUSE DOWN EVENT " + e.target.getAttribute('name') + " MouseTime:" + MouseTime);


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
        if (e.target.parentElement.getAttribute('name') == "trItem5") {

            clearTrSelections();
            addTrSelection(e.target.parentElement, 5);
            return;
        }
        if (e.target.parentElement.getAttribute('name') == "trItem6") {

            clearTrSelections();
            addTrSelection(e.target.parentElement, 6);
            return;
        }

        if (e.target.parentElement.getAttribute('name') == "trItem7") {

            clearTrSelections();
            addTrSelection(e.target.parentElement, 7);
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
        if (e.target.parentElement.getAttribute('name') == "trSelect5") {

            clearTrTargetSelections(true);
            addTrTargetSelection(e.target.parentElement, 5);
            return;
        }
        else if (e.target.parentElement.getAttribute('name') == "trSelect6") {

            clearTrTargetSelections(true);
            addTrTargetSelection(e.target.parentElement, 6);
            return;
        }
        else if (e.target.parentElement.getAttribute('name') == "trSelect7") {

            clearTrTargetSelections(true);
            addTrTargetSelection(e.target.parentElement, 7);
            return;
        }

        //팝업파일선택
        if (e.target.getAttribute('name') == "popfile") {
            //if the multiple selection modifier is not pressed 
            //and the item's grabbed state is currently false
            if (!hasModifier(e) && e.target.getAttribute('aria-grabbed') == 'false' && e.target.getAttribute('name') != "popUpOkBtn") {
                //clear all existing selections
                clearSelections();
                //then add this new selection
                addSelection(e.target);
                firstShift = e.target.getAttribute('label');
                console.log("First SHIFT KEY:" + firstShift);
                return;
            }

            if (hasShitfKey(e) == true && e.target.getAttribute('aria-grabbed') == 'false') {
                secondShift = e.target.getAttribute('label');
                console.log("Second SHIFT KEY:" + secondShift);
                if ((firstShift != secondShift) && firstShift > 0 && secondShift > 0) {
                    clearSelections();
                    ShiftPopSelection(firstShift, secondShift);
                    return;
                }
            }
        }
        //기본파일선택
        if (e.target.getAttribute('draggable')) {
            //if the multiple selection modifier is not pressed 
            //and the item's grabbed state is currently false
            if (!hasModifier(e) && e.target.getAttribute('aria-grabbed') == 'false') {
                //clear all existing selections
                clearSelections();
                //then add this new selection
                addSelection(e.target);
                firstShift = e.target.getAttribute('label');
                console.log("First SHIFT KEY:" + firstShift);
                return;
            }

            if (hasShitfKey(e) == true && e.target.getAttribute('aria-grabbed') == 'false') {
                secondShift = e.target.getAttribute('label');
                console.log("Second SHIFT KEY:" + secondShift);
                if ((firstShift != secondShift) && firstShift > 0 && secondShift > 0) {
                    clearSelections();
                    ShiftSelection(firstShift, secondShift);
                    return;
                }
            }
        }
        //else [if the element is anything else]
        //and the selection modifier is not pressed 
        else if (!hasModifier(e)) {

            if (e.target == null || e.target.getAttribute('name') == null) {
                console.log("Clear Selection target is null");
            }
            else if (e.target.getAttribute('name') != "popUpOkBtn") {   //팝업파일선택은 클릭이 넘어와도 OK버튼 이면 지우면 안된다.
                console.log("Clear Selection is called in target name!!!" + e.target.innerText);
                console.log("Clear Selection is called in mousedown listener!!!" + e.target.getAttribute('name'));
                clearSelections();
                firstShift = 0;
                secondShift = 0;
            }
        }

    }, false);
}

function ShiftPopSelection(firstShift, secondShift) {
    console.log("ShiftPopSelection start");
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
        items = document.querySelectorAll('[name="popfile"]'),
        len = items.length,
        i = 0; i < len; i++) {

        if (i >= tempFirst && i <= tempSecond) {
            addSelection(items[i]);
        }
    }
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

        if (e.target.getAttribute('draggable') && hasModifier(e)) {
            //if the item's grabbed state is currently true
            //console.log("MOUSE UP EVENT");
            if (e.target.getAttribute('aria-grabbed') == 'true') {
                //unselect this item
                if (hasShitfKey(e) != true && hasCtrlKey(e) != true) {
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
    else if (index == 5)
        DotNet.invokeMethodAsync("OpenNetLinkApp", "ProxyTargetSelect2", item.getAttribute('value'));
    else if (index == 6)
        DotNet.invokeMethodAsync("OpenNetLinkApp", "ReceiverTargetSelect", item.getAttribute('value'));
    else if (index == 7)
        DotNet.invokeMethodAsync("OpenNetLinkApp", "ApproveExtTargetSelect", item.getAttribute('value'));    

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
        if (remove == true)
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
    else if (index == 5)
        DotNet.invokeMethodAsync("OpenNetLinkApp", "ProxySearchSelect2", item.getAttribute('value'));
    else if (index == 6)
        DotNet.invokeMethodAsync("OpenNetLinkApp", "ReceiverSearchSelect", item.getAttribute('value'));
    else if (index == 7)
        DotNet.invokeMethodAsync("OpenNetLinkApp", "ApproveExtSearchSelect", item.getAttribute('value'));
    else if (index == 8)
        DotNet.invokeMethodAsync("OpenNetLinkApp", "ProxySearchSelect3", item.getAttribute('value'));
    else if (index == 9)
        DotNet.invokeMethodAsync("OpenNetLinkApp", "DeptTreeSelect", item.getAttribute('value'));

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
    console.log("ADD SELECTION : " + item.getAttribute('value') + "  TYPE:" + item.getAttribute("label"));
    if (nTransferUIIndex == 1)
        DotNet.invokeMethodAsync("OpenNetLinkApp", "AddPath", item.getAttribute('value'), item.getAttribute("label"));
    else
        DotNet.invokeMethodAsync("OpenNetLinkApp", "AddPath2", item.getAttribute('value'), item.getAttribute("label"));

    //add it to the items array
    selections.items.push(item);
    console.log(selections.items);
}

window.getFileSelection = () => {

    $("[name='popfile']").attr('aria-grabbed', 'false');
    var rtn = "";
    for (var i = 0; i < selections.items.length; i++) {
        if (i < selections.items.length - 1)
            rtn += selections.items[i].getAttribute('value') + "[HSDELIMETER]";
        else
            rtn += selections.items[i].getAttribute('value');
    }
    selections.items = [];
    return rtn;
}

//function for unselecting an item
function removeSelection(item) {
    //reset this item's grabbed state
    item.setAttribute('aria-grabbed', 'false');
    console.log("REMOVE SELECTION : " + item.getAttribute('value'));
    if (nTransferUIIndex == 1)
        DotNet.invokeMethodAsync("OpenNetLinkApp", "RemovePath", item.getAttribute('value'));
    else
        DotNet.invokeMethodAsync("OpenNetLinkApp", "RemovePath2", item.getAttribute('value'));
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
        if (nTransferUIIndex == 1)
            DotNet.invokeMethodAsync("OpenNetLinkApp", "ClearPath");
        else
            DotNet.invokeMethodAsync("OpenNetLinkApp", "ClearPath2");
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

window.preventDragStart = () => {
    document.addEventListener("dragover", function (e) {
        e = e || event;
        if (e.target.id.indexOf("fileInput") == -1) {
            // check which element is our target 
            e.preventDefault();
        }
    }, false);
}

window.preventDrop = () => {
    document.addEventListener("drop", function (e) {
        e = e || event;
        if (e.target.id.indexOf("fileInput") == -1) {
            // check which element is our target 
            e.preventDefault();
        }
    }, false);
}


window.addDragStart = (message) => {
    document.addEventListener('dragstart', function (e) {
        if
            (
            hasModifier(e)
            &&
            e.target.getAttribute('aria-grabbed') == 'false'
        ) {
            //add this additional selection
            addSelection(e.target);
        }
        e.dataTransfer.setData('text', '');

    }, false);
}