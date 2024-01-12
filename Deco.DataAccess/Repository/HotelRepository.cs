using Deco.DataAccess.Data;
using Deco.DataAccess.Repository.IRepository;
using Deco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Deco.DataAccess.Repository
{
    public class HotelRepository : Repository<Hotel>, IHotelRepository
    {
        private readonly ApplicationDbContext _db;
        public HotelRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Hotel obj)
        {
            _db.Hotels.Update(obj);
        }
    }
}
