using Application.Contracts.Response;
using Persistence.Models;

namespace ShoraWorkManager.Models
{
    public class ConstructionSiteDetailsViewModel
    {
        public ConstructionSite ConstructionSite { get; set; }
        public List<MaterialStockDto> MaterialMovements { get; set; } = new List<MaterialStockDto>();
        public List<WorkedHoursDto> WorkedHours { get; set; } = new List<WorkedHoursDto>();
    }
}
