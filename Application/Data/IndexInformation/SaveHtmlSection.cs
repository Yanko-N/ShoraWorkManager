using Application.Core;
using Application.Enums;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Data.IndexInformation
{
    public class SaveHtmlSection
    {
        public class Command : IRequest<Result<bool>>
        {
            public string Html { get; set; }
            public FolderSections SelectedSection { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<bool>>
        {
            private readonly ILogger<SaveHtmlSection> _logger;
            private readonly IHostEnvironment _hostEnvironment;

            private int counter = 0;
            
            public Handler(ILogger<SaveHtmlSection> logger,IHostEnvironment hostEnvironment)
            {
                _logger = logger;
                _hostEnvironment = hostEnvironment;
            }

            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if(counter >= AppConstants.General.MAX_TRIES)
                    {
                        return Result<bool>.Failure("Maximum number of retries reached while saving the HTML section.");
                    }

                    counter++;

                    string jsonDataFolder = Path.Combine(_hostEnvironment.ContentRootPath, AppConstants.FilePaths.INDEX_JSON);

                    string fileName = request.SelectedSection switch
                    {
                        FolderSections.One => AppConstants.FilePaths.IndexSections.ONE,
                        FolderSections.Two => AppConstants.FilePaths.IndexSections.TWO,
                        FolderSections.Three => AppConstants.FilePaths.IndexSections.THREE,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    // Verifico se a pasta existe
                    if (!Directory.Exists(jsonDataFolder))
                    {
                        //Não Existe a PASTA AAAA
                        Directory.CreateDirectory(jsonDataFolder);
                    }

                    string jsonDataPath = Path.Combine(jsonDataFolder, fileName);
                    try
                    {
                        //Cria o ficheiro
                        //Não é preciso fechar a Stream pq no fim do Scope do Using é fechada automaticamente
                        using (FileStream createStream = new FileStream(jsonDataPath, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            // Vejo o tamnho do ficheiro e escrevo o ficheiro
                            byte[] jsonDataBytes = System.Text.Encoding.UTF8.GetBytes(request.Html);
                            await createStream.WriteAsync(jsonDataBytes, 0, jsonDataBytes.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error creating a file: {ex.Message}");
                        return await ErrorHandlingForSavingError(jsonDataPath,request.Html,request.SelectedSection,cancellationToken);
                    }

                    _logger.LogInformation($"Saved with success the Section {request.SelectedSection.ToString()}.");
                    return Result<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    return Result<bool>.Failure($"An error occurred while saving the Html");
                }
            }
            public async Task<Result<bool>> ErrorHandlingForSavingError(string jsonDataPath, string html, FolderSections section,CancellationToken cancellationToken)
            {
                System.IO.File.Delete(jsonDataPath);
                return await Handle(new Command()
                {
                    Html = html,
                    SelectedSection = section
                }, cancellationToken);
            }
        }
    }
}
