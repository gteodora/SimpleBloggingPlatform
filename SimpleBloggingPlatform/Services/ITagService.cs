using SimpleBloggingPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBloggingPlatform.Services
{
    public interface ITagService
    {
       
        public Task<int> CreateTagAsync(string name);
        public Tag FindByName(string name);
    }
}
