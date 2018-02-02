using System;
using NT_Model.ViewModel;

namespace NT_Database.Infrastructure
{
    public class DbOperationException : Exception
    {
        private DbOperationResultViewModel _result;
        private readonly string _errorMsg;

        public DbOperationException(string errorMsg)
        {
            _errorMsg = errorMsg;
            _result = new DbOperationResultViewModel 
            {
                ErrorMsg = _errorMsg
            };
        }

        public DbOperationResultViewModel Result => _result;
        
    }
}