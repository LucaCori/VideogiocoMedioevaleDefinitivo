using System;
using System.Collections.Generic;

namespace Program
{
    // 3. CLASSE GIOCATORE (Aggiornata con LIFO e con Overloading)
    public class Giocatore
    {
        public string Nome {get; private set;}
        public Stanza StanzaCorrente {get; set;}
        //REQUISITO: "La gestione degli oggetti deve avvenire secondo una logico LIFO"
        //Usiamo Stac<T> invece di List<T>
        public Stack<Oggetto> InventarioPila {get; private set;}
        //REQUISITO: "Il giocatore può trasportare oggetti solo fino a un certo peso totale"
        public int PesoMassimo {get; private set;}
        //REQUISITO: "E' necessario interagire con il personaggio buono per risolvere il gioco"
        public bool HaInteragitoConBuono {get; set;}
        public Giocatore(string nome, Stanza stanzaIniziale, int pesoMassimo)
        {
            Nome = nome;
            StanzaCorrente = stanzaIniziale;
            InventarioPila = new Stack<Oggetto>();
            PesoMassimo = pesoMassimo;
            HaInteragitoConBuono = false;
        }

        //Metodo helper per calcolare il peso totale attuale nell'inventario
        public int OttieniPesoAttuale()
        {
            int pesoTotale = 0;
            foreach(var obj in InventarioPila)
            {
                pesoTotale += obj.Peso;
            }
            return pesoTotale;
        }

        /// <summary>
        /// Versione 1 del metodo Muovi: accetta una direzione cardinale (stringa).
        /// </summary>

        //Logica per muoversi in una direzione
        public void Muovi(string direzione)
        {
            Stanza? prossimaStanza = StanzaCorrente.OttieniStanzaAdiacente(direzione);

            // Trasformiamo la stringa in formato "Title Case" (es. NORD -> Nord) per un bell'output grafico
            string direzioneFormattata = char.ToUpper(direzione[0]) + direzione.Substring(1).ToLower();

            if(prossimaStanza != null)
            {
                StanzaCorrente = prossimaStanza;
                Console.WriteLine($"\nTi sposti verso {direzioneFormattata}.");
                Logger.ScriviLog($"Spostamento a {direzioneFormattata} in direzione della stanza: {prossimaStanza.Nome}");
            }
            else
            {
                Console.WriteLine($"\n[!] Non c'è nessuna porta in direzione {direzioneFormattata}");
            }
        }

        /// <summary>
        /// REQUISITO OVERLOADING: Versione 2 dello stesso metodo Muovi, ma accetta un oggett Stanza.
        /// </summary>
        public void Muovi(Stanza nuovaStanza)
        {
            StanzaCorrente = nuovaStanza;
            Logger.ScriviLog($"Teletrasporto forzato / Spostamento diretto nella stanza: {nuovaStanza.Nome}");
        }

        //Metodo per raccogliere l'oggetto in cima alla stanza e metterlo nel Pila (Push)
        public void RaccogliOggetto(string nomeOggetto)
        {
            //Cerchiamo l'oggetto nella stanza
            Oggetto? objDaRaccogliere = StanzaCorrente.OggettiPresenti.Find(o => o.Nome.Equals(nomeOggetto, StringComparison.OrdinalIgnoreCase));

            if(objDaRaccogliere == null)
            {
                Console.WriteLine("\n[!] Non c'è nessun '{nomeOggetto}' qui.");
                return;
            }

            if (!objDaRaccogliere.IsPrendibile)
            {
                //MODIFICA: Mostriamo prima la descrizione dell'oggetto non prendibile
                Console.WriteLine($"\nEsamini l'oggetto: {objDaRaccogliere.Descrizione}");

                Console.WriteLine($"\n[!] L'oggetto '{objDaRaccogliere.Nome}' non può essere raccolto!");
                return;
            }

            //Controllo del peso massimo
            if(OttieniPesoAttuale() + objDaRaccogliere.Peso > PesoMassimo)
            {
                Console.WriteLine($"\n[!] Inventario troppo pesante!");
                return;
            }

            //Se l'oggetto ha una descrizione specifica, la mostriamo alla prima interazione/raccolta
            Console.WriteLine($"\nEsamini l'oggetto: {objDaRaccogliere.Descrizione}");

            //Logica LIFO: Inseriamo in cima alla pila
            InventarioPila.Push(objDaRaccogliere);
            StanzaCorrente.OggettiPresenti.Remove(objDaRaccogliere);
            Console.WriteLine($"\nHai raccolto: {objDaRaccogliere.Nome}.");
            Logger.ScriviLog($"Raccolto oggetto: {objDaRaccogliere.Nome}");
        }

        //REQUISITO LIFO: "potrà prelevare e usare direttamente solo l'ultimo oggetto acquisito,
        // ma ovviamente potrà togliere momentaneamente oggetti fino a quello desiderato, usarlo e re-inserirli tutti"
        public void UsaOPosaOggetto(string nomeOggetto, bool posaNellaStanza)
        {
            if(InventarioPila.Count == 0)
            {
                Console.WriteLine("\n[!] Inventario vuoto!");
                return;
            }

            // REQUISITO LIFO RIGIDO: Controlliamo SUBITO se l'oggetto in cima è quello richiesto
            Oggetto oggettoInCima = InventarioPila.Peek();

            if(!oggettoInCima.Nome.Equals(nomeOggetto, StringComparison.OrdinalIgnoreCase))
            {
                // Se l'oggetto cercato non è in cima, stampiamo l'errore e blocchiamo l'azione
                Console.WriteLine($"\n[!] Errore LIFO: Non puoi interagire con '{nomeOggetto}'.");
                Console.WriteLine($"In cima al tuo inventario c'è '{oggettoInCima.Nome}'. Devi prima posare o usare quello!");
                return;
            }

            // Se arriviamo qui, l'oggetto richiesto è EFFETTIVAMENTE in cima alla pila!
            // Lo estraiamo ufficialmente (senza bisogno di cicli while complessi)
            Oggetto oggettoTrovato = InventarioPila.Pop();
            Stack<Oggetto> pilaTemporanea = new Stack<Oggetto>();

            if(oggettoTrovato != null)
            {
                //CASO 1: Il giocatore vuole POSARE l'oggetto
                if (posaNellaStanza)
                {
                    // Lo lasciamo nella stanza corrente ed esce definitivamente dall'inventario
                    StanzaCorrente.OggettiPresenti.Add(oggettoTrovato);
                    Console.WriteLine($"\nHai posato: {oggettoTrovato.Nome}.");
                    Logger.ScriviLog($"Posato oggetto: {oggettoTrovato.Nome} nella stanza {StanzaCorrente.Nome}");
                }
                //CASO 2: Il giocatore vuole USARE l'oggetto
                else
                {
                    //--- VINCOLO SPADA ---
                    if(oggettoTrovato.Nome.Equals("Spada", StringComparison.OrdinalIgnoreCase))
                    {
                        if(StanzaCorrente.PersonaggioPresente is Nemico)
                        {
                            Console.WriteLine($"\nHai utilizzato l'oggetto: {oggettoTrovato.Nome} per difenderti!");
                            Logger.ScriviLog($"Usato oggetto: {oggettoTrovato.Nome} contro il nemico.");
                            pilaTemporanea.Push(oggettoTrovato); // La spada si salva, torna in inventario
                        }
                        else
                        {
                            Console.WriteLine($"\n[!] Non puoi usare la SPADA qui. Non c'è nessun nemico da cui difendersi in questa stanza!");
                            pilaTemporanea.Push(oggettoTrovato); // Torna in inventario
                        }
                    }
                    //--- VINCOLO CORONA ---
                    else if(oggettoTrovato.Nome.Equals("Corona", StringComparison.OrdinalIgnoreCase))
                    {
                        if(StanzaCorrente.Nome.Equals("Sala del Trono", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"\nHai utilizzato l'oggetto: {oggettoTrovato.Nome}!");
                            Logger.ScriviLog($"Usato oggetto: {oggettoTrovato.Nome} nella Sala del Trono.");
                            StanzaCorrente.OggettiPresenti.Add(oggettoTrovato); // Viene consumata sul trono per attivare la vittoria nel Main
                        }
                        else
                        {
                            Console.WriteLine($"\n[!] Non puoi usare la CORONA qui! Questo oggetto può essere utilizzato solo ed esclusivamente all'interno della Sala del Trono.");
                            pilaTemporanea.Push(oggettoTrovato); // Torna in inventario
                        }
                    }
                    //--- VINCOLO TALISMANO ---
                    else if(oggettoTrovato.Nome.Equals("Talismano", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("\n[!] Stringi il Talismano... l'aria attorno a te inizia a fessurarsi di crepe violacee!");
                        Console.WriteLine("Vieni risucchiato in un vortice magico!");

                        // REINSERIMENTO ANTICIPATO: Il talismano deve tornare in cima prima del teletrasporto
                        pilaTemporanea.Push(oggettoTrovato);
                        while(pilaTemporanea.Count > 0)
                        {
                            InventarioPila.Push(pilaTemporanea.Pop());
                        }

                        // Resettiamo lo stato di tutti i nemici prima del salto magico
                        foreach (var stanza in Program.tutteLeStanze)
                        {
                            if (stanza.PersonaggioPresente is Nemico n) n.GiaAffrontato = false;
                        }

                        // Scegliamo una stanza casuale e viaggiamo
                        int indiceCasuale = Program.random.Next(Program.tutteLeStanze.Count);
                        Stanza stanzaDestinazione = Program.tutteLeStanze[indiceCasuale];

                        this.Muovi(stanzaDestinazione);
                        Console.WriteLine($"Ti risvegli confuso, materializzato in: {StanzaCorrente.Nome}!");

                        // CONTROLLO NEMICO IMMEDIATO
                        if(StanzaCorrente.PersonaggioPresente != null && StanzaCorrente.PersonaggioPresente is Nemico nemico)
                        {
                            nemico.Interagisci(this);
                            if (!nemico.GiaAffrontato)
                            {
                                if(InventarioPila.Count > 0)
                                {
                                    bool haSpadaInCima = InventarioPila.Peek().Nome.Equals("Spada", StringComparison.OrdinalIgnoreCase);
                                    bool attaccoAnnullato = false;

                                    if (haSpadaInCima)
                                    {
                                        Console.WriteLine($"[!] ATTENZIONE: Stringi la '{InventarioPila.Peek().Nome}' in cima al tuo inventario!");
                                        Console.WriteLine("Vuoi usarla per difenderti dal furto? (S/N): ");
                                        string? rispostaDifesa = Console.ReadLine();

                                        if(rispostaDifesa != null && rispostaDifesa.Trim().Equals("S", StringComparison.OrdinalIgnoreCase))
                                        {
                                            Console.WriteLine($"\n[Combattimento] Sguaini rapidamente la Spada! {nemico.Nome} si spaventa e indietreggia. Attacco annullato!");
                                            attaccoAnnullato = true;
                                        }
                                    }

                                    if (!attaccoAnnullato)
                                    {
                                        Oggetto oggettoRubato = InventarioPila.Pop();
                                        Console.WriteLine($"\n[!] {nemico.Nome} ti ha sorpreso alle spalle e ti ha rubato l'oggetto in cima all'inventario: {oggettoRubato.Nome}!");

                                        List<Stanza> stanzeDisponibiliPerFurto = Program.tutteLeStanze.FindAll(s => s != StanzaCorrente);
                                        Stanza stanzaDestinazioneOggetto = stanzeDisponibiliPerFurto[Program.random.Next(stanzeDisponibiliPerFurto.Count)];
                                        stanzaDestinazioneOggetto.OggettiPresenti.Add(oggettoRubato);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"\n[{nemico.Nome} ti fruga nelle tasche]: 'Non hai niente da rubare, straccione!'");
                                }
                                nemico.GiaAffrontato = true;
                            }
                        }
                        return; // Uscita sicura: la pila temporanea è già stata svuotata sopra
                    }
                    //--- ALTRI OGGETTI GENERALI (es: Boccale, Mestolo) ---
                    else
                    {
                        Console.WriteLine($"\nHai utilizzato l'oggetto: {oggettoTrovato.Nome}!");
                        Logger.ScriviLog($"Usato oggetto: {oggettoTrovato.Nome}");
                        pilaTemporanea.Push(oggettoTrovato); // Rimane nell'inventario
                    }
                }
            }

            // REQUISITO LIFO: Rimettiamo a posto gli oggetti se rimasti in pilaTemporanea (es: Spada o altri oggetti generali)
            while(pilaTemporanea.Count > 0)
            {
                InventarioPila.Push(pilaTemporanea.Pop());
            }
        }

        //Metodo per mostrare lo stato della pila dell'inventario
        public void MostraInventario()
        {
            Console.WriteLine($"\n--- INVENTARIO (Logica LIFO - Carico: {OttieniPesoAttuale()}/{PesoMassimo} kg) ---");
            if(InventarioPila.Count == 0)
            {
                Console.WriteLine("[Vuoto]");
                return;
            }
            
            foreach(var obj in InventarioPila)
            {
                Console.WriteLine($"- {obj.Nome} ({obj.Peso} kg)");
            }
        }
    }

}