using System;
using System.Threading.Tasks;
using ConfigStore.Api.Data;
using ConfigStore.Api.Data.Models;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.EntityFrameworkCore;

namespace ConfigStore.Api.Infrastructure.ActionHandlers {
    public class RenameModelActionHandler<T> where T : ModelBase {
        private readonly ConfigStoreContext _context;

        public RenameModelActionHandler(ConfigStoreContext context) {
            _context = context;
        }

        public async Task<bool> Do(Guid key, string name) {
            T entity = await _context.Set<T>().FirstOrDefaultAsync(obj => obj.Key == key);
            if (entity == null) {
                return false;
            }
            entity.Name = name;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}