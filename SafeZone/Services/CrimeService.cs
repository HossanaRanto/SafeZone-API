using Microsoft.AspNetCore.Hosting.Server;
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
        private readonly IServer server;
        private readonly IPayement payement;

        public CrimeService(
            DataContext db, 
            IHubContext<CrimeHub> hubContext, 
            IUserRepository user, 
            IServer server, 
            IPayement payement)
        {
            this.db = db;
            this.hubContext = hubContext;
            _user = user;
            this.server = server;
            this.payement = payement;
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

        public async Task<List<Crime>> GetCrimeForOfficer(int? start,int? count)
        {
            var unclosed_case = await this.db.Crimes.Where(
                c =>
                !c.IsOk &&
                !db.CloseCases.Include(cl => cl.Crime).Any(cl => cl.Crime == c) &&
                c.Officer == _user.ConnectedOfficer)
                .ToListAsync();

            if(start.HasValue && count.HasValue)
            {
                unclosed_case = unclosed_case.Take(start.Value).Skip(count.Value).ToList();
            }

            return unclosed_case;
        }

        public async Task<Tuple<bool,string>> PassOfficerValidity(Crime crime)
        {
            if (crime.IsOk)
            {
                return new Tuple<bool, string>(false, "Already validated");
            }

            var payement_id = await payement.Create(crime);
            var checkout = new CheckoutOrderResponse
            {
                SessionId = payement_id,
                Crime = crime,
                Paid_at = DateTime.Now
            };

            db.CheckoutOrderResponses.Add(checkout);
            db.SaveChanges();

            return new Tuple<bool, string>(true, "Payement done");
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
