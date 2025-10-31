using Application.Contracts.Request;
using Application.Contracts.Response;
using Application.Core;
using Application.Data.Workers;
using Application.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Persistence.Models;

namespace ShoraWorkManager.Controllers
{
    [Authorize(Roles = AppConstants.Roles.ALL_ROLES)]
    public class WorkersController : Controller
    {
        private readonly IMediator _mediator;

        public WorkersController( IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index([FromQuery] ListingWorkersRequest request)
        {

            var result = await _mediator.Send(new GetWorkersListing.Query
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
        public async Task<IActionResult> FilterWorkerList(int page, int pageSize, string search, string sortBy, string orderBy)
        {
            WorkerSortBy sortByResult = WorkerSortBy.None;
            try
            {
                sortByResult = Enum.Parse<WorkerSortBy>(sortBy);
            }
            catch
            {
                sortByResult = WorkerSortBy.None;
            }
            var orderByResult = orderBy == nameof(OrderByEnum.Ascending) ? OrderByEnum.Ascending : OrderByEnum.Descending;

            var result = await _mediator.Send(new GetWorkersListing.Query
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                SortBy = sortByResult,
                OrderBy = orderByResult
            });


            if (!result.IsSuccess)
            {
                return PartialView("WorkersListingPartial", PaginatedList<Worker>.Empty());
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


            return PartialView("WorkersListingPartial", result.Value);

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,PricePerHour")] Worker worker)
        {
            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new CreateWorker.Command
                {
                    Name = worker.Name,
                    PricePerHour = worker.PricePerHour
                });

                if (!result.IsSuccess)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return View(worker);
                }


                TempData["errorsMessages"] = new List<string>();
                TempData["statusMessages"] = result.IsFailure ? new List<string>() : new List<string>() { $"Sucess creating the worker {worker.Name}" };
                return RedirectToAction(nameof(Index));
            }
            return View(worker);
        }

        public async Task<IActionResult> EditPartial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientGetResult = await _mediator.Send(new GetWorker.Query
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
        public async Task<IActionResult> EditPartial(int id, [Bind("Id,Name,PricePerHour")] Worker worker)
        {
            if (id != worker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new EditWorker.Command
                {
                    Id = worker.Id,
                    Name = worker.Name,
                    PricerPerHour = worker.PricePerHour
                });

                if (!result.IsSuccess)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return PartialView(worker);
                }

                TempData["errorsMessages"] = new List<string>();
                TempData["statusMessages"] = result.IsFailure ? new List<string>() : new List<string>() { $"Sucess editing the worker {worker.Name}" };

                var index = RedirectToAction(nameof(Index));

                var editedResult = new EditedResult
                {
                    IsSuccess = true,
                    ReturnUrl = Url.Action(nameof(Index))!
                };

                return Json(editedResult);
            }
            return PartialView(worker);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resultDelete = await _mediator.Send(new DeleteWorker.Command
            {
                Id = (int)id
            });

            TempData["errorsMessages"] = resultDelete.IsSuccess ? new List<string>() : resultDelete.Errors.ToList();
            TempData["statusMessages"] = resultDelete.IsFailure ? new List<string>() : new List<string>() { $"Sucess deleting the worker {resultDelete.Value}" };

            return RedirectToAction(nameof(Index));
        }
    }
}
