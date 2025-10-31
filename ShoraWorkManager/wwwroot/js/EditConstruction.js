function SubmitEditConstructionSiteForm(constructionSiteId)
{
    const form = document.getElementById("editPartialForm-" + constructionSiteId);
    const formData = new FormData(form);

    $.ajax({
        url: '/ConstructionSites/EditPartial/' + constructionSiteId,
        type: "POST",
        data: formData,
        processData: false, 
        contentType: false, 
        success: function (result)
        {
            if (typeof result === "object" && result.isSuccess) {

                window.location = result.returnUrl;

            } else { //Significa que retornar a partial View
                $("#editPartial-" + constructionSiteId).html(result);
            }
            
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });
}

function GetEditConstructionSiteForm(constructionSiteId) {
    $.ajax({
        url: '/ConstructionSites/EditPartial',
        type: "GET",
        data: {
            id: constructionSiteId
        },
        success: function (result) {
            $("#editPartial-" + constructionSiteId).html(result);
        },
        error: function (xhr, status, error) {
            console.error('Error: ', status, error);
            console.log('Response Text: ', xhr.responseText);
        }
    });
}