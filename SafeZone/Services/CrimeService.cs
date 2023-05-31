using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SafeZone.Data;
using SafeZone.DTO;
using SafeZone.Hubs;
using SafeZone.Models;
using SafeZone.Repositories;

namespace SafeZone.Services
{
    public class CrimeService : ICrimeRepository
    {
        private readonly DataContext db;
        private readonly IUserRepository _user;
        private readonly IHubContext<CrimeHub> hubContext;

        public CrimeService(DataContext db, IHubContext<CrimeHub> hubContext, IUserRepository user)
        {
            this.db = db;
            this.hubContext = hubContext;
            _user = user;
        }

        public Task CloseCase(Crime crime)
        {
            throw new NotImplementedException();
        }

        public async Task<Crime> Get(int id)
        {
            return await this.db.Crimes
                .Include(c => c.Officer)
                .Include(c => c.Publisher)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Crime>> GetCrimeForOfficer()
        {
            var unclosed_case = await this.db.Crimes.Where(
                c => 
                !db.CloseCases.Include(cl => cl.Crime).Any(cl => cl.Crime == c) &&
                c.Officer==)
                .ToListAsync();

            return unclosed_case;
        }

        public async Task PostCrime(PostCrimeDTO postCrime)
        {
            var crime = new Crime
            {
                Description = postCrime.Description,
                Title = postCrime.Title,
                Coordinates = postCrime.Coordinates
            };

            var users = await _user.GetUsersNearAPosition(postCrime.Coordinates);
            var officer = users.Where(u => u.Item2.IsOfficer)
                .OrderBy(u => u.Item1)
                .Select(u => u.Item2).FirstOrDefault();

            db.Crimes.Add(crime);

            await hubContext.Clients.User(officer.Id.ToString()).SendAsync("new_crime", crime);

        }
    }
}
