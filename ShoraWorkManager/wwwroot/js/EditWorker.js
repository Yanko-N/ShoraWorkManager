function SubmitEditWorkerForm(workerId)
{
    const form = document.getElementById("editPartialForm-" + workerId);
    const formData = new FormData(form);

    $.ajax({
        url: '/Workers/EditPartial',
        type: "POST",
        data: formData,
        processData: false, 
        contentType: false, 
        success: function (result)
        {
            $("#editPartialForm-" + workerId).html(result);
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });
}

function GetEditWorkerForm(workerId) {
    $.ajax({
        url: '/Workers/EditPartial',
        type: "GET",
        data: {
            id: workerId
        },
        success: function (result) {
            $("#editPartial-" + workerId).html(result);
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });
}