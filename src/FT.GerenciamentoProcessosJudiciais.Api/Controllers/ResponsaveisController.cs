using System.Collections.Generic;
using System.Threading.Tasks;
using FT.GerenciamentoProcessosJudiciais.Dominio.Responsaveis.Contratos;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Consultas;
using FT.GerenciamentoProcessosJudiciais.Relatorios.Contratos.Dados;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FT.GerenciamentoProcessosJudiciais.Api.Controllers
{
    [ApiController, Route("responsaveis/v1")]
    public class ResponsaveisController : ControllerBase
    {
        private readonly IMediator mediator;

        public ResponsaveisController(IMediator mediator) => this.mediator = mediator;

        [HttpGet]
        public async Task<DadosPaginados<DadosResponsavel>> Get([FromQuery]ConsultarDadosResponsaveis query) =>
            await mediator.Send(query);

        [HttpGet, Route("{AggregateId}")]
        public async Task<DadosResponsavel> Load([FromQuery]CarregarDadosResponsavel query) =>
            await mediator.Send(query);

        [HttpPost]
        public async Task<ActionResult<ResponsavelAtualizado>> Post([FromBody]AtualizarResponsavel command)
        {
            var response = await mediator.Send(command);
            if (response.IsValido) return Ok(response); else return BadRequest(response);
        }

        [HttpPut, Route("{AggregateId}")]
        public async Task<ActionResult<ResponsavelAtualizado>> Put([FromBody]AtualizarResponsavel command)
        {
            var response = await mediator.Send(command);
            if (response.IsValido) return Ok(response); else return BadRequest(response);
        }

        [HttpDelete, Route("{AggregateId}")]
        public async Task<ActionResult<ResponsavelRemovido>> Delete([FromQuery]RemoverResponsavel command)
        {
            var response = await mediator.Send(command);
            if (response.IsValido) return Ok(response); else return BadRequest(response);         
        }
    }
}
