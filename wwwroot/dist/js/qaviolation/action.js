$('#approval-form').validate({
    rules: {
        PenaltyClauseCorrect: {
            required: true
        },
        PenaltyAmountCorrect: {
            required: true
        },
        Comment: {
            required: true
        },
    },
    submitHandler: async function (form, event) {
        event.preventDefault();
        const request = new FormData(form);
        submitForm('QaViolations/Approval', request);
    }
});

$('#cfo-form').validate({
    rules: {
        Comment: {
            required: true
        },
        DeducationDate: {
            required: true
        },
        DebitNote: {
            required: true
        }
    },
    submitHandler: async function (form, event) {
        event.preventDefault();
        const request = new FormData(form);
        submitForm('QaViolations/CFOSubmit', request);
    }
})

$('#head-procurement-form').validate({
    rules: {
        Comment: {
            required: true
        }
    },
    submitHandler: async function (form, event) {
        event.preventDefault();
        const request = new FormData(form);
        submitForm('QaViolations/HeadProcurementSubmit', request);
    }
})

async function submitForm(url, data) {
    try {
        const response = await fetch(url, {
            method: "POST",
            body: data,
        })
        const result = await response.json();
        if (response.status == 200) {
            Swal.fire({
                icon: "success",
                title: result.message
            }).then(() => {
                location.href = "QAViolations/Pending"
            })
        }
        else {
            Swal.fire({
                icon: "error",
                title: result.title ?? "Something went wrong"
            })
        }
    }
    catch (error) {
        Swal.fire({
            icon: "error",
            title: error.message
        })
    }
}