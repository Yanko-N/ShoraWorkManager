using Application.Contracts.Request;
using Application.Contracts.Response;
using Application.Core;
using Application.Data.Materials;
using Application.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.Models;

namespace ShoraWorkManager.Controllers
{
    [Authorize(Roles = AppConstants.Roles.ALL_ROLES)]
    public class MaterialsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public MaterialsController(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<IActionResult> Index([FromQuery] ListingMaterialsRequest request)
        {

            var result = await _mediator.Send(new GetMaterialsListing.Query
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
        public async Task<IActionResult> FilterMaterialList(int page, int pageSize, string search, string sortBy, string orderBy)
        {
            MaterialSortBy sortByResult = MaterialSortBy.None;
            try
            {
                sortByResult = Enum.Parse<MaterialSortBy>(sortBy);
            }
            catch
            {
                sortByResult = MaterialSortBy.None;
            }
            var orderByResult = orderBy == nameof(OrderByEnum.Ascending) ? OrderByEnum.Ascending : OrderByEnum.Descending;

            var result = await _mediator.Send(new GetMaterialsListing.Query
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                SortBy = sortByResult,
                OrderBy = orderByResult
            });


            if (!result.IsSuccess)
            {
                return PartialView("MaterialsListingPartial", PaginatedList<Material>.Empty());
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

            return PartialView("MaterialsListingPartial", result.Value);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,AvailableQuantity")] Material material)
        {
            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new CreateMaterial.Command
                {
                    Name = material.Name,
                    Description = material.Description,
                    AvailableQuantity = material.AvailableQuantity
                });

                if (!result.IsSuccess)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return View(material);
                }


                TempData["errorsMessages"] = new List<string>();
                TempData["statusMessages"] = result.IsFailure ? new List<string>() : new List<string>() { $"Sucess create the material {material.Name}" };
                return RedirectToAction(nameof(Index));
            }
            return View(material);
        }

        public async Task<IActionResult> EditPartial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientGetResult = await _mediator.Send(new GetMaterial.Query
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
        public async Task<IActionResult> EditPartial(int id, [Bind("Id,Name,Description,AvailableQuantity")] Material material)
        {
            if (id != material.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new EditMaterial.Command
                {
                    Id = material.Id,
                    Name = material.Name,
                    Description = material.Description,
                    AvailableQuantity = material.AvailableQuantity
                });

                if (!result.IsSuccess)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return PartialView(material);
                }

                TempData["errorsMessages"] = new List<string>();
                TempData["statusMessages"] = result.IsFailure ? new List<string>() : new List<string>() { $"Sucess editing the material {material.Name}" };
                var index = RedirectToAction(nameof(Index));

                var editedResult = new EditedResult
                {
                    IsSuccess = true,
                    ReturnUrl = Url.Action(nameof(Index))!
                };

                return Json(editedResult);
            }
            return PartialView(material);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resultDelete = await _mediator.Send(new DeleteMaterial.Command
            {
                Id = (int)id
            });

            TempData["errorsMessages"] = resultDelete.IsSuccess ? new List<string>() : resultDelete.Errors.ToList();
            TempData["statusMessages"] = resultDelete.IsFailure ? new List<string>() : new List<string>() { $"Sucess deleting the material {resultDelete.Value}" };

            return RedirectToAction(nameof(Index));
        }
    }
}
