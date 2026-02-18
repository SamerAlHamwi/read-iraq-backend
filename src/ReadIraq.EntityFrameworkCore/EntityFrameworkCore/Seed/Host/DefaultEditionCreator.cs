using Abp.Application.Editions;
using Abp.Application.Features;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Editions;
using System;
using System.Linq;

namespace ReadIraq.EntityFrameworkCore.Seed.Host
{
    public class DefaultEditionCreator
    {
        private readonly ReadIraqDbContext _context;

        public DefaultEditionCreator(ReadIraqDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateEditions();
        }

        private void CreateEditions()
        {
            try
            {
                var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
                if (defaultEdition == null)
                {
                    defaultEdition = new Edition { Name = EditionManager.DefaultEditionName, DisplayName = EditionManager.DefaultEditionName };
                    _context.Editions.Add(defaultEdition);
                    _context.SaveChanges();

                    /* Add desired features to the standard edition, if wanted... */
                }
            }
            catch (Exception)
            {
                // Table does not exist yet. This can happen during initial migration/database creation.
                // We catch the exception to prevent the seeder from crashing the application.
            }
        }

        private void CreateFeatureIfNotExists(int editionId, string featureName, bool isEnabled)
        {
            try
            {
                if (_context.EditionFeatureSettings.IgnoreQueryFilters().Any(ef => ef.EditionId == editionId && ef.Name == featureName))
                {
                    return;
                }

                _context.EditionFeatureSettings.Add(new EditionFeatureSetting
                {
                    Name = featureName,
                    Value = isEnabled.ToString(),
                    EditionId = editionId
                });
                _context.SaveChanges();
            }
            catch (Exception)
            {
                // Handle missing table or other DB issues gracefully during seeding
            }
        }
    }
}
