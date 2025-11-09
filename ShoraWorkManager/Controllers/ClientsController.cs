using Application.Contracts.Request;
using Application.Contracts.Response;
using Application.Core;
using Application.Data.Clientes;
using Application.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Persistence.Models;

namespace ShoraWorkManager.Controllers
{
    [Authorize(Roles = AppConstants.Roles.ALL_ROLES)]
    public class ClientsController : Controller
    {
        private readonly IMediator _mediator;

        public ClientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index([FromQuery] ListingClientsRequest request)
        {
           
            var result = await _mediator.Send(new GetClientsListing.Query
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Search = request.Search,
                SortBy = request.SortBy,
                OrderBy = request.OrderBy
            });

            if (!result.IsSuccess)
            {
                return BadRequest(result.ToString());
            }

            // Repassa os parâmetros atuais para a View (para manter os filtros )
            ViewBag.CurrentSearch = request.Search ?? string.Empty;
            ViewBag.CurrentSortBy = request.SortBy;
            ViewBag.CurrentOrderBy = request.OrderBy;
            ViewBag.PageSize = request.PageSize;
            ViewBag.CurrentPage = request.Page;

            ViewBag.OrderByList = new SelectListItem[]
            {
                new SelectListItem { Value = nameof(OrderByEnum.Ascending), Text = "Ascending" },
                new SelectListItem { Value = nameof(OrderByEnum.Descending), Text = "Descending" }
            };

            ViewBag.statusMessages = TempData.TryGetValue("statusMessages", out var statusMessages) ? statusMessages : null;
            ViewBag.errorsMessages = TempData.TryGetValue("errorsMessages", out var errorMessages) ? errorMessages : null;


            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> FilterClientList(int page,int pageSize,string search,string sortBy,string orderBy)
        {
            ClientSortBy sortByResult = ClientSortBy.None;
            try
            {
                sortByResult = Enum.Parse<ClientSortBy>(sortBy);
            }
            catch
            {
                sortByResult = ClientSortBy.None;
            }
            var orderByResult = orderBy == nameof(OrderByEnum.Ascending) ? OrderByEnum.Ascending : OrderByEnum.Descending;

            var result = await _mediator.Send(new GetClientsListing.Query
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                SortBy = sortByResult,
                OrderBy = orderByResult
            });


            if (!result.IsSuccess)
            {
                return PartialView("ClientsListingPartial",  PaginatedList<Client>.Empty());
            }

            // Repassa os parâmetros atuais para a View (para manter os filtros )
            ViewBag.CurrentSearch = search ?? string.Empty;
            ViewBag.CurrentSortBy = sortByResult;
            ViewBag.CurrentOrderBy = orderByResult;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = 1;

            ViewBag.OrderByList = new SelectListItem[]
            {
                new SelectListItem { Value = nameof(OrderByEnum.Ascending), Text = "Ascending" },
                new SelectListItem { Value = nameof(OrderByEnum.Descending), Text = "Descending" }
            };


            return PartialView("ClientsListingPartial", result.Value);  

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhoneNumber")] Client client)
        {
            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new CreateClient.Command
                {
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Email = client.Email,
                    Phone = client.PhoneNumber
                });

                if (!result.IsSuccess)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return View(client);
                }


                TempData["errorsMessages"] = new List<string>();
                TempData["statusMessages"] = result.IsFailure ? new List<string>() : new List<string>() {$"Sucess creating the client {client.FirstName} {client.LastName}" } ;
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        public async Task<IActionResult> EditPartial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientGetResult = await _mediator.Send(new GetClient.Query 
            { 
                Id = (int)id 
            });

            if (!clientGetResult.IsSuccess)
            {
                return NotFound();
            }

            return PartialView(clientGetResult.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartial(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new EditClient.Command
                {
                    Id = client.Id,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Email = client.Email,
                    Phone = client.PhoneNumber
                });

                if (!result.IsSuccess)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return PartialView(client);
                }

                TempData["errorsMessages"] =  new List<string>();
                TempData["statusMessages"] = result.IsFailure ? new List<string>() : new List<string>() {$"Sucess editing the client {client.FirstName} {client.LastName}" } ;

                var index = RedirectToAction(nameof(Index));

                var editedResult = new EditedResult
                {
                    IsSuccess = true,
                    ReturnUrl = Url.Action(nameof(Index))!
                };

                return Json(editedResult);
            }
            return PartialView(client);
        }
 
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resultDelete = await _mediator.Send(new DeleteClient.Command
            {
                Id = (int)id
            });

            TempData["errorsMessages"] = resultDelete.IsSuccess ? new List<string>() : resultDelete.Errors.ToList();
            TempData["statusMessages"] = resultDelete.IsFailure ? new List<string>() : new List<string>() {$"Sucess deleting the client {resultDelete.Value}" } ;

            return RedirectToAction(nameof(Index));
        }
    }
}
