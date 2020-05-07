$(document).ready(function () {
    $("[name='UserType']").change(refreshItems);

    refreshItems();
});

function refreshItems() {
    $("[name='UserType']").each(function () {
        if (this.checked && this.value === 'Individual') {
            $('.company-field').fadeOut(500);
            $('.individual-field').delay(500).fadeIn(500);
        }
        if (this.checked && this.value === 'Company') {
            $('.individual-field').fadeOut(500);
            $('.company-field').delay(500).fadeIn(500);
        }
    });
}