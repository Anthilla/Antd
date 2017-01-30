function Toggle(element) {
    $(element).toggle();
}

function Hide(element) {
    $(element).hide();
}

function Show(element) {
    $(element).show();
}

function HideLoading() {
    $("#LoadingSpinner").hide();
}

function ShowLoading() {
    $("#LoadingSpinner").show();
}

$('[data-element="ToolbarButton"]').click(function () {
    var self = $(this);
    $('[data-element="ToolbarButton"]').each(function () {
        $(this).removeClass("bg-anthilla-green");
        $(this).removeClass("fg-anthilla-gray-2");
        $(this).removeClass("bd-anthilla-green");
        $(this).addClass("bg-anthilla-gray-2");
        $(this).addClass("fg-anthilla-green");
        $(this).addClass("bd-anthilla-gray-2");
    });
    self.removeClass("bg-anthilla-gray-2");
    self.removeClass("fg-anthilla-green");
    self.removeClass("bd-anthilla-gray-2");
    self.addClass("bg-anthilla-green");
    self.addClass("fg-anthilla-gray-2");
    self.addClass("bd-anthilla-green");
});

function SelectToolbarButton() {
    var self = $(this);
    $('[data-element="ToolbarButton"]').each(function () {
        $(this).removeClass("bg-anthilla-green");
        $(this).removeClass("fg-anthilla-gray-2");
        $(this).removeClass("bd-anthilla-green");
        $(this).addClass("bg-anthilla-gray-2");
        $(this).addClass("fg-anthilla-green");
        $(this).addClass("bd-anthilla-gray-2");
    });
    self.removeClass("bg-anthilla-gray-2");
    self.removeClass("fg-anthilla-green");
    self.removeClass("bd-anthilla-gray-2");
    self.addClass("bg-anthilla-green");
    self.addClass("fg-anthilla-gray-2");
    self.addClass("bd-anthilla-green");
}

function GetValue(element) {
    return $(element).val();
}

function FindValue(parent, child) {
    return $(parent).find(child).val();
}

var RequestStatus = [
    "EMPTY",
    "OPENNEW",
    "OPENASSIGNED",
    "OPENINPROGRESS",
    "OPENINFORQUESTED",
    "OPENHOLD",
    "OPENBLOCKED",
    "RESOLVEDSOLVED",
    "RESOLVEDCLOSED"
];

$(document).ready(function () {
    $(".textEditor").trumbowyg();
});

