function deleteSubCategory(id) {
    showConfirm(async () => {
        try {
            await fetch('QaViolationSubCategories/delete/' + id, {
                method: "POST"
            })
            location.reload();
        }
        catch(e) {
            Swal.fire(e.message, 'error');
        }
    })
}

function showConfirm(callback) {
    Swal.fire({
        title: "Do you want to delete the sub category?",
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: "Yes",
        denyButtonText: `No`
    }).then((result) => {

        if (result) {
            callback();
        }
    });
}