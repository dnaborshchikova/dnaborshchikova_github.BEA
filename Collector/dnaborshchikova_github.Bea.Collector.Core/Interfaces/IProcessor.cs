using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnaborshchikova_github.Bea.Collector.Core.Interfaces
{
    public interface IProcessor
    {
        public Task<bool> ProcessAsync(List<EventProcessRange> ranges);
    }
}
