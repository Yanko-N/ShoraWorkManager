using Application.Classes;
using Application.Core;
using Application.Enums;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Data.IndexInformation
{
    public class GetIndexPhotos
    {
        public class Command : IRequest<Result<IndexPhotos>>
        {

        }

        public class Handler : IRequestHandler<Command, Result<IndexPhotos>>
        {
            private readonly ILogger<GetIndexPhotos> _logger;
            private readonly IHostEnvironment _hostEnvironment;

            private int counter = 0;

            public Handler(ILogger<GetIndexPhotos> logger, IHostEnvironment hostEnvironment)
            {
                _logger = logger;
                _hostEnvironment = hostEnvironment;
            }

            public async Task<Result<IndexPhotos>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (counter >= AppConstants.General.MAX_TRIES)
                    {
                        return Result<IndexPhotos>.Failure("Maximum number of retries reached while getting the index photos.");
                    }

                    counter++;

                    string jsonPhotosFolder = Path.Combine(_hostEnvironment.ContentRootPath, AppConstants.FilePaths.INDEX_PHOTOS);

                    // Verifico se a pasta existe
                    if (!Directory.Exists(jsonPhotosFolder))
                    {
                        //Não Existe a PASTA AAAA
                        Directory.CreateDirectory(jsonPhotosFolder);
                    }

                    FolderSections[] sections = (FolderSections[])Enum.GetValues(typeof(FolderSections));

                    IndexPhotos indexPhotos = new IndexPhotos();

                    foreach (var section in sections)
                    {
                        string sectionFolder = section switch
                        {
                            FolderSections.One => AppConstants.FilePaths.IndexPhotos.ONE,
                            FolderSections.Two => AppConstants.FilePaths.IndexPhotos.TWO,
                            FolderSections.Three => AppConstants.FilePaths.IndexPhotos.THREE,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        var sectionFolderPath = Path.Combine(jsonPhotosFolder, sectionFolder);

                        // Verifico se a pasta existe
                        if (!Directory.Exists(sectionFolderPath))
                        {
                            //Não Existe a PASTA AAAA
                            Directory.CreateDirectory(sectionFolderPath);
                        }

                        // Get dos nomes da pasta
                        List<string?> allPhotos = Directory.GetFiles(sectionFolderPath)
                                                       .Select(Path.GetFileName)
                                                       .ToList() ?? new List<string?>();

                        List<string> photoUrls = allPhotos
                            .Where(photo => !string.IsNullOrWhiteSpace(photo))
                            .ToList();

                        List<string> fullPhotoUrls = photoUrls
                            .Select(photo => Path.Combine(AppConstants.FilePaths.INDEX_PHOTOS_URL,sectionFolder,photo))
                            .ToList();

                        switch (section)
                        {
                            case FolderSections.One:
                                indexPhotos.PhotosOne = fullPhotoUrls;
                                break;
                            case FolderSections.Two:
                                indexPhotos.PhotosTwo = fullPhotoUrls;
                                break;
                            case FolderSections.Three:
                                indexPhotos.PhotosThree = fullPhotoUrls;
                                break;
                        }
                    }

                    _logger.LogInformation("Index Photos was retreived with success");
                    return Result<IndexPhotos>.Success(indexPhotos);
                }
                catch (Exception ex)
                {
                    return Result<IndexPhotos>.Failure($"An error occurred while getting the index photos");
                }
            }
        }
    }
}
