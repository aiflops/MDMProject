var utils = {
    isEmptyOrSpaces: function (str) {
        return str === null || str === undefined || str.match(/^ *$/) !== null;
    },
    arrayForEach: function (array, action) {
        for (var i = 0, j = array.length; i < j; i++)
            action(array[i]);
    },
    arrayMap: function (array, mapping) {
        array = array || [];
        var result = [];
        for (var i = 0, j = array.length; i < j; i++)
            result.push(mapping(array[i]));
        return result;
    }
};

window.utils = utils;