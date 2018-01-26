using System;
using System.Collections.Generic;
using System.EventSourcing.Client;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using EPY.Services.Common.Service.Exceptions;
using EPY.Services.Common.Service.Models;
using EPY.Services.TipoLogTiempoService.Repositories;
using EPY.Services.TipoLogTiempoService.Services.Models;
using EPY.Services.TipoLogTiempoService.Services.Models.Events;
using Microsoft.AspNetCore.Authorization;

namespace EPY.Services.TipoLogTiempoService.Services
{
    [Authorize]
    public class TipoLogTiempoService : ITipoLogTiempoService
    {
        private readonly IEventClient eventClient;

        private ITipoLogTiempoRepository timelogRepository;

        public TipoLogTiempoService(ITipoLogTiempoRepository timeLogRepository, IEventClient eventClient)
        {
            timelogRepository = timeLogRepository;
            this.eventClient = eventClient;
        }

        public async Task<ServiceResult> AddTipoLogTiempo(string color, float factor, string name)
        {
            Ensure.That(factor, nameof(factor))
                .WithException(x => new ServiceException($"The factor has to be zero or greater than zero"))
                .IsGte(0);
            Ensure.That(name, nameof(name))
                .WithException(x => new ServiceException($"The parameter {x.Name} cannot be <null> or <whitespace>"))
                .IsNotNullOrEmpty();
            Ensure.That(color, nameof(color))
                .WithException(x => new ServiceException($"The parameter {x.Name} cannot be <null> or <whitespace>"))
                .IsNotNullOrEmpty();
            var type = new TipoLogTiempoCreateEvent()
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Factor = factor,
                Color = color,
            };
            try
            {
                await eventClient.Publish(type);
                return new ServiceResult()
                {
                    Status = Results.Success,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult()
                {
                    Error = ex,
                    Status = Results.Failed,
                };
            }
        }

        public async Task<ServiceResult> DeleteTipoLogTiempo(string id)
        {
            try
            {
                var logType = await timelogRepository.GetTimelogById(id);

                if (logType == null)
                {
                    return new ServiceResult
                    {
                        Status = Results.NotFound,
                        Error = new Exception($"The object with id:{id} was not found"),
                    };
                }

                var type = new TipoLogTiempoDeleteEvent()
                {
                    Id = logType.Id.ToString()
                };
                await eventClient.Publish(type);

                return new ServiceResult
                {
                    Status = Results.Success
                };
            }
            catch (Exception e)
            {
                return new ServiceResult
                {
                    Status = Results.Failed,
                    Error = e
                };
            }
        }

        public async Task<ServiceResult<IEnumerable<TipoLogTiempo>>> GetAllTipoLogTiempos()
        {
            try
            {
                var result = await timelogRepository.GetAllTimelogsTypes();

                var TipoLogTiempoList = result.Select(g => new TipoLogTiempo
                {
                    Color = g.Color,
                    Factor = g.Factor,
                    Id = g.Id,
                    Name = g.Name,
                });
                return new ServiceResult<IEnumerable<TipoLogTiempo>>()
                {
                    Result = TipoLogTiempoList,
                    Status = Results.Success,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<IEnumerable<TipoLogTiempo>>()
                {
                    Error = ex,
                    Status = Results.Failed,
                };
            }
        }

        public async Task<ServiceResult<TipoLogTiempo>> GetTipoLogTiempoById(string id)
        {
            try
            {
                var result = await timelogRepository.GetTimelogById(id);
                if (result != null)
                {
                    var TipoLogTiempo = new TipoLogTiempo
                    {
                        Color = result.Color,
                        Factor = result.Factor,
                        Id = result.Id,
                        Name = result.Name,
                    };

                    return new ServiceResult<TipoLogTiempo>()
                    {
                        Result = TipoLogTiempo,
                        Status = Results.Success,
                    };
                }
                else
                {
                    return new ServiceResult<TipoLogTiempo>()
                    {
                        Status = Results.NotFound,
                        Error = new Exception($"The object with id:{id} was not found"),
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult<TipoLogTiempo>()
                {
                    Error = ex,
                    Status = Results.Failed,
                };
            }
        }

        public async Task<ServiceResult<TipoLogTiempo>> UpdateTipoLogTiempo(Guid id, string color, float factor, string name)
        {
            Ensure.That(id, nameof(id))
                .WithException(x => new ServiceException($"The parameter {x.Name} cannot be <null> or <whitespace>"))
                .IsNotEmpty();
            Ensure.That(factor, nameof(factor))
                .WithException(x => new ServiceException($"The factor has to be zero or greater than zero"))
                .IsGte(0);
            Ensure.That(name, nameof(name))
                .WithException(x => new ServiceException($"The parameter {x.Name} cannot be <null> or <whitespace>"))
                .IsNotNullOrEmpty();
            Ensure.That(color, nameof(color))
                .WithException(x => new ServiceException($"The parameter {x.Name} cannot be <null> or <whitespace>"))
                .IsNotNullOrEmpty();
            var type = new TipoLogTiempo()
            {
                Id = id,
                Name = name,
                Factor = factor,
                Color = color,
            };

            try
            {
                await eventClient.Publish(type);
                return new ServiceResult<TipoLogTiempo>()
                {
                    Result = type,
                    Status = Results.Success,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<TipoLogTiempo>()
                {
                    Error = ex,
                    Status = Results.Failed,
                };
            }
        }
    }
}
