using Application.Classes;
using Application.Core;
using Application.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Data.IndexInformation
{
    public class GetIndexJson
    {
        public class Command : IRequest<Result<IndexJson>>
        {

        }

        public class Handler : IRequestHandler<Command, Result<IndexJson>>
        {
            private readonly ILogger<GetIndexJson> _logger;
            private readonly IHostEnvironment _hostEnvironment;

            private int counter = 0;

            public Handler(ILogger<GetIndexJson> logger, IHostEnvironment hostEnvironment)
            {
                _logger = logger;
                _hostEnvironment = hostEnvironment;
            }

            public async Task<Result<IndexJson>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (counter >= AppConstants.General.MAX_TRIES)
                    {
                        return Result<IndexJson>.Failure("Maximum number of retries reached while getting the index json.");
                    }

                    counter++;

                    string jsonDataFolder = Path.Combine(_hostEnvironment.ContentRootPath, AppConstants.FilePaths.INDEX_JSON);

                    FolderSections[] sections = (FolderSections[])Enum.GetValues(typeof(FolderSections));

                    IndexJson indexJson = new IndexJson();

                    foreach (var section in sections)
                    {
                        string fileName = section switch
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
                                byte[] jsonDataBytes = System.Text.Encoding.UTF8.GetBytes(string.Empty);
                                await createStream.WriteAsync(jsonDataBytes, 0, jsonDataBytes.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Erro a criar o ficheiro: {ex.Message}");
                            return Result<IndexJson>.Failure("An error occurred while creating the file.");
                        }

                        switch (section)
                        {
                            case FolderSections.One:
                                indexJson.SectionOneHtmlText = await System.IO.File.ReadAllTextAsync(jsonDataPath);
                                break;
                            case FolderSections.Two:
                                indexJson.SectionTwoHtmlText = await System.IO.File.ReadAllTextAsync(jsonDataPath);
                                break;
                            case FolderSections.Three:
                                indexJson.SectionThreeHtmlText = await System.IO.File.ReadAllTextAsync(jsonDataPath);
                                break;
                        }
                    }

                    _logger.LogInformation("Index Json was retreived with success");
                    return Result<IndexJson>.Success(indexJson);
                }
                catch (Exception ex)
                {
                    return Result<IndexJson>.Failure($"An error occurred while getting the index information");
                }
            }
        }
    }
}
