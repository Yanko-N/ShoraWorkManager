using Application.Contracts.Request;
using Application.Data.ConstructionSites;
using Application.Data.Payments;
using Application.Data.Workers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ShoraWorkManager.Controllers
{
    public class PaymentsController : Controller
    {

        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(int? constructionId,int? workerId)
        {
            if (constructionId == null || workerId == null)
            { 
                return BadRequest();
            }

            var resultConstructionId = await _mediator.Send(new GetConstructionSite.Query()
            {
                Id = (int)constructionId
            });

            var resultWorkerId = await _mediator.Send(new GetWorker.Query()
            {
                Id = (int)workerId
            });

            if (resultConstructionId.IsFailure || resultWorkerId.IsFailure)
            {
                return BadRequest();
            }

            var payments = await _mediator.Send(new GetPaymentsByContructionAndWorker.Query()
            {
                WorkerId = (int)workerId,
                ConstructionId = (int)constructionId
            });

            if (payments.IsFailure)
            {
                return BadRequest();
            }

            ViewBag.ConstructionSiteName = resultConstructionId.Value.Name;
            ViewBag.ConstructionSiteId = resultConstructionId.Value.Id;
            ViewBag.WorkerName = resultWorkerId.Value.Name;
            ViewBag.statusMessages = TempData.TryGetValue("statusMessages", out var statusMessages) ? statusMessages : null;
            ViewBag.errorsMessages = TempData.TryGetValue("errorsMessages", out var errorMessages) ? errorMessages : null;

            return View(payments.Value);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayWorker(PaymentRequest request)
        {

            try
            {
                var resultPayment = await _mediator.Send(new CreatePayment.Command()
                {
                    ConstructionId = request.ConstructionId,
                    WorkerId = request.WorkerId,
                });


                TempData["errorsMessages"] = resultPayment.IsFailure ? resultPayment.Errors.ToList() : new List<string>();
                TempData["statusMessages"] = resultPayment.IsFailure ? new List<string>() : new List<string>() { $"Sucess creating the payment" };

                if (!resultPayment.IsSuccess)
                {
                    return RedirectToAction(
                        nameof(ConstructionSitesController.Details),
                        "ConstructionSites",
                        new { id = request.ConstructionId}
                    );
                }

                return RedirectToAction(
                    nameof(Index),
                    new { constructionId = request.ConstructionId, workerId = request.WorkerId }
                );
            }
            catch
            {
                TempData["errorsMessages"] = new List<string>() { $"Error when creating new Payment"};
                TempData["statusMessages"] = new List<string>();

                return RedirectToAction(
                    nameof(ConstructionSitesController.Details),
                    "ConstructionSites",
                    new { id = request.ConstructionId }
                );
            }
        }
    }
}
