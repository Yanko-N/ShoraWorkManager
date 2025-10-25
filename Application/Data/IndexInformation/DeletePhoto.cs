using Application.Core;
using Application.Enums;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Data.IndexInformation
{
    public class DeletePhoto
    {
        public class Command : IRequest<Result<string>>
        {
            public string Name { get; set; }    
            public FolderSections SelectedSection { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly ILogger<DeletePhoto> _logger;
            private readonly IHostEnvironment _hostEnvironment;

            public Handler(ILogger<DeletePhoto> logger, IHostEnvironment hostEnvironment)
            {
                _logger = logger;
                _hostEnvironment = hostEnvironment;
            }

            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(request.Name))
                    {
                        return Result<string>.Failure("No photos were provided to save.");
                    }

                    string photosFolderPath = Path.Combine(_hostEnvironment.ContentRootPath, AppConstants.FilePaths.INDEX_PHOTOS);

                    string? fileName = Path.GetFileName(request.Name);

                    if(fileName == null)
                    {
                        return Result<string>.Failure("The provided photo name is invalid.");
                    }

                    //Verifico se existe já esse caminho
                    if (!Directory.Exists(photosFolderPath))
                    {
                        //crio
                        Directory.CreateDirectory(photosFolderPath);
                    }

                    string sectionFolder = request.SelectedSection switch
                    {
                        FolderSections.One => AppConstants.FilePaths.IndexPhotos.ONE,
                        FolderSections.Two => AppConstants.FilePaths.IndexPhotos.TWO,
                        FolderSections.Three => AppConstants.FilePaths.IndexPhotos.THREE,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    photosFolderPath = Path.Combine(photosFolderPath, sectionFolder);

                    //Verifico se existe já esse caminho
                    if (!Directory.Exists(photosFolderPath))
                    {
                        //crio
                        Directory.CreateDirectory(photosFolderPath);
                    }

                    string photoPath = Path.Combine(photosFolderPath, fileName);
                    if (!System.IO.File.Exists(photoPath))
                    {
                        return Result<string>.Failure($"There was some mistake! The photo {request.Name} was not found");
                    }

                    try
                    {
                        System.IO.File.Delete(photoPath);
                    }
                    catch (DirectoryNotFoundException dirEx)
                    {
                        _logger.LogError($"ex happened : {dirEx?.Message ?? dirEx?.InnerException?.Message ?? "Unknow ex"}");
                        return Result<string>.Failure($"There was some mistake! The photo {request.Name} was not found");
                    }
                    catch(UnauthorizedAccessException unauthEx)
                    {
                        _logger.LogError($"ex happened : {unauthEx?.Message ?? unauthEx?.InnerException?.Message ?? "Unknow ex"}");
                        return Result<string>.Failure($"The website doesnt have permissions, something bad happened");
                    }
                    catch(Exception) 
                    {
                        throw;
                    }

                    _logger.LogInformation($"Deleted with success the photo on the section {request.SelectedSection.ToString()} and name {request.Name}.");

                    return Result<string>.Success($"The photo {request.Name} was successfully removed");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ex happened : {ex?.Message ?? ex?.InnerException?.Message ?? "Unknow ex"}");
                    return Result<string>.Failure($"An error occurred while deleting the photo");
                }
            }
        }
    }
}
