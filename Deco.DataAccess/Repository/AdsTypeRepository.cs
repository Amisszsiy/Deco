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
    public class AdsTypeRepository : Repository<AdsType>, IAdsTypeRepository
    {
        private readonly ApplicationDbContext _db;
        public AdsTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(AdsType obj)
        {
            _db.AdsType.Update(obj);
        }
    }
}
