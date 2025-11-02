using Application.Data.ConstructionSites;
using Application.Data.MaterialMoviments;
using Application.Data.Materials;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Persistence.Data;
using Persistence.Models;
using ShoraWorkManager.Models;

namespace ShoraWorkManager.Controllers
{
    public class MaterialMovementsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;
        public MaterialMovementsController(IMediator mediator,ApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<IActionResult> MaterialMovementRegistry(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var resultConstructionSite = await _mediator.Send(new GetConstructionSite.Query()
            {
                Id = (int)id
            });

            if(!resultConstructionSite.IsSuccess)
            {
                return NotFound();
            }

            ViewData["ConstructionSiteId"] = resultConstructionSite.Value.Id;
            ViewData["ConstructionSiteName"] = resultConstructionSite.Value.Name;

            var listMovement = await _mediator.Send(new GetMaterialFromConstructionSiteList.Query()
            {
                ConstructionSiteId = (int)id
            });

            if (!listMovement.IsSuccess)
            {
                return BadRequest(listMovement.ToString());
            }

            return View(listMovement.Value);
        }


        public async Task<IActionResult> GetTheMaterialMovementIndex(int? id)
        {

            if (id == null)
            {
                return BadRequest();
            }

            var resultConstructionSite = await _mediator.Send(new GetConstructionSite.Query()
            {
                Id = (int)id
            });

            if (!resultConstructionSite.IsSuccess) {

                return BadRequest(resultConstructionSite.ToString());
            }

            var materialsMoviments = await _mediator.Send(new GetMaterialFromConstructionSite.Query()
            {
                ConstructionSiteId = (int)id
            });

            if (!materialsMoviments.IsSuccess)
            {
                return BadRequest(materialsMoviments.ToString());
            }

            var viewModel = new ConstructionSiteDetailsViewModel()
            {
                ConstructionSite = resultConstructionSite.Value,
                MaterialMovements = materialsMoviments.Value
            };

            return PartialView("ListPartial",viewModel);
        }

        public async Task<IActionResult> Create(int? id)
        {

            if (id == null)
            {
                return BadRequest();
            }

            var resultConstructionSite = await _mediator.Send(new GetConstructionSite.Query()
            {
                Id = (int)id
            });

            var resultMaterials = await _mediator.Send(new GetAllMaterials.Query());

            if (!resultConstructionSite.IsSuccess)
            {
                return BadRequest(resultConstructionSite.ToString());
            }

            if (!resultMaterials.IsSuccess)
            {
                return BadRequest(resultMaterials.ToString());
            }

            ViewData["ConstructionSiteId"] = resultConstructionSite.Value.Id;
            ViewData["MaterialId"] = new SelectList(resultMaterials.Value, "Id", "Name");
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id ,[Bind("Id,MaterialId,ConstructionSiteId,Quantity,MovementDate")] MaterialMovement materialMovement)
        {
            if (id == null)
            {
                return BadRequest();
            }

            if(id != materialMovement.ConstructionSiteId)
            {
                return BadRequest();
            }

            var resultConstructionSite = await _mediator.Send(new GetConstructionSite.Query()
            {
                Id = materialMovement.ConstructionSiteId
            });

            var resultMaterials = await _mediator.Send(new GetAllMaterials.Query());

            if (!resultConstructionSite.IsSuccess)
            {
                return BadRequest(resultConstructionSite.ToString());
            }

            if (!resultMaterials.IsSuccess)
            {
                return BadRequest(resultMaterials.ToString());
            }

            if (ModelState.IsValid)
            {
                var resultCreateMaterialMovement = await _mediator.Send(new CreateMaterialMoviments.Command()
                {
                    ConstructionId = materialMovement.ConstructionSiteId,
                    MaterialId = materialMovement.MaterialId,
                    Quantity = materialMovement.Quantity
                });

                if (!resultCreateMaterialMovement.IsSuccess)
                {
                    foreach (var error in resultCreateMaterialMovement.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }

                    ViewData["ConstructionSiteId"] = resultConstructionSite.Value.Id;
                    ViewData["MaterialId"] = new SelectList(resultMaterials.Value, "Id", "Name");
                    return PartialView(materialMovement);   
                }

                var materialsMoviments = await _mediator.Send(new GetMaterialFromConstructionSite.Query()
                {
                    ConstructionSiteId = (int)id
                });

                if (!materialsMoviments.IsSuccess)
                {
                    return BadRequest(materialsMoviments.ToString());
                }

                var viewModel = new ConstructionSiteDetailsViewModel()
                {
                    ConstructionSite = resultConstructionSite.Value,
                    MaterialMovements = materialsMoviments.Value
                };

                return PartialView("ListPartial", viewModel);

            }

            ViewData["ConstructionSiteId"] = resultConstructionSite.Value.Id;
            ViewData["MaterialId"] = new SelectList(resultMaterials.Value, "Id", "Name");
            return PartialView(materialMovement);
        }
    }
}
