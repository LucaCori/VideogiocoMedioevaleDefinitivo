using System;

namespace Program
{
    // 1.2 CLASSE OGGETTO (Aggiornamento con il Peso)
    public class Oggetto
    {
        public string Nome {get; private set;}
        public string Descrizione {get; private set;}
        public bool IsPrendibile {get; private set;}
        //REQUISITO: "Ogni oggetto ha un peso"
        public int Peso {get; private set;}
        public Oggetto(string nome, string descrizione, bool isPrendibile, int peso)
        {
            Nome = nome;
            Descrizione = descrizione;
            IsPrendibile = isPrendibile;
            Peso = peso;
        }
    }
}