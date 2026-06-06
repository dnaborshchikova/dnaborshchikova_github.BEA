using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnaborshchikova_github.Bea.Collector.Core.Interfaces
{
    public interface IEventProcessor
    {
        public Task ProcessAsync();
    }
}
