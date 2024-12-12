using CourseProject.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.BLL.Interfaces
{
    public interface IIndicatorService
    {
        Task<Guid> CreateIndicator(IndicatorModel model);
        Task<List<IndicatorModel>> GetIndicators();
        Task DeleteIndicator(Guid id);
        Task UpdateIndicatorValue(UpdateIndicatorValueModel model);
    }
}