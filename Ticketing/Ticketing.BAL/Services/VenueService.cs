using Ticketing.BAL.Contracts;
using Ticketing.BAL.Model;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Repositories;
using Mapster;
using log4net;
using Microsoft.Extensions.Logging;

namespace Ticketing.BAL.Services
{
    public class VenueService : IVenueService
    {
        private readonly IRepository<Venue> _repository;
        private readonly IRepository<Section> _repositorySection;
        private readonly ILog _logger;

        public VenueService(Repository<Venue> repository, Repository<Section> repositorySection, ILog logger)
        {
            _repository = repository;
            _repositorySection = repositorySection;
            _logger = logger;
        }

        public Task<IEnumerable<VenueReturnModel>> GetVenuesAsync()
        {
            _logger.Info("VenueService Start Get venues.");
            var venues = _repository.GetAll();

            _logger.Info("VenueService  Getvenues return result.");
            return  Task.FromResult<IEnumerable<VenueReturnModel>>(venues.ProjectToType<VenueReturnModel>().ToList());
        }

        public async Task<IEnumerable<SectionReturnModel>> GetSectionsOfVenueAsync(int venueId)
        {
            _logger.Info($"VenueService Start SGetSectionsOfVenue. {venueId}");
            var sections = _repositorySection.GetAll();

            _logger.Info($"VenueService Return SGetSectionsOfVenue. {venueId}");
            return await Task.FromResult<IEnumerable<SectionReturnModel>>(sections.Where(s => s.VenueId == venueId).ProjectToType<SectionReturnModel>().ToList());
        }
    }
}
