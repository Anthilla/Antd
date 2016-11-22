var $xhr = jQuery.noConflict();

var _requests = [];

var _abortAllRequests = function () {
    $xhr(_requests).each(function (i, xhr) {
        xhr.abort();
    });
    _requests = [];
}

$xhr(window).on("beforeunload", function () {
    _abortAllRequests();
});

function AbortAllAjaxRequests() {
    _abortAllRequests();
}