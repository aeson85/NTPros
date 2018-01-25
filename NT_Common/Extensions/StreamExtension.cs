using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NT_Common.Extensions
{
    public static class StreamExtension
    {
        public static async Task<byte[]> ToByteArray(this Stream stream)
        {
            var sr = new StreamReader(stream);
            var content = await sr.ReadToEndAsync();
            return Encoding.UTF8.GetBytes(content);
        }
    }
}