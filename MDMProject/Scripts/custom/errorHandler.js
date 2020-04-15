"use strict";
var ErrorHandle = (function () {

    function onError(msg, url, line, col, error) {

        try {
            var data = {
                msg: msg,
                url: url,
                line: line,
                col: col,
                stack: error != null ? (error.stack != null ? error.stack : error) : null,
                extra: {
                    agent: navigator.userAgent,
                    language: navigator.language,
                    platform: navigator.platform,
                    pageUrl: window.location.href
                }
            };

            var message = "Unexpected error";

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: '/api/Log/',
                data: JSON.stringify(data),
                success: function () {
                    // OK
                },
                error: function () {
                    console.log(message);
                }
            });

        } catch (e) {
            console.log('do not catch on error ' + e);
        }
    }

    window.onerror = onError;
})();
