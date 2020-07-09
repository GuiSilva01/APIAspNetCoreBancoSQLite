using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Interface;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Controllers
{
    //Configuração da rota da controller Palavras por meio do atributo
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        public PalavrasController(IPalavraRepository repository)
        {
            _repository = repository;
        }

        //Nesse caso quando o route está em branco ele pega a rota padão  " api/palavras?data=2020-07/05 "
        [Route("")]
        [HttpGet]
        public ActionResult ObterPalavras([FromQuery]PalavraUrlQuery query)
        {
           var item =  _repository.ObterPalavras(query);

            if (query.PagNumero > item.Paginacao.TotalPagina)
            {
                return NotFound();
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));

            return new JsonResult(item.ToList());
        }

        


        // Nesse caso está buncando por ID  " /api/palavras/1 "
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {

            var obj = _repository.Obter(id);

            if(obj == null)
            {
                return NotFound();
            }

            //O tipo de retorno OK devolve o tipo mais popular que nesse caso JsonResult
            return Ok(obj);
        }

        // Nesse caso estamos enviando para o servidor atras do metodo POST (id, nome, ativo, pontuacao, criacao)
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _repository.Cadastrar(palavra);

            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        // PUT (id, nome, ativo, pontuacao, criacao)
        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar (int id, [FromBody]Palavra palavra)
        {
            var obj = _repository.Obter(id);
  
            if (obj == null)
            {
                return NotFound();
            }

            palavra.Id = id;
            _repository.Atualizar(palavra);

            return Ok();
        }

        // DELETE (id, nome, ativo, pontuacao, criacao)
        [Route("{id}")] 
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _repository.Obter(id);
            palavra.Ativo = false;

            if(palavra == null)
            {
                return NotFound();
            }

            _repository.Deletar(id);
            

            //Vai retorna o Codigo 204 de Sucesso mais sem conteudo para mostrar já que é uma deleção
            return NoContent();
        }

    }
}
