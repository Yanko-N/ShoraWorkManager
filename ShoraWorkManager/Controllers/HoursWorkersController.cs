using Application.Core;
using Application.Data.ConstructionSites;
using Application.Data.WorkedHours;
using Application.Data.Workers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Persistence.Models;
using ShoraWorkManager.Models;

namespace ShoraWorkManager.Controllers
{
    [Authorize(Roles = AppConstants.Roles.ALL_ROLES)]
    public class HoursWorkersController : Controller
    {
        private readonly IMediator _mediator;
        public HoursWorkersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> GetTheWorkedHoursIndex(int? id)
        {

            if (id == null)
            {
                return BadRequest();
            }

            var resultConstructionSite = await _mediator.Send(new GetConstructionSite.Query()
            {
                Id = (int)id
            });

            if (!resultConstructionSite.IsSuccess)
            {

                return BadRequest(resultConstructionSite.ToString());
            }

            var hoursWorked = await _mediator.Send(new GetWorkedHoursFromConstructionSite.Query()
            {
                ConstructionId = (int)id
            });

            if (!hoursWorked.IsSuccess)
            {
                return BadRequest(hoursWorked.ToString());
            }

            var viewModel = new ConstructionSiteDetailsViewModel()
            {
                ConstructionSite = resultConstructionSite.Value,
                WorkedHours = hoursWorked.Value
            };

            return PartialView("ListPartial", viewModel);
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

            var resultWorkers = await _mediator.Send(new GetAllWorkers.Query());

            if (!resultConstructionSite.IsSuccess)
            {
                return BadRequest(resultConstructionSite.ToString());
            }

            if (!resultWorkers.IsSuccess)
            {
                return BadRequest(resultWorkers.ToString());
            }

            ViewData["ConstructionSiteId"] = resultConstructionSite.Value.Id;
            ViewData["WorkerId"] = new SelectList(resultWorkers.Value, "Id", "Name");
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id, [Bind("Id,ConstructionSiteId,WorkerId,WorkedHours,RegisteredAt,WasPayed")] ContructionSiteWorkedHoursWorker contructionSiteWorkedHoursWorker)
        {
            if (id == null)
            {
                return BadRequest();
            }

            if (id != contructionSiteWorkedHoursWorker.ConstructionSiteId)
            {
                return BadRequest();
            }

            var resultConstructionSite = await _mediator.Send(new GetConstructionSite.Query()
            {
                Id = contructionSiteWorkedHoursWorker.ConstructionSiteId
            });

            var resultWorkers = await _mediator.Send(new GetAllWorkers.Query());

            if (!resultConstructionSite.IsSuccess)
            {
                return BadRequest(resultConstructionSite.ToString());
            }

            if (!resultWorkers.IsSuccess)
            {
                return BadRequest(resultWorkers.ToString());
            }

            if (ModelState.IsValid)
            {
                var resultCreateHoursWorker = await _mediator.Send(new CreateWorkedHours.Command()
                {
                    ConstructionSiteId = resultConstructionSite.Value.Id,
                    WorkedHours = contructionSiteWorkedHoursWorker.WorkedHours,
                    WorkerId = contructionSiteWorkedHoursWorker.WorkerId
                });

                if (!resultCreateHoursWorker.IsSuccess)
                {
                    foreach (var error in resultCreateHoursWorker.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }

                    ViewData["ConstructionSiteId"] = resultConstructionSite.Value.Id;
                    ViewData["WorkerId"] = new SelectList(resultWorkers.Value, "Id", "Name", contructionSiteWorkedHoursWorker.WorkerId);
                    return PartialView(contructionSiteWorkedHoursWorker);
                }

                var workedHoursResult = await _mediator.Send(new GetWorkedHoursFromConstructionSite.Query()
                {
                    ConstructionId = (int)id
                });

                if (!workedHoursResult.IsSuccess)
                {
                    return BadRequest(workedHoursResult.ToString());
                }

                var viewModel = new ConstructionSiteDetailsViewModel()
                {
                    ConstructionSite = resultConstructionSite.Value,
                    WorkedHours = workedHoursResult.Value
                };

                return PartialView("ListPartial", viewModel);
            }

            ViewData["ConstructionSiteId"] = resultConstructionSite.Value.Id;
            ViewData["WorkerId"] = new SelectList(resultWorkers.Value, "Id", "Name", contructionSiteWorkedHoursWorker.WorkerId);
            return PartialView(contructionSiteWorkedHoursWorker);
        }
       
    }
}
