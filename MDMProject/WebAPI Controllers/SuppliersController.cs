using MDMProject.Data;
using MDMProject.Mappers;
using MDMProject.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace MDMProject.WebAPI_Controllers
{
    public class SuppliersController : ApiController
    {
        // GET: api/Supplier
        public async Task<IEnumerable<CollectionPointViewModel>> Get()
        {
            using (var db = new ApplicationDbContext())
            {
                var allUsers = await db.GetAllCollectionPoints().AsNoTracking().ToListAsync();
                var allAddresses = await db.Addresses.AsNoTracking().ToListAsync();

                var usersToMap = allUsers.Where(x => x.Address != null && x.Address.Latitude != null && x.Address.Longitude != null && x.ApprovedBy != null).ToList();
                var collectionPointViewModels = usersToMap.ToCollectionPointViewModels().ToList();

                return collectionPointViewModels;
            }
        }

        // GET: api/Supplier/5
        public async Task<CollectionPointViewModel> Get(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (user != null)
                {
                    var supplierViewModel = user.ToCollectionPointViewModel();
                    return supplierViewModel;
                }

                return null;
            }
        }
    }
}