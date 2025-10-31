using Application.Core;
using Domain.Exceptions;
using Domain.Validators;
using Domain.Validators.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.Clientes
{
    public class EditClient
    {
        public class Command : IRequest<Result<Client>>
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Client>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<EditClient> _logger;
            public Handler(ApplicationDbContext context, ILogger<EditClient> logger)
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

                    var clientWithEmail = await _context.Clients
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Email == request.Email,cancellationToken);


                    if (clientWithEmail != null && clientWithEmail.Id != request.Id)
                    {
                        _logger.LogWarning("Failed editing a Client because Email is already in use.");
                        return Result<Client>.Failure("Email is already in use.");
                    }

                    var clientWithPhone = await _context.Clients
                       .AsNoTracking()
                       .FirstOrDefaultAsync(c => c.PhoneNumber == request.Phone, cancellationToken);


                    if (clientWithPhone != null && clientWithPhone.Id != request.Id)
                    {
                        _logger.LogWarning("Failed editing a Client because Phone number is already in use.");
                        return Result<Client>.Failure("Phone number is already in use.");
                    }

                    var clientToUpdate = await _context.Clients
                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                    if (clientToUpdate == null)
                    {
                        _logger.LogWarning("Client not found.");
                        return Result<Client>.Failure("Client not found.");
                    }

                    clientToUpdate.FirstName = request.FirstName;
                    clientToUpdate.LastName = request.LastName;
                    clientToUpdate.Email = request.Email;
                    clientToUpdate.PhoneNumber = request.Phone;

                    _context.Clients.Update(clientToUpdate);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                    if (!result)
                    {
                        _logger.LogWarning("Failed to edit client.");
                        return Result<Client>.Failure("Failed to edit client");
                    }

                    _logger.LogInformation("Client edited successfully.");
                    return Result<Client>.Success(clientToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    _logger.LogError("Concurrency error editing client");
                    return Result<Client>.Failure("Concurrency error occured, try again.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error editing client");
                    return Result<Client>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
