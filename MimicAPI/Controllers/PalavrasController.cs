using Microsoft.AspNetCore.Mvc;
using MimicAPI.Database;
using MimicAPI.Models;
using System;
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

        //Nesse caso quando o route está em branco ele pega a rota padão  " api/palavras "
        [Route("")]
        [HttpGet]
        public ActionResult ObterPalavras()
        {
            return new JsonResult( _banco.Palavras);
        }

        // Nesse caso está buncando por ID  " /api/palavras/1 "
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            //O tipo de retorno OK devolve o tipo mais popular que nesse caso JsonResult
            return Ok(_banco.Palavras.Find(id));
        }

        // Nesse caso estamos enviando para o servidor atras do metodo POST (id, nome, ativo, pontuacao, criacao)
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();

            return Ok();
        }

        // PUT (id, nome, ativo, pontuacao, criacao)
        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar (int id, [FromBody]Palavra palavra)
        {
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
            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return Ok();
        }

    }
}
