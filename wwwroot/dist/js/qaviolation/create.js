$('#ViolationCategory').on('change', async () => {
    fetchSubCategory();
})
$('#SubCategory').on('change', async () => {
    fetchPenaltyDetail();
})

$('#violationForm').validate({
    rules: {
        vendorEmail: {
            required: true,
            email: true
        },
        observationDetails: {
            required: true
        },
        violationCategory: {
            required: true
        },
        subCategory: {
            required: true
        },
        amount: {
            required: true
        }

    },
    submitHandler: async function (form, event) {
        event.preventDefault();
        const request = new FormData(form);
        submitForm(request);

    }
});

async function fetchSubCategory() {
    const value = $('#ViolationCategory').val();
    try {
        const response = await fetch('QaViolationSubCategories/ByCategory/' + value)
        const data = await response.json();
        let html = "<option value=''>Choose...</option>"
        for (let i = 0; i < data.length; i++) {
            html += `<option value=${data[i].id}> ${data[i].name} </option>`
        }
        console.log(html);
        $('#SubCategory').html(html);
    }
    catch (error) {
        Swal.fire(error.message + " while fetching sub category", "error")
    }
}

async function fetchPenaltyDetail() {
    const value = $('#SubCategory').val();
    try {
        const response = await fetch('PenaltyDetails/BySubCategory/' + value)
        const data = await response.json();

        $('#financialPenalty').html(data.financialPenalty);
        $('#administrative').html(data.administrative);
       

        const maxVal = parseFloat(data.max_amount); // float like 123.45

        //$('#Amount').val(data.max_amount); // optional default value
        $('#Amount').attr('max', maxVal);
        $('#Amount').attr('step', '0.01'); // allow decimal input
        document.getElementById('Amount').max = maxVal;

       // alert('Max value set to: ' + maxVal);
    }

    catch (error) {
        Swal.fire({
            icon: "error",
            title: error.message
        })
    }
}

async function submitForm(data) {
    try {
        const response = await fetch('QaViolations/Create', {
            method: "POST",
            body: data,
        })
        const result = await response.json()
        if (response.status != 200) {

            Swal.fire({
                icon: "error",
                title: result.title
            })
        }
        else {
            Swal.fire({
                icon: "success",
                title: "Violation Request Created"
            }).then(() => {
                location.href = "QAViolations/Pending"
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