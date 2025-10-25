using Application.Core;
using Application.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Data.IndexInformation
{
    public class SavePhotos
    {
        public class Command : IRequest<Result<(List<string> errors,List<string> success)>>
        {
            public List<IFormFile> Photos { get; set; } = new List<IFormFile>();
            public FolderSections SelectedSection { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<(List<string> errors, List<string> success)>>
        {
            private readonly ILogger<SavePhotos> _logger;
            private readonly IHostEnvironment _hostEnvironment;

            public Handler(ILogger<SavePhotos> logger, IHostEnvironment hostEnvironment)
            {
                _logger = logger;
                _hostEnvironment = hostEnvironment;
            }

            public async Task<Result<(List<string> errors, List<string> success)>> Handle(Command request, CancellationToken cancellationToken)
            {
                List<string> errorsMessages = new List<string>();
                List<string> statusMessages = new List<string>();

                try
                {
                    if(request.Photos == null || request.Photos.Count == 0)
                    {
                        return Result<(List<string> errors, List<string> success)>.Failure("No photos were provided to save.");
                    }

                    string destination = Path.Combine(_hostEnvironment.ContentRootPath, AppConstants.FilePaths.INDEX_PHOTOS);

                    //Verifico se existe já esse caminho
                    if (!Directory.Exists(destination))
                    {
                        //crio
                        Directory.CreateDirectory(destination);
                    }

                    string photoFolderPath = request.SelectedSection switch
                    {
                        FolderSections.One => AppConstants.FilePaths.IndexPhotos.ONE,
                        FolderSections.Two => AppConstants.FilePaths.IndexPhotos.TWO,
                        FolderSections.Three => AppConstants.FilePaths.IndexPhotos.THREE,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    destination = Path.Combine(destination,photoFolderPath);

                    //Verifico se existe já esse caminho
                    if (!Directory.Exists(destination))
                    {
                        //crio
                        Directory.CreateDirectory(destination);
                    }

                    //Passo por todas as Fotos Recebidas
                    foreach (var photoFile in request.Photos)
                    {
                        try
                        {
                            string extension = System.IO.Path.GetExtension(photoFile.FileName);

                            // Verifico se está numa das extensões permitidas
                            if (!(extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                                extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)))
                            {
                                errorsMessages.Add($"The photo {photoFile.FileName} is from a invalid data type png, jpeg, jpg");
                                continue;
                            }

                            //Destino Final
                            var finalDestination = Path.Combine(destination, photoFile.FileName);

                            //Crio uma Stream nesse caminho,coloco-o lá e fecho
                            using (FileStream fs = new FileStream(finalDestination, FileMode.Create))
                            {
                                photoFile.CopyTo(fs);
                            }

                            statusMessages.Add($"The photo {photoFile.FileName} was submitted with success");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Something went wrong when saving the file {photoFile.FileName} | ex {ex?.Message ?? ex?.InnerException?.Message ?? string.Empty}");
                            errorsMessages.Add($"Something went wrong when saving the file {photoFile.FileName}");
                            continue;
                        }
                        
                    }

                    _logger.LogInformation($"Saved with success the photos with {request.SelectedSection.ToString()}.");
                    return Result<(List<string> errors, List<string> success)>.Success((errorsMessages,statusMessages));
                }
                catch (Exception ex)
                {
                    return Result<(List<string> errors, List<string> success)>.Failure($"An error occurred while saving the photos");
                }
            }
        }
    }
}
