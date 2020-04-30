$('#modal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget) // Button that triggered the modal
    var windowTitle = button.data('title');
    var buttonTitle = button.data('button-title');

    var modal = $(this);
    modal.find('.modal-title').text(windowTitle);

    var submitButton = modal.find('.modal-footer .btn-primary');
    submitButton.text(buttonTitle);

    var modalBody = modal.find('.modal-body');

    var formUrl = button.data('form-url');

    $.get(formUrl, function (data) {
        modalBody.html(data);
        submitButton.show();

        submitButton.click(function () {
            var form = modalBody.find('form');

            //get the action-url of the form
            var actionurl = form[0].action;
            var serializedData = objectifyForm(form.serializeArray());

            var token = serializedData.__RequestVerificationToken;

            //do your own request an handle the results
            $.ajax({
                url: actionurl,
                type: 'POST',
                dataType: 'json',
                data: serializedData,
                beforeSend: function (request) {
                    request.setRequestHeader("RequestVerificationToken", token);
                },
                success: function (data) {
                    var cancelButton = modal.find('.modal-footer .btn-secondary');
                    cancelButton.text('Ok');

                    if (data.success) {
                        modalBody.html('<div>' + data.message + '</div>');
                    }
                    else {
                        modalBody.html('<div class="text-danger">' + data.message + '</div>');
                    }

                    submitButton.text('');
                    submitButton.hide();
                    submitButton.off('click');

                    modal.attr('data-is-form-finished', 'true');
                }
            });
        });
    });
});

$('#modal').on('hide.bs.modal', function (e) {
    var modal = $(this)
    var submitButton = modal.find('.modal-footer .btn-primary');
    var cancelButton = modal.find('.modal-footer .btn-secondary');

    submitButton.text('');
    submitButton.hide();
    submitButton.off('click');

    cancelButton.text('Anuluj');

    if (modal.attr('data-is-form-finished') == 'true') {
        setTimeout(function () {
            location.reload();
        }, 500);
    }
});

function objectifyForm(formArray) {//serialize data function

    var returnArray = {};
    for (var i = 0; i < formArray.length; i++) {
        returnArray[formArray[i]['name']] = formArray[i]['value'];
    }
    return returnArray;
}