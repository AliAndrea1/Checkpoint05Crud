using System;
using System.IO;

namespace Checkpoint05Crud.Services
{
    public static class LogService
    {
        private static readonly string caminhoLog =
            Path.Combine(AppContext.BaseDirectory, "operacoes.log");

        public static void Registrar(string mensagem)
        {
            try
            {
                string registro =
                    $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {mensagem}";

                File.AppendAllText(
                    caminhoLog,
                    registro + Environment.NewLine
                );
            }
            catch
            {  
            }
        }
    }
}