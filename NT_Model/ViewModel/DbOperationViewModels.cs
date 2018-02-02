namespace NT_Model.ViewModel
{
    public class DbOperationViewModel
    {
        public string Data { get; set; }

        public string OperationRoute { get; set; }
    }

    public class DbOperationResultViewModel
    {
        public bool Success { get; set; }

        public string Data { get; set; }

        private string _errorMsg;
        public string ErrorMsg 
        { 
            get => _errorMsg;
            set
            {
                this.Success = string.IsNullOrWhiteSpace(value) ? true : false;
                _errorMsg = value;
            }
        }
    }
}