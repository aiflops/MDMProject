$(document).ready(function () {
    $("[name='UserType']").change(function () {
        if (this.checked && this.value == 'Individual') {
            $('.company-field').fadeOut(500);
            $('.individual-field').delay(500).fadeIn(500);
        }
        if (this.checked && this.value == 'Company') {
            $('.individual-field').fadeOut(500);
            $('.company-field').delay(500).fadeIn(500);
        }
    });

    $("[name='CoordinatorId']").change(function () {
        var otherCoordinatorId = $('[data-other-coordinator-id]').attr('data-other-coordinator-id');
        if (this.value == otherCoordinatorId) {
            $('.other-coordinator-details').fadeIn(500);
        }
        else {
            $('.other-coordinator-details').fadeOut(500);
        }
    });
});