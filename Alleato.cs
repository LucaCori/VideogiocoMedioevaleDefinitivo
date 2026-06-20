using System;

namespace Program
{
    /// <summary>
    /// Rappresenta un personaggio alleato (Buono)
    /// </summary>
    public class Alleato : PersonaggioBase
    {
        public Alleato(string nome, string descrizione) : base(nome, descrizione)
        {
            IsBuono = true;
        }

        ///<summary>
        /// Sovrascrittura del metodo di interazione per l'alleato.
        /// </summary>
        public override void Interagisci(Giocatore giocatore)
        {
            base.Interagisci(giocatore);
            Console.WriteLine($"\"{Descrizione}\"");
            Console.WriteLine($"[{Nome} ti sorride]: 'Se hai la Corona e siamo nel posto giusto, ti aiuterò a salvare il regno!'");
            giocatore.HaInteragitoConBuono = true;
            Logger.ScriviLog($"Giocatore ha interagito positivamente con l'alleato {Nome}");
        }
    }
}
