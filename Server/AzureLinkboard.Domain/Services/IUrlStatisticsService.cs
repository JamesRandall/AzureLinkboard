using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureLinkboard.Domain.Services
{
    internal interface IUrlStatisticsService
    {
        Task<int> IncrementNumberOfSaves(string url);
    }
}
