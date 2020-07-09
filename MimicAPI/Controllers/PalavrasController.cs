using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
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
        private readonly MimicContext _banco;
        public PalavrasController(MimicContext banco)
        {
            _banco = banco;
        }

        //Nesse caso quando o route está em branco ele pega a rota padão  " api/palavras?data=2020-07/05 "
        [Route("")]
        [HttpGet]
        public ActionResult ObterPalavras([FromQuery]PalavraUrlQuery query)
        {
            //Para a variavel item nao ser um arquivo de banco e sim uma query
            var item = _banco.Palavras.AsQueryable();

            //Verificando se data tem valor
            if (query.Data.HasValue)
            {
                // Buscando registros do banco onde data informada for maior que a data do registro do banco
                item = item.Where(a => a.Criado > query.Data.Value || a.Atualizado > query.Data.Value);            
            }

            if (query.PagNumero.HasValue)
            {
                //Contando quantos registros tem no objeto Palavras
                var quantidadeTotalRegistros = item.Count();

                //Logica da Paginacao          ' Skip() é pular '         ' Take() é pegar '
                item = item.Skip((query.PagNumero.Value - 1) * query.PagRegistro.Value).Take(query.PagRegistro.Value);

                var paginacao = new Paginacao();
                paginacao.NumeroPagina = query.PagNumero.Value;
                paginacao.RegistrosPorPagina = query.PagRegistro.Value;
                paginacao.TotalRegistros = quantidadeTotalRegistros;
                paginacao.TotalPagina = (int) Math.Ceiling((double)quantidadeTotalRegistros / query.PagRegistro.Value);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginacao));

                if(query.PagNumero > paginacao.TotalPagina)
                {
                    return NotFound();
                }
                
            }


            return new JsonResult(item);
        }

        


        // Nesse caso está buncando por ID  " /api/palavras/1 "
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            
            var obj = _banco.Palavras.Find(id);

            if(obj == null)
            {
                return NotFound();
            }

            //O tipo de retorno OK devolve o tipo mais popular que nesse caso JsonResult
            return Ok();
        }

        // Nesse caso estamos enviando para o servidor atras do metodo POST (id, nome, ativo, pontuacao, criacao)
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();

            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        // PUT (id, nome, ativo, pontuacao, criacao)
        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar (int id, [FromBody]Palavra palavra)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(x => x.Id == id);
  
            if (obj == null)
            {
                return NotFound();
            }

            palavra.Id = id;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return Ok();
        }

        // DELETE (id, nome, ativo, pontuacao, criacao)
        [Route("{id}")] 
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _banco.Palavras.Find(id);
            if(palavra == null)
            {
                return NotFound();
            }

            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            //Vai retorna o Codigo 204 de Sucesso mais sem conteudo para mostrar já que é uma deleção
            return NoContent();
        }

    }
}
