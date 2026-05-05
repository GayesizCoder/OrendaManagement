using System.Collections.Generic;
using Orenda.Web.Models;

namespace Orenda.Web.Models.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public Kullanici Kullanici { get; set; } = null!;
        public SaglikVerisi? SaglikVerisi { get; set; }
        public List<SistemLog> GirisCikisLoglari { get; set; } = new();
        public List<Talep> Talepler { get; set; } = new();
        public List<Izin> Izinler { get; set; } = new();
        public List<ToDo> Gorevler { get; set; } = new();
    }
}
