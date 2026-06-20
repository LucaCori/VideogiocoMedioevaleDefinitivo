using System;
using System.IO;

namespace Program
{
    // 1.1 SISTEMA DI LOGGING (Nuovo Requisito)
    ///<summary>
    /// Classe statica per la gestione del file di log delle azioni di gioco
    /// </summary>
    public static class Logger
    {
        public static string FileLog = "log_partita.txt";

        ///<summary>
        /// Registra un messaggio nel file di log con timestamo corrente.
        /// </summary>
        public static void ScriviLog(string messaggio)
        {
            try
            {
                using(StreamWriter w = File.AppendText(FileLog))
                {
                    w.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {messaggio}");
                }
            }
            catch {/* Ignora errori di scrittura sileziosamente */}
        }
    }
}