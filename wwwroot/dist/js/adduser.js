$(document).ready(function () {

    $('#app').on('change', function () {
        if (this.value == "SHA") {
            $('#role').find('option')
                .remove()
                .end().append(`<option value="10">SHA Team</option>
                                <option value="11">Customer</option>
                                <option value="8">Admin</option>
                                <option value="9">Super Admin</option>`);
        } else if (this.value == "QA") {
            $('#role').find('option')
                .remove()
                .end().append(`<option value="1">QA Officer</option>
                                <option value="13">User</option>
                                <option value="4">Sectional Head QA</option>
                                <option value="5">Business Head</option>
                                <option value="6">Assignee Section</option>
                                <option value="7">Department HOD</option>
                                <option value="12">HoD DETP</option>
                                <option value="8">Admin</option>
                                <option value="9">Super Admin</option>`);
        } else if (this.value == "Design") {
            $('#role').find('option')
                .remove()
                .end().append(` <option value="8">Admin</option>
                                <option value="9">Super Admin</option>`);
        }
    });
    //Jquery validation
    $('#myform').validate({
        rules: {
            full_name: {
                required: true
            },
            personal_number: {
                required: true
            },
            email: {
                required: true
            },
            role: {
                required: true
            },
            app: {
                required: true
            }
        },
        messages: {
            full_name: {
                required: "This field is required"
            },
            personal_number: {
                required: "This field is required"
            },
            email: {
                required: "This field is required"
            }
        },
        errorElement: 'span',
        errorPlacement: function (error, element) {
            error.addClass('invalid-feedback');
            element.closest('.form-group').append(error);
        },
        highlight: function (element, errorClass, validClass) {
            $(element).addClass('is-invalid');
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).removeClass('is-invalid');
        }
    });
})

function Submit() {
    if ($('#myform').valid()) {
        $('#modal-loader').modal('show')
        $('#submit_request').prop('disabled', true)

        var dataToSend = {
            full_name: $('#full_name').val(),
            personal_number: $('#personal_number').val(),
            email: $('#email').val(),
            department: $('#department').val(),
            app: $('#app').val(),
            role: $('#role').val()
        }
        var url = $('#submit_request').data('request-url');

        $.ajax(
            {
                type: "POST",
                url: url,
                data: dataToSend,
                datatype: "json",
                error: function (result) {
                    alert("There is a Problem, Try Again!");
                },
                success: function (result) {
                    if (result.status == true) {
                        $('#modal-loader').modal('hide')
                        $('#submit_request').prop('disabled', false)
                        $('#myform').trigger('reset')
                        $('#visit_no').text(result.visit_no)
                        $('#modal-success').modal('show')
                    }
                    else {
                        alert(result.message);
                    }
                }
            });
    }
}