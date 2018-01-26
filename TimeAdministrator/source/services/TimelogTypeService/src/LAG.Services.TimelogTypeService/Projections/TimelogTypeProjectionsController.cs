using System;
using System.Threading.Tasks;
using EPY.Services.TipoLogTiempoService.Projections.Events;
using EPY.Services.TipoLogTiempoService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Repo = EPY.Services.TipoLogTiempoService.Repositories.Models;

namespace EPY.Services.TipoLogTiempoService.Projections
{
    [Route("/v1/events/types")]
    public class TipoLogTiempoProjectionsController
    {
        private readonly ITipoLogTiempoRepository repository;

        public TipoLogTiempoProjectionsController(ITipoLogTiempoRepository repository)
        {
            this.repository = repository;
        }

        [HttpPut]
        public async Task CreateTipoLogTiempo([FromBody]TipoLogTiempoCreateEvent type)
        {
            await repository.AddTipoLogTiempo(new Repo.TipoLogTiempo()
            {
                Color = type.Color,
                Factor = type.Factor,
                Id = new Guid(type.Id),
                Name = type.Name,
            });
        }

        [HttpDelete]
        public async Task DeleteTipoLogTiempo([FromBody]TipoLogTiempoDeleteEvent type)
        {
            await repository.DeleteTipoLogTiempo(type.Id);
        }

        [HttpPost]
        public async Task UpdateTipoLogTiempo([FromBody]TipoLogTiempoUpdateEvent type)
        {
            await repository.UpdateTipoLogTiempo(new Repo.TipoLogTiempo()
            {
                Color = type.Color,
                Factor = type.Factor,
                Id = new Guid(type.Id),
                Name = type.Name,
            });
        }
    }
}
