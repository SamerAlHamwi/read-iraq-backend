using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Attachments;
using ReadIraq.Localization.SourceFiles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Advertisiments
{
    public class AdvertisimentManager : DomainService, IAdvertisimentManager
    {
        private readonly IRepository<Advertisiment> _advertisimentrepository;
        private readonly IRepository<AdvertisimentPosition> _advertisimentPositionRepository;
        private readonly IAttachmentManager _attachmentManager;
        public AdvertisimentManager(
            IRepository<Advertisiment> advertisimentRepository,
            IRepository<AdvertisimentPosition> advertisimentPositionRepository,
            IAttachmentManager attachmentManager
            )
        {
            _advertisimentrepository = advertisimentRepository;
            _advertisimentPositionRepository = advertisimentPositionRepository;
            _attachmentManager = attachmentManager;

        }



        public async Task AddPositionToAdvertisimentAsync(AdvertisimentPosition advertisimentPosition)
        {
            if (await _advertisimentPositionRepository.GetAll().Where(x => x.AdvertisimentId == advertisimentPosition.AdvertisimentId && x.Screen == advertisimentPosition.Screen && x.Position == advertisimentPosition.Position).AnyAsync())
            {
                throw new UserFriendlyException(string.Format(L(Exceptions.ThisPositionIsAlreadyExist)));
            }
            await _advertisimentPositionRepository.InsertAsync(advertisimentPosition);
        }

        public async Task<Advertisiment> CheckAdvertisiment(int Id)
        {
            return await _advertisimentrepository.FirstOrDefaultAsync(x => x.Id == Id);
        }



        public async Task<List<AdvertisimentPosition>> GetAdvertisimentPositionsAsync(int advertisimentId)
        {
            var Advertisiment = await CheckAdvertisiment(advertisimentId);
            if (Advertisiment == null)
            {
                throw new UserFriendlyException(L(Exceptions.ObjectWasNotFound));
            }
            return await _advertisimentPositionRepository.GetAll().Where(x => x.AdvertisimentId == advertisimentId).ToListAsync();
        }

        public async Task<Advertisiment> GetEntityAsync(int Id)
        {
            return await _advertisimentrepository.GetAllIncluding(x => x.AdvertisimentPositions).FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Advertisiment> InsertAsync(Advertisiment advertisiment)
        {
            return await _advertisimentrepository.InsertAsync(advertisiment);
        }



        public async Task RemovePositionFromAdvertisiment(AdvertisimentPosition advertisimentPosition)
        {
            var Position = await _advertisimentPositionRepository.GetAll().Where(x => x.AdvertisimentId == advertisimentPosition.AdvertisimentId && x.Screen == advertisimentPosition.Screen && x.Position == advertisimentPosition.Position).FirstOrDefaultAsync();
            if (Position == null)
            {
                throw new UserFriendlyException(string.Format(L(Exceptions.ObjectWasNotFound)));
            }
            await _advertisimentPositionRepository.HardDeleteAsync(Position);
        }

    }
}

