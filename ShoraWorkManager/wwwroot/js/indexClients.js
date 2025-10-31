function UpdateClientList() {
    // obter cenas da pagina
    var searchValue = document.getElementById('SearchInputId').value;
    var sortByValue = document.getElementById('SortById').value;
    var pageSizeValue = document.getElementById('PageSizeId').value;
    var orderByValue = document.querySelector('#orderBySelect').value;


    //  AJAX
    AjaxCallToUpdateClientList(1, pageSizeValue, searchValue, sortByValue, orderByValue);
}

function UpdateClientListViaSortBy(sortBy) {
    // obter cenas da pagina
    document.getElementById('SortById').value = sortBy;

    switch (sortBy) {
        case 'FirstName':
            document.getElementById('tableHeader-' + sortBy).className = "btn btn-dark btn-sm w-100 w-md-auto";
            document.getElementById('tableHeader-LastName').className = "btn btn-outline-dark btn-sm w-100 w-md-auto";
            document.getElementById('tableHeader-Email').className = "btn btn-outline-dark btn-sm w-100 w-md-auto";
            document.getElementById('tableHeader-PhoneNumber').className = "btn btn-outline-dark btn-sm w-100 w-md-auto";
            break;
        case "LastName":
            document.getElementById('tableHeader-' + sortBy).className = "btn btn-dark btn-sm w-100 w-md-auto";
            document.getElementById('tableHeader-FirstName').className = "btn btn-outline-dark btn-sm w-100 w-md-auto"; 
            document.getElementById('tableHeader-Email').className = "btn btn-outline-dark btn-sm w-100 w-md-auto";
            document.getElementById('tableHeader-PhoneNumber').className = "btn btn-outline-dark btn-sm w-100 w-md-auto";
            break;
        case "Email":
            document.getElementById('tableHeader-' + sortBy).className = "btn btn-dark btn-sm w-100 w-md-auto";
            document.getElementById('tableHeader-LastName').className = "btn btn-outline-dark btn-sm w-100 w-md-auto";
            document.getElementById('tableHeader-FirstName').className = "btn btn-outline-dark btn-sm w-100 w-md-auto";
            document.getElementById('tableHeader-PhoneNumber').className = "btn btn-outline-dark btn-sm w-100 w-md-auto";
            break;
        case "PhoneNumber":
            document.getElementById('tableHeader-' + sortBy).className = "btn btn-dark btn-sm w-100 w-md-auto";
            document.getElementById('tableHeader-LastName').className = "btn btn-outline-dark btn-sm w-100 w-md-auto";
            document.getElementById('tableHeader-Email').className = "btn btn-outline-dark btn-sm w-100 w-md-auto";
            document.getElementById('tableHeader-FirstName').className = "btn btn-outline-dark btn-sm w-100 w-md-auto";
            break;

    }

    UpdateClientList();
}

function AjaxCallToUpdateClientList(page,pageSizeValue,searchValue,sortByValue,orderByValue) {

    if (page === undefined || page === null) {
        page = 1;
    }

    $.ajax({
        url: '/Clients/FilterClientList',
        type: "GET",
        data: {
            page : page,
            pageSize: pageSizeValue,
            search: searchValue,
            sortBy: sortByValue,
            orderBy: orderByValue
        },
        success: function (result) {

            $("#ClientesListing").html(result);
        },
        error: function (error) {
            console.error("Error:", error);
        }
    });
}

function ChangePage(next, pageCount)
{
    var currentPageString = document.getElementById('PageId').value;

    var currentPage = 1;
    try
    {
        currentPage = parseInt(currentPageString);
    } catch (e)
    {
        currentPage = 1;
    }



    if (next)
    {
        var isLastPage = (currentPage >= pageCount);
        if (isLastPage)
        {
            return;
        }
        currentPage += 1;

        if (currentPage >= pageCount)
        {
            document.getElementById('nextPageButtonId').className = "page-item disabled";
            document.getElementById('beforePageButtonId').className = "page-item";
        }

    }
    else
    {
        if (currentPage <= 1)
        {
            return;
        }

        if (currentPage - 1 <= 1)
        {
            document.getElementById('beforePageButtonId').className = "page-item disabled";
            document.getElementById('nextPageButtonId').className = "page-item";
        }

        currentPage -= 1;
    }

    document.getElementById('infoPagesSpanId').innerText = "Page " + currentPage + " of " + pageCount;

    document.getElementById('PageId').value = currentPage;

    var searchValue = document.getElementById('SearchInputId').value;
    var sortByValue = document.getElementById('SortById').value;
    var pageSizeValue = document.getElementById('PageSizeId').value;
    var orderByValue = document.querySelector('#orderBySelect').value;

    AjaxCallToUpdateClientList(currentPage, pageSizeValue, searchValue, sortByValue, orderByValue);

}