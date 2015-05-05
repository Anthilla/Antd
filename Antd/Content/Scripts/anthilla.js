var localhost = '/gw/';

$(document).ready(function () { $('input:text').attr('autocomplete', 'off'); });

function Reset() {
    $('.item').hide();
    $('select').prop('selectedIndex', 0);
    $('.project-selectable').removeClass('picked');
    $('.group-selectable').removeClass('picked');
    $('.js-files').hide();
    $("input:text").each(function () {
        $(this).val("");
    });
    $('.file').remove();
    //$('.file').hide();
    return false;
}

function SetCreate() {
    var button = $('#create-button');
    if (button != null) {
        button.toggleClass('fg-anthilla-green');
        button.toggleClass('no-overlay');
        button.toggleClass('fg-anthilla-gray');
        button.toggleClass('bg-anthilla-green');
    }
    $('#DashboardForm').toggle();
    return false;
}

function Quit() {
    Reset();
    SetCreate();
    return false;
}

function QuitTimingPanel(panel) {
    $('#' + panel).hide();
    return false;
}

$('.selectable').on('click', function (e) {
    $(this).toggleClass("selected");
});

var UgBtn = $('#create-button-ug');
var FgBtn = $('#create-button-fg');
var RgBtn = $('#create-button-rg');

function SwitchBtnCol(btn) {
    btn.toggleClass('fg-anthilla-green');
    btn.toggleClass('fg-anthilla-gray');
    btn.toggleClass('bg-anthilla-green');
}

function ResetBtnCols(btn1, btn2) {
    btn1.removeClass('fg-anthilla-green');
    btn1.removeClass('fg-anthilla-gray');
    btn1.removeClass('bg-anthilla-green');
    btn2.removeClass('fg-anthilla-green');
    btn2.removeClass('fg-anthilla-gray');
    btn2.removeClass('bg-anthilla-green');
}

function SetCreateUG() {
    SwitchBtnCol(UgBtn);
    ResetBtnCols(FgBtn, RgBtn);
    $('#UserGroupForm').toggle();
    $('#FunctionGroupForm').hide();
    $('#GroupRelationForm').hide();
}
function SetCreateFG() {
    SwitchBtnCol(FgBtn);
    UgBtn.addClass('fg-anthilla-green');
    UgBtn.addClass('no-overlay');
    UgBtn.removeClass('fg-anthilla-gray');
    UgBtn.removeClass('bg-anthilla-green');
    RgBtn.addClass('fg-anthilla-green');
    RgBtn.addClass('no-overlay');
    RgBtn.removeClass('fg-anthilla-gray');
    RgBtn.removeClass('bg-anthilla-green');
    $('#UserGroupForm').hide();
    $('#FunctionGroupForm').toggle();
    $('#GroupRelationForm').hide();
}
function SetCreateRG() {
    SwitchBtnCol(RgBtn);
    UgBtn.addClass('fg-anthilla-green');
    UgBtn.addClass('no-overlay');
    UgBtn.removeClass('fg-anthilla-gray');
    UgBtn.removeClass('bg-anthilla-green');
    FgBtn.addClass('fg-anthilla-green');
    FgBtn.addClass('no-overlay');
    FgBtn.removeClass('fg-anthilla-gray');
    FgBtn.removeClass('bg-anthilla-green');
    $('#UserGroupForm').hide();
    $('#FunctionGroupForm').hide();
    $('#GroupRelationForm').toggle();
}

function QuitUG() {
    Reset();
    $('.dashboard-header>a').show();
    $('.dashboard-content').hide();
    $('.dashboard-content>#DashboardUG').hide();
}

function QuitFG() {
    Reset();
    $('.dashboard-header>a').show();
    $('.dashboard-content').hide();
    $('.dashboard-content>#DashboardFG').hide();
}

function QuitRG() {
    Reset();
    $('.dashboard-header>a').show();
    $('.dashboard-content').hide();
    $('.dashboard-content>#DashboardRG').hide();
}

//Feedback
function SetFeedback() {
    $('#FeedbackForm').toggle();
    return false;
}

function QuitFeedback() {
    $('#FeedbackForm').hide();
    return false;
}

function ShowProjectCode(project) {
    jQuery.support.cors = true;
    $.ajax({
        url: '/rawdata/get/project/' + project,
        type: 'GET',
        dataType: 'json',
        contentType: "application/json;charset=utf-8",
        success: function (project) {
            $("#ProjectCode").text(project.AnthillaProjectCode);
        }
    });
}

function ShowProjectVersion(packageGuid) {
    jQuery.support.cors = true;
    $.ajax({
        url: '/rawdata/get/file/' + packageGuid,
        type: 'GET',
        dataType: 'json',
        contentType: "application/json;charset=utf-8",
        success: function (packageGuid) {
            $("#ProjectVersion").text(packageGuid.AnthillaHierarchyIndex);
        }
    });
}

function ShowList() {
    $('#Calendar-list-view').show();
    $('#Calendar-grid-view').hide();
}
function ShowGrid() {
    $('#Calendar-grid-view').show();
    $('#Calendar-list-view').hide();
}

function ShowLog() {
    $('#Log-list-view').show();
    $('#Request-list-view').hide();
}
function ShowRequest() {
    $('#Request-list-view').show();
    $('#Log-list-view').hide();
}

//------------------time picker........
//$('#time-end').click(function () {
//    $('#timeEndArea').toggle();
//});

//$('#timeEndArea').find('a').click(function () {
//    var input = $('#time-end');
//    var cell = $(this);
//    var h = cell.attr('data-hour');
//    input.val(h);
//    $('#timeEndArea').hide();
//    return false;
//});

//$('.end-selector').windowed({
//    change: function (event, selected) {
//        $('#timeEndArea').show();
//        var selected = $(selected);
//        var am = $('#end-am-hs');
//        var pm = $('#end-pm-hs');
//        if (selected.text() == 'AM') {
//            am.show();
//            pm.hide();
//        }
//        if (selected.text() == 'PM') {
//            am.hide();
//            pm.show();
//        }
//    }
//});

//$('#time-start').click(function () {
//    $('#timeStartArea').toggle();
//});

//$('#timeStartArea').find('a').click(function () {
//    var input = $('#time-start');
//    var cell = $(this);
//    var h = cell.attr('data-hour');
//    input.val(h);
//    $('#timeStartArea').hide();
//    SetEndHour();
//    return false;
//});

//$('.start-selector').windowed({
//    change: function (event, selected) {
//        $('#timeStartArea').show();
//        var selected = $(selected);
//        var am = $('#sta-am-hs');
//        var pm = $('#sta-pm-hs');
//        if (selected.text() == 'AM') {
//            am.show();
//            pm.hide();
//        }
//        if (selected.text() == 'PM') {
//            am.hide();
//            pm.show();
//        }
//    }
//});