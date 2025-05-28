//const { data } = require("jquery");

$(document).ready(function () {
    $('#myform').validate({
        rules: {
            name_of_structure: {
                required: true,
            },
            cost_center: {
                required: true,
                maxlength: 4
            },
            contact_person_name: {
                required: true
            },
            contact_person_email: {
                required: true,
                email: true
            },
            contact_person_phone: {
                required: true,
                maxlength: 10
            },
            number_of_structure: {
                required: true,
                maxlength: 2
            },
            description: {
                required: true,
            }

        },
        messages: {
            name_of_structure: {
                required: "Please enter Name of Structure"
            },
            cost_center: {
                required: "Please enter Cost Center",
                maxlength: "Maximum length is 4 digits"
            },
            contact_person_name: {
                required: "Please enter name"
            },
            contact_person_email: {
                required: "Please enter email",
                email: "Please enter valid email address"
            },
            contact_person_phone: {
                required: "Please enter phone number",
                maxlength: "Maximum length is 10 digits"
            },
            number_of_structure: {
                required: "Please enter Number of Structure",
                maxlength: "Maximum length is 2 digits"
            },
            description: {
                required: "Please enter description",
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
function SubmitRequest() {
    if ($('#myform').valid()) {
        $('#modal-loader').modal('show')
        $('#submit_sha_request').prop('disabled', true)
        var dataToSend = new FormData();
        dataToSend.append("date", $('#date').val())
        dataToSend.append("department", $('#department').val())
        dataToSend.append("name_of_structure", $('#name_of_structure').val())
        dataToSend.append("cost_center", $('#cost_center').val())
        dataToSend.append("priority", $('#priority').val())
        dataToSend.append("contact_person_name", $('#contact_person_name').val())
        dataToSend.append("contact_person_email", $('#contact_person_email').val())
        dataToSend.append("contact_person_phone", $('#contact_person_phone').val())
        dataToSend.append("location", $('#location').val())
        dataToSend.append("structure_type", $('#structure_type').val())
        dataToSend.append("number_of_structure", $('#number_of_structure').val())
        dataToSend.append("description", $('#description').val())
        var files = $('#file')[0].files;
        for (var i = 0; i < files.length; i++) {
            dataToSend.append("file", files[i]);
        }
        
        //var dataToSend = {
        //    date: ,
        //    department: $('#department').val(),
        //    name_of_structure: $('#name_of_structure').val(),
        //    cost_center: $('#cost_center').val(),
        //    priority: $('#priority').val(),
        //    contact_person_name: $('#contact_person_name').val(),
        //    contact_person_email: $('#contact_person_email').val(),
        //    contact_person_phone: $('#contact_person_phone').val(),
        //    location: $('#location').val(),
        //    structure_type: $('#structure_type').val(),
        //    number_of_structure: $('#number_of_structure').val(),
        //    description: $('#description').val()
        //}
        $.ajax(
            {
                type: "POST",
                url: $('#submit_sha_request').data('request-url'),
                data: dataToSend,
                cache: false,
                contentType: false,
                processData: false,
                error: function (result) {
                    alert("There is a Problem, Try Again!");
                },
                success: function (result) {
                    if (result.status == true) {
                        $('#modal-loader').modal('hide')
                        $('#submit_sha_request').prop('disabled', false)
                        $('#myform').trigger('reset')
                        $('#job_request_no').text(result.job_request_no)
                        $('#modal-success').modal('show')
                        setTimeout(function () {// wait for 5 secs(2)
                            window.location.href = $('#actionurl').val();
                            // then reload the page.(3)
                        }, 3000);
                    }
                    else {
                        alert(result.message);
                    }
                }
            });
    }
}