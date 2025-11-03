using Application.Contracts.Request;
using Application.Contracts.Response;
using Application.Data.Clientes;
using Application.Data.ConstructionSites;
using Application.Data.MaterialMoviments;
using Application.Data.WorkedHours;
using Application.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Persistence.Models;
using ShoraWorkManager.Models;

namespace ShoraWorkManager.Controllers
{
    public class ConstructionSitesController : Controller
    {
        private readonly IMediator _mediator;

        public ConstructionSitesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index([FromQuery] ListingConstructionSiteRequest request)
        {

            var result = await _mediator.Send(new GetConstructionSitesListing.Query
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
        public async Task<IActionResult> FilterConstructionSiteList(int page, int pageSize, string search, string sortBy, string orderBy)
        {
            ConstructionSiteSortBy sortByResult = ConstructionSiteSortBy.None;
            try
            {
                sortByResult = Enum.Parse<ConstructionSiteSortBy>(sortBy);
            }
            catch
            {
                sortByResult = ConstructionSiteSortBy.None;
            }
            var orderByResult = orderBy == nameof(OrderByEnum.Ascending) ? OrderByEnum.Ascending : OrderByEnum.Descending;

            var result = await _mediator.Send(new GetConstructionSitesListing.Query
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                SortBy = sortByResult,
                OrderBy = orderByResult
            });


            if (!result.IsSuccess)
            {
                return PartialView("ConstructionSitesListingPartial", PaginatedList<ConstructionSite>.Empty());
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


            return PartialView("ConstructionSitesListingPartial", result.Value);

        }

        public async Task<IActionResult> Create()
        {
            var resultGetAllClients = await _mediator.Send(new GetAllClients.Query());

            ViewData["ClientId"] = new SelectList(resultGetAllClients.IsSuccess ? resultGetAllClients.Value : new List<Client>(), "Id", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Latitude,Longitude,IsActive,ClientId")] ConstructionSite constructionSite)
        {
            var resultGetAllClients = await _mediator.Send(new GetAllClients.Query());


           if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new CreateConstructionSite.Command
                {
                    Name = constructionSite.Name,
                    ClientId = constructionSite.ClientId,
                    Description = constructionSite.Description,
                    Latitude = constructionSite.Latitude,
                    Longitude = constructionSite.Longitude,
                });

                if (!result.IsSuccess)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }


                    ViewData["ClientId"] = new SelectList(resultGetAllClients.IsSuccess ? resultGetAllClients.Value : new List<Client>(), "Id", "Email");

                    return View(constructionSite);
                }

                TempData["errorsMessages"] = new List<string>();
                TempData["statusMessages"] = result.IsFailure ? new List<string>() : new List<string>() { $"Sucess creating the constructionSite {constructionSite.Name}" };
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(resultGetAllClients.IsSuccess ? resultGetAllClients.Value : new List<Client>(), "Id", "Email");
            return View(constructionSite);
        }

        public async Task<IActionResult> EditPartial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contructionGetResult = await _mediator.Send(new GetConstructionSite.Query
            {
                Id = (int)id
            });

            if (!contructionGetResult.IsSuccess)
            {
                return NotFound();
            }

            var resultGetAllClients = await _mediator.Send(new GetAllClients.Query());
            ViewData["ClientId"] = new SelectList(resultGetAllClients.IsSuccess ? resultGetAllClients.Value : new List<Client>(), "Id", "Email");


            return PartialView(contructionGetResult.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartial(int id, [Bind("Id,Name,Description,Latitude,Longitude,IsActive,ClientId")] ConstructionSite constructionSite)
        {
            if (id != constructionSite.Id)
            {
                return NotFound();
            }
            var resultGetAllClients = await _mediator.Send(new GetAllClients.Query());

            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new EditConstructionSite.Command
                {
                    Id = constructionSite.Id,
                    Name = constructionSite.Name,
                    ClientId = constructionSite.ClientId,
                    Description = constructionSite.Description,
                    Latitude = constructionSite.Latitude,
                    Longitude = constructionSite.Longitude,
                });

                if (!result.IsSuccess)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    ViewData["ClientId"] = new SelectList(resultGetAllClients.IsSuccess ? resultGetAllClients.Value : new List<Client>(), "Id", "Email");

                    return PartialView(constructionSite);
                }

                TempData["errorsMessages"] = new List<string>();
                TempData["statusMessages"] = result.IsFailure ? new List<string>() : new List<string>() { $"Sucess editing the construction Site {constructionSite.Name}" };

                var index = RedirectToAction(nameof(Index));

                var editedResult = new EditedResult
                {
                    IsSuccess = true,
                    ReturnUrl = Url.Action(nameof(Index))!
                };

                return Json(editedResult);
            }

            ViewData["ClientId"] = new SelectList(resultGetAllClients.IsSuccess ? resultGetAllClients.Value : new List<Client>(), "Id", "Email");
            return PartialView(constructionSite);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resultDelete = await _mediator.Send(new ChangeStateConstructionSite.Command
            {
                Id = (int)id,
                JustFlipState = true
            });

            TempData["errorsMessages"] = resultDelete.IsSuccess ? new List<string>() : resultDelete.Errors.ToList();
            TempData["statusMessages"] = resultDelete.IsFailure ? new List<string>() : new List<string>() { $"{resultDelete.Value}" };

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var constructionSiteResult = await _mediator.Send(new GetConstructionSite.Query()
            {
                Id = (int)id
            });

            if (!constructionSiteResult.IsSuccess)
            {
                return NotFound(constructionSiteResult.ToString());
            }

            var materialsMoviments = await _mediator.Send(new GetMaterialFromConstructionSite.Query()
            {
                ConstructionSiteId = (int)id
            });

            if (!materialsMoviments.IsSuccess)
            {
                return BadRequest(materialsMoviments.ToString());
            }

            var workedHours = await _mediator.Send(new GetWorkedHoursFromConstructionSite.Query()
            {
                ConstructionId = (int)id
            });

            if (!workedHours.IsSuccess)
            {
                return BadRequest(workedHours.ToString());
            }

            ViewBag.statusMessages = TempData.TryGetValue("statusMessages", out var statusMessages) ? statusMessages : null;
            ViewBag.errorsMessages = TempData.TryGetValue("errorsMessages", out var errorMessages) ? errorMessages : null;


            var viewModel = new ConstructionSiteDetailsViewModel()
            {
                ConstructionSite = constructionSiteResult.Value,
                MaterialMovements = materialsMoviments.Value,
                WorkedHours = workedHours.Value
            };

            return View(viewModel);
        }

        public async Task<IActionResult> MaterialsPartial(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var constructionSite = await _mediator.Send(new GetConstructionSite.Query()
            {
                Id = (int)id
            });

            if (!constructionSite.IsSuccess)
            {
                return BadRequest();
            }

            return PartialView(constructionSite.Value);
        }
    }
}
