using System.Collections.Generic;
using System.Threading.Tasks;
using FT.GerenciamentoProcessosJudiciais.Dominio.Processos.Contratos;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Consultas;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FT.GerenciamentoProcessosJudiciais.Api.Controllers
{
    [ApiController, Route("processos/v1")]
    public class ProcessosController : ControllerBase
    {
        private readonly IMediator mediator;

        public ProcessosController(IMediator mediator) => this.mediator = mediator;

        [HttpGet]
        public async Task<DadosPaginados<DadosProcesso>> Get([FromQuery]ConsultarDadosProcessos query) =>
            await mediator.Send(query);

        [HttpGet, Route("{AggregateId}")]
        public async Task<DadosProcesso> Load([FromQuery]CarregarDadosProcesso query) =>
            await mediator.Send(query);

        [HttpGet, Route("{AggregateId}/hierarquia")]
        public async Task<IEnumerable<DadosHierarquiaProcesso>> LoadHierarquia([FromQuery]CarregarDadosHierarquiaProcesso query) =>
            await mediator.Send(query);

        [HttpPost]
        public async Task<ActionResult<ProcessoAtualizado>> Post([FromBody]AtualizarProcesso command)
        {
            var response = await mediator.Send(command);
            if (response.IsValido) return Ok(response); else return BadRequest(response);
        }

        [HttpPut, Route("{AggregateId}")]
        public async Task<ActionResult<ProcessoAtualizado>> Put([FromBody]AtualizarProcesso command)
        {
            var response = await mediator.Send(command);
            if (response.IsValido) return Ok(response); else return BadRequest(response);
        }

        [HttpDelete, Route("{AggregateId}")]
        public async Task<ActionResult<ProcessoRemovido>> Delete([FromQuery]RemoverProcesso command)
        {
            var response = await mediator.Send(command);
            if (response.IsValido) return Ok(response); else return BadRequest(response);
        }
    }
}
