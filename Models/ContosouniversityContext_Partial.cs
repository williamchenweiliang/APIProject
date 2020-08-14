using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APIHomeWork.Models
{
    public partial class ContosouniversityContext : DbContext
    {
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var ModifiedEntities = ChangeTracker.Entries().ToList();

            foreach (var Item in ModifiedEntities)
            {
                switch (Item.Entity.GetType().Name.ToString())
                {
                    case "Course":
                    case "Department":
                    case "Person":
                        if (Item.State == EntityState.Deleted)
                        {
                            Item.State = EntityState.Modified;
                            Item.CurrentValues.SetValues(new { DateModified = DateTime.Now, IsDeleted = true });
                        }
                        else
                        {
                            Item.CurrentValues.SetValues(new { DateModified = DateTime.Now });
                        }
                        break;
                }   
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}