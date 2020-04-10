$.validator.addMethod("enforcetrue", function (value, element, param) {
    return element.checked;
});
$.validator.unobtrusive.adapters.addBool("enforcetrue");