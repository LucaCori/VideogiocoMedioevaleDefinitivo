using System;

namespace Program
{
    ///<summary>
    /// Rappresenta un alleato speciale che fornisce indizi sulle meccaniche di gioco
    /// </summary>
    public class PersonaggioGuida : Alleato
    {
        private string _consiglio;

        public PersonaggioGuida(string nome, string descrizione, string consiglio) : base (nome, descrizione)
        {
            _consiglio = consiglio;
        }

        public override void Interagisci(Giocatore giocatore)
        {
            //Esegue l'incontro base (Incontri Viandante... e log)
            Console.WriteLine($"\nIncontri {Nome}:");
            Logger.ScriviLog($"Incontro con {Nome}");

            Console.WriteLine($"\"{Descrizione}\"");
            Console.WriteLine($"[{Nome} ti sussurra]: '{_consiglio}'");

            //Impostiamo comunque a true l'interazione
            giocatore.HaInteragitoConBuono = true;
        }
    }
}
