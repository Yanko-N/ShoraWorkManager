using Application.Classes;
using Application.Core;
using Application.Data.IndexInformation;
using Application.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoraWorkManager.Models;
using System.Diagnostics;

namespace ShoraWorkManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Views

        public async Task<IActionResult> Index()
        {
            IndexJson indexJsonModel = await GetIndexJson();
            IndexPhotos indexPhotosModel = await GetIndexPhotos();

            IndexViewModel indexViewModel = new IndexViewModel()
            {
                SectionOneHtmlText = indexJsonModel.SectionOneHtmlText,
                SectionOnePhotos = indexPhotosModel.PhotosOne,
                SectionTwoHtmlText = indexJsonModel.SectionTwoHtmlText,
                SectionTwoPhotos = indexPhotosModel.PhotosTwo,
                SectionThreeHtmlText = indexJsonModel.SectionThreeHtmlText,
                SectionThreePhotos = indexPhotosModel.PhotosThree
            };

            return View(indexViewModel);
        }

        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        public async Task<IActionResult> ControlIndex()
        {
            ViewBag.statusMessages = TempData.TryGetValue("statusMessages", out var statusMessages) ? statusMessages : null;
            ViewBag.errorsMessages = TempData.TryGetValue("errorsMessages", out var errorMessages) ? errorMessages : null;

            IndexJson indexJsonModel = await GetIndexJson();
            IndexPhotos indexPhotosModel = await GetIndexPhotos();

            IndexEditModel indexEditModel = new IndexEditModel()
            {
                SectionOneHtmlText = indexJsonModel.SectionOneHtmlText,
                SectionOnePhotos = indexPhotosModel.PhotosOne,
                SectionTwoHtmlText = indexJsonModel.SectionTwoHtmlText,
                SectionTwoPhotos = indexPhotosModel.PhotosTwo,
                SectionThreeHtmlText = indexJsonModel.SectionThreeHtmlText,
                SectionThreePhotos = indexPhotosModel.PhotosThree
            };

            return View(indexEditModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion

        #region Form Post's

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        public async Task<IActionResult> UpdateSectionOneHtml([FromForm(Name = "HtmlText")] string text)
        {
            var result = await SaveSomeDataFromIndexJsonAsync(text, FolderSections.One);

            List<string> errorsMessages = result.errors;
            List<string> statusMessages = result.success;

            TempData["statusMessages"] = statusMessages;
            TempData["errorsMessages"] = errorsMessages;

            return RedirectToAction(nameof(ControlIndex));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        public async Task<IActionResult> UpdateSectionTwoHtml([FromForm(Name = "HtmlText")] string text)
        {
            var result = await SaveSomeDataFromIndexJsonAsync(text, FolderSections.Two);

            List<string> errorsMessages = result.errors;
            List<string> statusMessages = result.success;

            TempData["statusMessages"] = statusMessages;
            TempData["errorsMessages"] = errorsMessages;

            return RedirectToAction(nameof(ControlIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        public async Task<IActionResult> UpdateSectionThreeHtml([FromForm(Name = "HtmlText")] string text)
        {
            var result = await SaveSomeDataFromIndexJsonAsync(text, FolderSections.Three);

            List<string> errorsMessages = result.errors;
            List<string> statusMessages = result.success;

            TempData["statusMessages"] = statusMessages;
            TempData["errorsMessages"] = errorsMessages;

            return RedirectToAction(nameof(ControlIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        public async Task<IActionResult> AddPhotoSectionOne([FromForm(Name = "photos")] List<IFormFile> Photos)
        {
            List<string> errorsMessages = new List<string>();
            List<string> statusMessages = new List<string>();

            var result = await AddPhotos(Photos, FolderSections.One);

            if(result.IsSuccess)
            {
                statusMessages = result.Value.success;
                errorsMessages = result.Value.errors;
            }
            else
            {
                errorsMessages.AddRange(result.Errors);
            }

            TempData["statusMessages"] = statusMessages;
            TempData["errorsMessages"] = errorsMessages;

            return RedirectToAction(nameof(ControlIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        public async Task<IActionResult> AddPhotoSectionTwo([FromForm(Name = "photos")] List<IFormFile> Photos)
        {
            List<string> errorsMessages = new List<string>();
            List<string> statusMessages = new List<string>();

            var result = await AddPhotos(Photos, FolderSections.Two);

            if (result.IsSuccess)
            {
                statusMessages = result.Value.success;
                errorsMessages = result.Value.errors;
            }
            else
            {
                errorsMessages.AddRange(result.Errors);
            }

            TempData["statusMessages"] = statusMessages;
            TempData["errorsMessages"] = errorsMessages;

            return RedirectToAction(nameof(ControlIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        public async Task<IActionResult> AddPhotoSectionThree([FromForm(Name = "photos")] List<IFormFile> Photos)
        {
            List<string> errorsMessages = new List<string>();
            List<string> statusMessages = new List<string>();

            var result = await AddPhotos(Photos, FolderSections.Three);

            if (result.IsSuccess)
            {
                statusMessages = result.Value.success;
                errorsMessages = result.Value.errors;
            }
            else
            {
                errorsMessages.AddRange(result.Errors);
            }

            TempData["statusMessages"] = statusMessages;
            TempData["errorsMessages"] = errorsMessages;

            return RedirectToAction(nameof(ControlIndex));
        }

        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhotoSectionOne(string name)
        {

            var result = await DeletePhotoAsync(name, FolderSections.One);

            TempData["statusMessages"] = result.success;
            TempData["errorsMessages"] = result.errors;

            return RedirectToAction(nameof(ControlIndex));
        }

        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhotoSectionTwo(string name)
        {

            var result = await DeletePhotoAsync(name, FolderSections.Two);

            TempData["statusMessages"] = result.success;
            TempData["errorsMessages"] = result.errors;

            return RedirectToAction(nameof(ControlIndex));
        }

        [Authorize(Roles = AppConstants.Roles.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhotoSectionThree(string name)
        {
            var result = await DeletePhotoAsync(name, FolderSections.Three);

            TempData["statusMessages"] = result.success;
            TempData["errorsMessages"] = result.errors;

            return RedirectToAction(nameof(ControlIndex));
        }

        #endregion

        #region Helper methods
        async Task<(List<string> errors, List<string> success)> DeletePhotoAsync(string name,FolderSections section)
        {
            var result = await _mediator.Send(new DeletePhoto.Command()
            {
                Name = name,
                SelectedSection = section,
            });

            List<string> errorsMessages = new List<string>();
            List<string> statusMessages = new List<string>();

            if (result.IsSuccess)
            {
                statusMessages.Add(result.Value);
            }
            else
            {
                errorsMessages.AddRange(result.Errors);
            }

            return (errorsMessages, statusMessages);
        }
        async Task<(List<string> errors, List<string> success)> SaveSomeDataFromIndexJsonAsync(string content,FolderSections sections)
        {
            List<string> errorsMessages = new List<string>();
            List<string> statusMessages = new List<string>();

            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    errorsMessages.Add("IndexJson model is null.");
                }

                var result = await _mediator.Send(new Application.Data.IndexInformation.SaveHtmlSection.Command
                {
                    Html = content,
                    SelectedSection = sections
                });

                if(result.IsSuccess)
                {
                    statusMessages.Add("Section updated successfully.");
                }
                else
                {
                    errorsMessages.AddRange(result.Errors);
                }
            }
            catch (Exception ex)
            {
                errorsMessages.Add("Some error ocurred");
            }

            return (errorsMessages, statusMessages);
        }
        async Task<Result<(List<string> errors, List<string> success)>> AddPhotos(List<IFormFile> Photos,FolderSections sections)
        {
            if (Photos == null || Photos.Count == 0)
            {
                return Result<(List<string> errors, List<string> success)>.Failure("No photos were provided to save.");
            }

            try
            {
                return await _mediator.Send(new Application.Data.IndexInformation.SavePhotos.Command
                {
                    Photos = Photos,
                    SelectedSection = sections
                });
            }
            catch (Exception ex)
            {
                return Result<(List<string> errors, List<string> success)>.Failure("Some error ocurred");
            }
        }
        async Task<IndexJson> GetIndexJson()
        {
            var resultGetIndex = await _mediator.Send(new Application.Data.IndexInformation.GetIndexJson.Command());

            IndexJson indexJsonModel = new IndexJson();

            if (resultGetIndex.IsSuccess && resultGetIndex.Value != null)
            {
                indexJsonModel = resultGetIndex.Value;
            }

            return indexJsonModel;
        }
        async Task<IndexPhotos> GetIndexPhotos()
        {
            var resultGetIndexPhotos = await _mediator.Send(new Application.Data.IndexInformation.GetIndexPhotos.Command());
            IndexPhotos indexPhotosModel = new IndexPhotos();
            if (resultGetIndexPhotos.IsSuccess && resultGetIndexPhotos.Value != null)
            {
                indexPhotosModel = resultGetIndexPhotos.Value;
            }

            return indexPhotosModel;
        }

        #endregion

    }
}
