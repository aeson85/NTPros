using System;
using System.Linq.Expressions;
using AutoMapper;
using Newtonsoft.Json;
using NT_Database.Infrastructure.Repository;
using NT_Model.Entity;
using NT_Common.Extensions;
using NT_Model.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace NT_Database.Infrastructure.Handler
{
    public class AppUserDbHandler : DbHandler<AppUser>
    {
        private readonly IRepository<AppUser> _repository;
        private readonly IMapper _mapper;
        
        public AppUserDbHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _repository = unitOfWork.Repository<AppUser>();
            _mapper = mapper;
        }

        public DbOperationResultViewModel UpsertForWeChat(string entityStr)
        {
            var result = this.CreateReponse();
            var entity = JsonConvert.DeserializeObject<AppUser>(entityStr); 
            Expression<Func<AppUser, bool>> predicate = p => p.WeChatInfo.OpenId == entity.WeChatInfo.OpenId;
            var appUser = _repository.SingleOrDefault(predicate, disableTracking: false, include: query => query.Include(p => p.WeChatInfo));
            if (appUser == null)
            {
                _repository.Add(entity);
            }
            else
            {
                appUser = _mapper.Map(entity, appUser);
                _repository.Update(appUser);
            }
            this.UnitOfWork.Commit();
            return result;
        }

        public override DbOperationResultViewModel Select(string entityStr)
        {
            var result = this.CreateReponse();
            var entity = JsonConvert.DeserializeObject<AppUser>(entityStr);   
            Expression<Func<AppUser, bool>> predicate = p => true;
            if (!string.IsNullOrWhiteSpace(entity.Id))
            {
                predicate = predicate.AndAlso(p => p.Id == entity.Id);
            }
            else if (!string.IsNullOrWhiteSpace(entity.WeChatInfo?.OpenId))
            {
                predicate = predicate.AndAlso(p => p.WeChatInfo.OpenId == entity.WeChatInfo.OpenId);
            }
            var appUsers = _repository.Get(predicate);
            result.Data = JsonConvert.SerializeObject(appUsers);
            return result;
        }
    }
}