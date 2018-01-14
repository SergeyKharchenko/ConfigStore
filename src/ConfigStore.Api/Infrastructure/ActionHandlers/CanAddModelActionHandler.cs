using System.Threading.Tasks;
using ConfigStore.Api.Data;
using ConfigStore.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfigStore.Api.Infrastructure.ActionHandlers {
    public class CanAddModelActionHandler<T> where T : ModelBase {
        private readonly ConfigStoreContext _context;

        public CanAddModelActionHandler(ConfigStoreContext context) {
            _context = context;
        }

        public async Task<bool> Do(string name) {
            return !await _context.Set<T>().AnyAsync(obj => obj.Name.ToLower() == name.ToLower());
        }
    }
}