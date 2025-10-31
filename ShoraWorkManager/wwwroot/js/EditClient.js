function SubmitEditClientForm(clientId)
{
    const form = document.getElementById("editPartialForm-" + clientId);
    const formData = new FormData(form);

    $.ajax({
        url: '/Clients/EditPartial',
        type: "POST",
        data: formData,
        processData: false, 
        contentType: false, 
        success: function (result)
        {
            $("#editPartialForm-" + clientId).html(result);
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });
}

function GetEditClientForm(clientId) {
    $.ajax({
        url: '/Clients/EditPartial',
        type: "GET",
        data: {
            id: clientId
        },
        success: function (result) {
            $("#editPartial-" + clientId).html(result);
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });
}