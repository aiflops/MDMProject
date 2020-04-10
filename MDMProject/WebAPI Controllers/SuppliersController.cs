using MDMProject.Data;
using MDMProject.Mappers;
using MDMProject.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace MDMProject.WebAPI_Controllers
{
    public class SuppliersController : ApiController
    {
        // GET: api/Supplier
        public async Task<IEnumerable<SupplierViewModel>> Get()
        {
            using (var db = new ApplicationDbContext())
            {
                var allUsers = await db.Users.AsNoTracking().ToListAsync();
                var allOfferedEquipments = await db.ProtectiveEquipments.AsNoTracking().ToListAsync();
                var allAddresses = await db.Addresses.AsNoTracking().ToListAsync();
                var offeredHelps = await db.OfferedHelps.AsNoTracking().ToListAsync();
                var equipmentTypes = await db.EquipmentTypes.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name);
                var helpTypes = await db.HelpTypes.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name);

                var usersToMap = allUsers.Where(x => x.Address != null && x.Address.Latitude != null && x.Address.Longitude != null).ToList();
                var supplierViewModels = usersToMap.ToSupplierViewModels(equipmentTypes, helpTypes).ToList();

                return supplierViewModels;
            }
        }

        // GET: api/Supplier/5
        public async Task<SupplierViewModel> Get(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (user != null)
                {
                    var equipmentTypes = db.EquipmentTypes.AsNoTracking().ToDictionary(x => x.Id, x => x.Name);
                    var helpTypes = db.HelpTypes.AsNoTracking().ToDictionary(x => x.Id, x => x.Name);

                    var supplierViewModel = user.ToSupplierViewModel(equipmentTypes, helpTypes);
                    return supplierViewModel;
                }

                return null;
            }
        }
    }
}