using Newtonsoft.Json;
using NT_Model.Entity;
using NT_Model.ViewModel;
using System;
using System.Linq;
using NT_Database.Infrastructure.Repository;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace NT_Database.Infrastructure.Handler
{
    public class DbOperator : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public DbOperator(IServiceProvider serviceProvider)
        {
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _serviceProvider = serviceProvider;
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
                var handler = _serviceProvider.GetRequiredService(type);
                var result = type.GetMethod(method, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).Invoke(handler, new object[] { opreationModel.Data });
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new DbOperationException(errorMsg);
            }
            /*
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
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new DbOperationException(errorMsg);
            }*/
        }
        public void Dispose()
        {
            _unitOfWork?.Dispose();
        }
    }
}