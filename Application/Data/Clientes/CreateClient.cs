using Application.Core;
using Domain.Exceptions;
using Domain.Validators;
using Domain.Validators.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.Clientes
{
    public class CreateClient
    {
        public class Command : IRequest<Result<Client>>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Client>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<CreateClient> _logger;
            public Handler(ApplicationDbContext context, ILogger<CreateClient> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<Client>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    IDataValidator<ClientValidator.ClientInput> clientValidator = new ClientValidator();

                    try
                    {
                        clientValidator.Validate(new ClientValidator.ClientInput
                        {
                            FirstName = request.FirstName,
                            LastName = request.LastName,
                            Email = request.Email,
                            PhoneNumber = request.Phone
                        });
                    }
                    catch (CustomValidationException ex)
                    {
                        return Result<Client>.Failure(ex.ErrorsMessage);
                    }

                    var emailExists = _context.Clients.Any(c => c.Email == request.Email);

                    if(emailExists)
                    {
                        _logger.LogWarning("Failed creating a Client because Email is already in use.");
                        return Result<Client>.Failure("Email is already in use.");
                    }

                    var phoneExists = _context.Clients.Any(c => c.PhoneNumber == request.Phone);

                    if(phoneExists)
                    {
                        _logger.LogWarning("Failed creating a Client because Phone number is already in use.");
                        return Result<Client>.Failure("Phone number is already in use.");
                    }

                    var client = new Client
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                        PhoneNumber = request.Phone
                    };

                    await _context.Clients.AddAsync(client,cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                    if (!result)
                    {
                        _logger.LogWarning("Failed to create client.");
                        return Result<Client>.Failure("Failed to create client");
                    }

                    _logger.LogInformation("Client created successfully.");
                    return Result<Client>.Success(client);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating client");
                    return Result<Client>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
