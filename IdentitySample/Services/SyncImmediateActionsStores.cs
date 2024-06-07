using IdentitySample.Models.Context;
using Kaktos.UserImmediateActions.Models;
using Kaktos.UserImmediateActions.Stores;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IdentitySample.Services
{
    public class SyncImmediateActionsStores
    {
        private readonly AppDbContext _dbContext;
        private readonly IImmediateActionsStore _mainStore;

        public SyncImmediateActionsStores(AppDbContext dbContext, IImmediateActionsStore mainStore)
        {
            _dbContext = dbContext;
            _mainStore = mainStore;
        }

        public async Task SyncImmediateActionsFromPermanentStoreToMainStoreAsync()
        {
            if (!await _dbContext.ImmediateActions.AnyAsync())
            {
                return;
            }

            var actions = await _dbContext.ImmediateActions
                .Where(i => i.ExpirationTime > DateTime.Now)
                .ToListAsync();

            foreach (var action in actions)
            {
                var expirationTime = action.ExpirationTime - DateTime.Now;

                await _mainStore.AddAsync(action.ActionKey, 
                    expirationTime,
                    new ImmediateActionDataModel(action.AddedDate, action.Purpose),
                    false);
            }
        }
    }
}
