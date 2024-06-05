using IdentitySample.Models.Context;
using Kaktos.UserImmediateActions.Models;
using Kaktos.UserImmediateActions.Stores;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using IdentitySample.Models;

namespace IdentitySample.Services
{
    public class ApplicationPermanentImmediateActionsStore : IPermanentImmediateActionsStore
    {
        private readonly AppDbContext _dbContext;

        public ApplicationPermanentImmediateActionsStore(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(string key, DateTime expirationTime, ImmediateActionDataModel data)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var model = new ImmediateAction
            {
                ActionKey = key,
                ExpirationTime = expirationTime,
                AddedDate = data.AddedDate,
                Purpose = data.Purpose
            };

            _dbContext.ImmediateActions.Add(model);

            _dbContext.SaveChanges();
        }

        public async Task AddAsync(string key, DateTime expirationTime, ImmediateActionDataModel data, CancellationToken cancellationToken = new())
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var model = new ImmediateAction
            {
                ActionKey = key,
                ExpirationTime = expirationTime,
                AddedDate = data.AddedDate,
                Purpose = data.Purpose
            };

            await _dbContext.ImmediateActions.AddAsync(model, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

