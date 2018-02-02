using Newtonsoft.Json;
using NT_Model.Entity;
using NT_Model.ViewModel;
using System;
using System.Linq;
using NT_Database.Infrastructure.Repository;
using System.Reflection;
using AutoMapper;

namespace NT_Database.Infrastructure.Handler
{
    public class DbOperator : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DbOperator(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public string Execute(string message)
        {
            try
            {
                var opreationModel = JsonConvert.DeserializeObject<DbOperationViewModel>(message);
                var operRouteLst = opreationModel.OperationRoute.Split(".");
                var entityName = operRouteLst[0];
                var method = operRouteLst[1];
                var type = typeof(DbOperator).Assembly.GetTypes().Single(p => p.Name.Equals($"{entityName}DbHandler", StringComparison.OrdinalIgnoreCase));
                var handler = Activator.CreateInstance(type, _unitOfWork, _mapper);
                var result = type.GetMethod(method, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).Invoke(handler, new object[] { opreationModel.Data });
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                throw new DbOperationException(ex.InnerException?.Message);
            }
        }
        public void Dispose()
        {
            _unitOfWork?.Dispose();
        }
    }
}