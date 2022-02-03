using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsumoApiViaCep.Controllers
{
    [ApiController]
    public class ViaCepController : ControllerBase
    {
        [HttpGet("ConsumoApiViaCep")]
        public IActionResult ApiViaCep(string cep)
        {
            var logEntrada = new ConsumoApi.Contratos.Log.POST.Request();
            logEntrada.NomeAplicacao = "ConsumoApiCep";
            logEntrada.NomeMaquina = Environment.MachineName;
            logEntrada.Usuario = Environment.UserName;

            try
            {
                cep.Replace(".", "").Replace("-", "").Replace(" ", "");

                if (string.IsNullOrWhiteSpace(cep) || cep.Length < 8 || cep.Length > 8)
                {
                    throw new InvalidOperationException("O cep deve ter 8 digitos");
                }
                long i;
                if (long.TryParse(cep, out i) == false)
                {
                    throw new InvalidOperationException("Não pode conter letras no campo nesse cep");
                }
                var retornoCep = ConsumoApi.ExecutarApi.ConsultaCepGet<ConsumoApi.Contratos.ApiViaCep.GET.Response>(
                   string.Format("https://viacep.com.br/ws/{0}/json/", cep));

                if (retornoCep.cep == null)
                    throw new Exception("CEP INVALIDO TENTE NOVAMENTE");

                return StatusCode(200, retornoCep);
            }
            catch (InvalidOperationException ex)
            {
                logEntrada.MensagemErro = ex.Message;
                logEntrada.RastreioErro = ex.StackTrace;
                logEntrada.DataHora = DateTime.Now;
                var request = ConsumoApi.ExecutarApi.LogErro<ConsumoApi.Contratos.Log.POST.Request>("https://logaplicacao.aiur.com.br/v1/Logs", logEntrada);
                return StatusCode(400, ex.Message);

            }
            catch (Exception ex)
            {
                logEntrada.MensagemErro = ex.Message;
                logEntrada.RastreioErro = ex.StackTrace;
                logEntrada.DataHora = DateTime.Now;
                var request = ConsumoApi.ExecutarApi.LogErro<ConsumoApi.Contratos.Log.POST.Request>("https://logaplicacao.aiur.com.br/v1/Logs", logEntrada);
                return StatusCode(404, ex.Message);
            }
            
        }

    }
}
