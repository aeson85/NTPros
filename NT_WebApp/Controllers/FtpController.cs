using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.FileProviders.Physical;
using NT_WebApp.Models.ViewModels;

namespace NT_WebApp.Controllers
{
    [Route("api/[controller]")]
    public class FtpController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IFileProvider _fileProvider;

        public FtpController(IConfiguration configuration, IFileProvider fileProvider, IMapper mapper)
        {
            _configuration = configuration;
            _fileProvider = fileProvider;
            _mapper = mapper;
        }

        [HttpGet("{*relPath}")]
        public IActionResult Get()
        {
            var relPath = (this.RouteData.Values["relPath"] as string) ?? string.Empty;
            var content = _fileProvider.GetDirectoryContents(relPath);
            if (content.Exists)
            {
                var result = _mapper.Map<List<PhysicalDirectoryInfoViewModel>>(content.ToList());
                return Ok(result);
            }
            return NotFound();
        }
    }
}