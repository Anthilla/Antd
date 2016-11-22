var $config = jQuery.noConflict();

$config('input[data-role="command-place"]').keyup(function () {
    var thisValue = $config(this).val();
    if (thisValue.indexOf("[") > 0) {
        //todo: in questo caso prendere la stringa che si sta per scrivere dopo la [
        //      fare una query, tipo selectize, nei ValueBundle e mostrare tutt i valori appartenenti a quel tag
        //      con relativo indice
        //      in questo modo puoi già scrivere il tag esatto senza dover andare a filtrare la tabella dei valori
        //      OPPURE, questa query la si fa sulla tabella, senza fare chiamate ajax
        //      tipo il campo di input di 'Search
        //      e in base a quello che scrivi ti mostra solo le righe della tabella interessate
    }
    if (thisValue.length > 0) {
        jQuery.support.cors = true;
        var aj = $config.ajax({
            url: "/cfg/layouts",
            type: "GET",
            success: function (layoutJson) {
                var results = [];
                $config.each(layoutJson, function (i, item) {
                    if (item.name.indexOf(thisValue) === 0) {
                        results.push(item.name);
                    }
                });
                if (results.length > 0) {
                    DisplayResults(results);
                }
                else {
                    CleanResults();
                    HideDisplayer();
                }
            }
        });
        _requests.push(aj);
    }
    else {
        CleanResults();
        HideDisplayer();
    }
});

function ShowDisplayer() {
    $config("#ResultsContainer").show();
}

function HideDisplayer() {
    $config("#ResultsContainer").hide();
}

function DisplayResults(objectsToDisplay) {
    ShowDisplayer();
    CleanResults();
    $config.each(objectsToDisplay, function (i, item) {
        $config("#ResultsContainer").append('<li data-role="query-result" class="bg-anthilla-violet" style="display: inline-block; padding: 3px 15px; margin: 5px;">' + item + "</li>");
        SelectLayoutToEdit();
    });
}

function CleanResults() {
    var container = $config("#ResultsContainer");
    container.html("");
}

function SelectLayoutToEdit() {
    $config('[data-role="query-result"]').dblclick(function () {
        $config('input[data-role="command-place"]').val($config(this).text());
    });
}

$config('i[data-role="show-all-command"]').mousedown(function () {
    $config('[data-role="command-saved"]').each(function () { $config(this).hide(); });
    $config('[data-role="command-exd"]').each(function () { $config(this).show(); });
});

$config('i[data-role="show-all-command"]').mouseup(function () {
    $config('[data-role="command-saved"]').each(function () { $config(this).show(); });
    $config('[data-role="command-exd"]').each(function () { $config(this).hide(); });
});

$config('i[data-role="show-command"]').mousedown(function () {
    var container = $config(this).parents("li");
    container.find('[data-role="command-saved"]').hide();
    container.find('[data-role="command-exd"]').show();
});

$config('i[data-role="show-command"]').mouseup(function () {
    var container = $config(this).parents("li");
    container.find('[data-role="command-saved"]').show();
    container.find('[data-role="command-exd"]').hide();
});

$config("#sortable").sortable({
    //sort: function () {
    //    RefreshCommandIndexes();
    //    ReindexCheck();
    //},
    stop: function () {
        RefreshCommandIndexes();
        ReindexCheck();
    },
}).disableSelection();

$config(document).on("click", "body", function () {
    RefreshCommandIndexes();
    ReindexCheck();
});

$config(document).ready(function () {
    $config("#valueBundleTag").hide();
});

function ReindexCheck() {
    $config('[data-role="control"]').each(function (index) {
        var i = index;
        $config(this).find('[data-role="DisplayIndex"]').val(i);
        $config(this).find("input").each(function () {
            var name = $config(this).attr("data-name");
            var nn = name + "_" + i;
            $config(this).attr("name", nn);
        });
    });
}

function RefreshCommandIndexes() {
    $config("input[name=Index]").each(function (index) {
        $config(this).val("");
        $config(this).val(index);
    });
}

$config('i[data-role="remove-command"]').on("click", function () {
    var g = $config(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $config.ajax({
        url: "/cfg/delcommand",
        type: "POST",
        data: {
            Guid: g
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$config('i[data-role="disable-command"]').on("click", function () {
    var g = $config(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $config.ajax({
        url: "/cfg/disablecommand",
        type: "POST",
        data: {
            Guid: g
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$config('i[data-role="enable-command"]').on("click", function () {
    var g = $config(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $config.ajax({
        url: "/cfg/enablecommand",
        type: "POST",
        data: {
            Guid: g
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$config('i[data-role="play-command"]').on("click", function () {
    var g = $config(this).attr("data-guid");
    jQuery.support.cors = true;
    var aj = $config.ajax({
        url: "/cfg/launchcommand",
        type: "POST",
        data: {
            Guid: g
        },
        success: function () {
            location.reload(true);
        }
    });
    _requests.push(aj);
});

$config('input[data-role="command-place"]').focusin(function () {
    $config("#Tip").show();
});

$config('input[data-role="command-place"]').focusout(function () {
    $config("#Tip").hide();
});
