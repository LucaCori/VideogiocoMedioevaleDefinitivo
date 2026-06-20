using System;
using System.Collections.Generic;

namespace Program
{
    //2.2 CLASSE STANZA
    public class Stanza
    {
        public string Nome {get; private set;}
        public string Descrizione{get; private set;}

        //REQUISITO: "mostrata in automatico alla prima visita"
        public bool GiaVisitata {get; set;}

        //Dizionario per gestire i collegamenti cardinali (N, S, W, E)
        //Mappa una stringa (direzione) alla stanza corrispondente
        private Dictionary<string, Stanza> Collegamenti {get; set;}

        //Requisito: "Ogni stanza può contenere qualsiasi numero di oggetti"
        public List<Oggetto> OggettiPresenti {get; private set;}

        //Unsa stanza può ospitare un personaggio alla volta (o null)
        public PersonaggioBase? PersonaggioPresente {get; set;}

        public Stanza(string nome, string descrizione)
        {
            Nome = nome;
            Descrizione = descrizione;
            GiaVisitata = false;
            Collegamenti = new Dictionary<string, Stanza>();
            OggettiPresenti = new List<Oggetto>();
            PersonaggioPresente = null;
        }

        //Metodo per collegare le stanze tra di loro
        public void ImpostaCollegamento(string direzione, Stanza stanzaDestinazione)
        {
            //Convertiamo in maiuscolo per evitare errori di battitura
            Collegamenti[direzione.ToUpper()] = stanzaDestinazione;
        }

        //Metodo per ottenere la stanza adiacente data una direzione
        public Stanza? OttieniStanzaAdiacente(string direzione)
        {
            string dirInMaiuscolo = direzione.ToUpper();
            if (Collegamenti.ContainsKey(dirInMaiuscolo))
            {
                return Collegamenti[dirInMaiuscolo];
            }
            return null; //Nessuna porta in quella direzione
        }
    }
}