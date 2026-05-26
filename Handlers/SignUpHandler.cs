using MediatR;
using Microsoft.EntityFrameworkCore;
using VehicleMangement.Commands;
using VehicleMangement.Data;
using VehicleMangement.Events;
using VehicleMangement.EventStore;
using VehicleMangement.Models;
using BC = BCrypt.Net.BCrypt;
namespace VehicleMangement.Handlers
{
    public class SignUpHandler : IRequestHandler<SignUpCommand, string>
    {
        private readonly UserDbContext _context;
        private readonly EventRepository _eventRepository;

        public SignUpHandler(UserDbContext context, EventRepository eventRepository)
        {
            _context = context;
            _eventRepository = eventRepository;
        }
        public async Task<string> Handle(SignUpCommand command,CancellationToken cancellationToken)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(command.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$"))
            {
                throw new ArgumentException("InvalidEmail");
            }
            var existsinguser= await _context.Users.AnyAsync(u=>u.Email==command.Email,cancellationToken);
            if (existsinguser)
            {
                throw new InvalidOperationException("Eamil Already Exists");
            }
            var hashpassword = BC.HashPassword(command.Password);
            var newuser=new User
            {
                Name = command.Name,
                Email = command.Email,
                PasswordHash = hashpassword
            };
            _context.Users.Add(newuser);
            await _context.SaveChangesAsync();
            var evt=new UserCreatedEvent
            {
                UserId = newuser.UserId,
                Name = newuser.Name,
                Email = newuser.Email
            };
            await _eventRepository.SaveAsync(newuser.UserId, evt);
            return "User Created Successfully";
        }
      
    }
}
