using System;

namespace Program
{
    //2.1 GERARCHIA PERSONAGGI (Ereditarietà e Overriding) (NPC) - REQUISITO: "Ci sono altri personaggi..."
    public class PersonaggioBase
    {
        public string Nome {get; private set;}
        public string Descrizione {get; private set;}
        public bool IsBuono {get; protected set;} //true = buono, false = cattivo

        public PersonaggioBase(string nome, string descrizione)
        {
            Nome = nome;
            Descrizione = descrizione;
        }

        /// <summary>
        /// Metodo virtuale destinato ad essere sovrascritto nelle classi derivate (Overriding)
        /// </summary>

        public virtual void Interagisci(Giocatore giocatore)
        {
            Console.WriteLine($"\nIncontri {Nome}:");
            Logger.ScriviLog($"Incontro con {Nome}");
        }
    }
}