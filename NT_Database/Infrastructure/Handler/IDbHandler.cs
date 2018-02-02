using System;
using Newtonsoft.Json;
using NT_Database.Infrastructure.Repository;
using NT_Model.Entity;
using NT_Model.ViewModel;

namespace NT_Database.Infrastructure.Handler
{
    public interface IDbHandler<T> where T : class, IBaseEntity
    {
    }

    public abstract class DbHandler<T> : IDbHandler<T> where T : class, IBaseEntity
    {
        private readonly IUnitOfWork _unitOfWork;

        protected IUnitOfWork UnitOfWork => _unitOfWork;

        public DbHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        protected DbOperationResultViewModel CreateReponse(bool success = true, string errorMsg = "")
        {
            return new DbOperationResultViewModel
            {
                Success = success,
                ErrorMsg = errorMsg
            };
        }

        public virtual DbOperationResultViewModel Create(string entityStr)
        {
            var result = this.CreateReponse();
            T entity = JsonConvert.DeserializeObject<T>(entityStr);
            _unitOfWork.Repository<T>().Add(entity);
            _unitOfWork.Commit();
            return result;
        }

        public virtual DbOperationResultViewModel Update(string entityStr)
        {
            var result = this.CreateReponse();
            T entity = JsonConvert.DeserializeObject<T>(entityStr);
            _unitOfWork.Repository<T>().Update(entity);
            _unitOfWork.Commit();
            return result;
        }

        public abstract DbOperationResultViewModel Select(string entityStr);
    }
}