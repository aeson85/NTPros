using System;

namespace NT_Model.ViewModel
{
    public interface IPhysicalDirectoryInfoViewModel
    {
        string Name { get; set; }
        
        long Length { get; set; }

        bool IsDirectory { get; set; }

        DateTimeOffset LastModified { get; }
        
        string Url { get; set; }
    }
    
    public class PhysicalDirectoryInfoViewModel :　IPhysicalDirectoryInfoViewModel
    {
        public string Name { get; set; }

        public long Length { get; set; }

        public bool IsDirectory { get; set; }

        public DateTimeOffset LastModified { get; }

        public string Url { get; set; }
    }
}