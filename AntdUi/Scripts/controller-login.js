"use strict";

var app = angular.module("loginApp", ["pascalprecht.translate"]);

app.config(function ($translateProvider) {
    $translateProvider.useUrlLoader("/translate");
    $translateProvider.preferredLanguage("it");
});