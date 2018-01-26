using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPY.Services.TipoLogTiempoService.Repositories.Models;

namespace EPY.Services.TipoLogTiempoService.Repositories.Memory
{
    public class MemoryTipoLogTiempoRepository : ITipoLogTiempoRepository
    {
        public MemoryTipoLogTiempoRepository()
        {
            // Todo: Delete Test Data.
            var guid = Guid.NewGuid();
            TimeLogsById.Add(guid.ToString(), new TipoLogTiempo()
            {
                Id = guid,
                Color = "#FF0000",
                Factor = 1,
                Name = "Exclusive",
            });
            guid = Guid.NewGuid();
            TimeLogsById.Add(guid.ToString(), new TipoLogTiempo()
            {
                Id = guid,
                Color = "#FFFFFF",
                Factor = 0.8f,
                Name = "Aniversary",
            });
            guid = Guid.NewGuid();
            TimeLogsById.Add(guid.ToString(), new TipoLogTiempo()
            {
                Id = guid,
                Color = "#000000",
                Factor = 0.5f,
                Name = "Rain day",
            });
            guid = Guid.NewGuid();
            TimeLogsById.Add(guid.ToString(), new TipoLogTiempo()
            {
                Id = guid,
                Color = "#FF00FF",
                Factor = 0.3f,
                Name = "Allo",
            });
            guid = Guid.NewGuid();
            TimeLogsById.Add(guid.ToString(), new TipoLogTiempo()
            {
                Id = guid,
                Color = "#FFFF00",
                Factor = 1,
                Name = "Holiday",
            });
        }

        public Dictionary<string, TipoLogTiempo> TimeLogsById { get; set; } = new Dictionary<string, TipoLogTiempo>();

        public Task AddTipoLogTiempo(TipoLogTiempo type)
        {
            TimeLogsById.Add(type.Id.ToString(), type);
            return Task.CompletedTask;
        }

        public Task DeleteTipoLogTiempo(string id)
        {
            TimeLogsById.Remove(id);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<TipoLogTiempo>> GetAllTimelogsTypes()
        {
            var result = TimeLogsById.Select(g => g.Value).ToList();
            return Task.FromResult(result as IEnumerable<TipoLogTiempo>);
        }

        public Task<TipoLogTiempo> GetTimelogById(string id)
        {
            var result = TimeLogsById.Where(g => g.Key == id).FirstOrDefault();
            return Task.FromResult(result.Value);
        }

        public Task UpdateTipoLogTiempo(TipoLogTiempo type)
        {
            TimeLogsById[type.Id.ToString()] = type;
            return Task.CompletedTask;
        }
    }
}