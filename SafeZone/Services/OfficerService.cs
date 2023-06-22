using Microsoft.EntityFrameworkCore;
using SafeZone.Data;
using SafeZone.Models;
using SafeZone.Repositories;

namespace SafeZone.Services
{
    public class OfficerService : IOfficerRepository
    {
        private readonly DataContext db;
        private readonly IUserRepository _user;

        public OfficerService(DataContext db)
        {
            this.db = db;
            db.Officers.Include(o => o.User);
        }

        public async Task<Officer> Get(int id)
        {
            var officer = await db.Officers.FirstOrDefaultAsync(o => o.Id == id);
            return officer;
        }

        public async Task<Officer> GetFromUser(int userid)
        {
            var officer = await db.Officers.FirstOrDefaultAsync(o => o.User.Id == userid);
            return officer;
        }
    }
}
