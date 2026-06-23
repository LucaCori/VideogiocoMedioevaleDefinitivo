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
        /// REQUISITO OVERLOADING: Versione 2 dello stesso metodo Muovi, ma accetta un oggetto Stanza.
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

        /// <summary>
        /// Gestisce l'interazione con gli oggetti dell'inventario secondo una logica LIFO (Last-In, First-Out).
        /// Permette di posare l'oggetto nella stanza corrente o di usarlo attivando vincoli contestuali.
        /// </summary>
        public void UsaOPosaOggetto(string nomeOggetto, bool posaNellaStanza)
        {
            // 1. GESTIONE DELLE ECCEZIONI / EDGE CASES
            // Controllo difensivo: se l'inventario è vuoto, l'azione non è computabile.
            if(InventarioPila.Count == 0)
            {
                Console.WriteLine("\n[!] Inventario vuoto!");
                return; // Early return per evitare computazioni inutili o eccezioni a runtime
            }

            // 2. VERIFICA DEL REQUISITO LIFO (Last-In, First-Out)
            // Utilizziamo il metodo .Peek() che restituisce l'elemento in cima alla pila SENZA rimuoverlo.
            // Questo garantisce un accesso in tempo costante O(1) per la verifica del vincolo.
            Oggetto oggettoInCima = InventarioPila.Peek();

            // Confronto case-insensitive del nome dell'oggetto per migliorare la UX (User Experience)
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

            // Allocazione di una struttura di supporto (Pila Temporanea) per gestire la persistenza
            // degli oggetti non consumabili, preservandone l'ordine originale di inserimento.
            Stack<Oggetto> pilaTemporanea = new Stack<Oggetto>();

            if(oggettoTrovato != null)
            {
                //CASO 1: LOGICA DI RILASCIO (POSA NELLA STANZA)
                if (posaNellaStanza)
                {
                    // L'oggetto esce permanentemente dall'inventario e viene inserito 
                    // nella collezione degli oggetti della stanza corrente (Relazione di aggregazione)
                    StanzaCorrente.OggettiPresenti.Add(oggettoTrovato);
                    Console.WriteLine($"\nHai posato: {oggettoTrovato.Nome}.");
                    Logger.ScriviLog($"Posato oggetto: {oggettoTrovato.Nome} nella stanza {StanzaCorrente.Nome}");
                }
                //CASO 2: LOGICA DI UTILIZZO CONDIZIONALE (BUSINESS LOGIC DEL GIOCO)
                else
                {
                    //--- VINCOLO SPADA ---
                    if(oggettoTrovato.Nome.Equals("Spada", StringComparison.OrdinalIgnoreCase))
                    {
                        // Pattern Matching / Type Checking: verifichiamo polimorficamente se il personaggio è di tipo 'Nemico'
                        if(StanzaCorrente.PersonaggioPresente is Nemico)
                        {
                            Console.WriteLine($"\nHai utilizzato l'oggetto: {oggettoTrovato.Nome} per difenderti!");
                            Logger.ScriviLog($"Usato oggetto: {oggettoTrovato.Nome} contro il nemico.");
                            pilaTemporanea.Push(oggettoTrovato); // Oggetto non consumabile: viene parcheggiato per il reinserimento
                        }
                        else
                        {
                            Console.WriteLine($"\n[!] Non puoi usare la SPADA qui. Non c'è nessun nemico da cui difendersi in questa stanza!");
                            pilaTemporanea.Push(oggettoTrovato); // Rifiuto d'uso: l'oggetto torna comunque in inventario
                        }
                    }
                    //--- VINCOLO CORONA ---
                    else if(oggettoTrovato.Nome.Equals("Corona", StringComparison.OrdinalIgnoreCase))
                    {
                        // Controllo spaziale: l'oggetto Corona è vincolato a un ID/Nome di stanza specifico (Trigger di Vittoria)
                        if(StanzaCorrente.Nome.Equals("Sala del Trono", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"\nHai utilizzato l'oggetto: {oggettoTrovato.Nome}!");
                            Logger.ScriviLog($"Usato oggetto: {oggettoTrovato.Nome} nella Sala del Trono.");
                            StanzaCorrente.OggettiPresenti.Add(oggettoTrovato); // Oggetto CONSUMATO: si deposita nell'ambiente
                        }
                        else
                        {
                            Console.WriteLine($"\n[!] Non puoi usare la CORONA qui! Questo oggetto può essere utilizzato solo ed esclusivamente all'interno della Sala del Trono.");
                            pilaTemporanea.Push(oggettoTrovato); // Rifiuto d'uso: torna in inventario
                        }
                    }
                    //--- VINCOLO TALISMANO ---
                    else if(oggettoTrovato.Nome.Equals("Talismano", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("\n[!] Stringi il Talismano... l'aria attorno a te inizia a fessurarsi di crepe violacee!");
                        Console.WriteLine("Vieni risucchiato in un vortice magico!");

                        // GESTIONE DELLO STATO ANTECEDENTE AL TELETRASPORTO
                        // Il talismano deve tornare in inventario prima del salto per evitare incoerenze di stato
                        pilaTemporanea.Push(oggettoTrovato);
                        while(pilaTemporanea.Count > 0)
                        {
                            InventarioPila.Push(pilaTemporanea.Pop());
                        }

                        // Reset dello stato dei Nemici nel grafo globale delle stanze (Iterazione su collezione globale)
                        foreach (var stanza in Program.tutteLeStanze)
                        {
                            if (stanza.PersonaggioPresente is Nemico n) n.GiaAffrontato = false;
                        }

                        // Generazione di un indice pseudo-casuale per determinare la stanza di destinazione
                        int indiceCasuale = Program.random.Next(Program.tutteLeStanze.Count);
                        Stanza stanzaDestinazione = Program.tutteLeStanze[indiceCasuale];

                        // Invocazione dell'overloading del metodo Muovi(Stanza) per il teletrasporto diretto
                        this.Muovi(stanzaDestinazione);
                        Console.WriteLine($"Ti risvegli confuso, materializzato in: {StanzaCorrente.Nome}!");

                        // LOGICA DI AGGRESSIONE IMMEDIATA POST-TELETRASPORTO
                        if(StanzaCorrente.PersonaggioPresente != null && StanzaCorrente.PersonaggioPresente is Nemico nemico)
                        {
                            nemico.Interagisci(this); // Interazione polimorfica
                            if (!nemico.GiaAffrontato)
                            {
                                if(InventarioPila.Count > 0)
                                {
                                    // Controllo speculativo sulla nuova cima dell'inventario dopo il teletrasporto
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

                                    // RISOLUZIONE DEL FURTO (Se il giocatore non ha o non usa la spada in cima)
                                    if (!attaccoAnnullato)
                                    {
                                        Oggetto oggettoRubato = InventarioPila.Pop();
                                        Console.WriteLine($"\n[!] {nemico.Nome} ti ha sorpreso alle spalle e ti ha rubato l'oggetto in cima all'inventario: {oggettoRubato.Nome}!");

                                        // Algoritmo di redistribuzione dell'oggetto rubato in una stanza casuale del grafo (esclusa quella corrente)
                                        List<Stanza> stanzeDisponibiliPerFurto = Program.tutteLeStanze.FindAll(s => s != StanzaCorrente);
                                        Stanza stanzaDestinazioneOggetto = stanzeDisponibiliPerFurto[Program.random.Next(stanzeDisponibiliPerFurto.Count)];
                                        stanzaDestinazioneOggetto.OggettiPresenti.Add(oggettoRubato);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"\n[{nemico.Nome} ti fruga nelle tasche]: 'Non hai niente da rubare, straccione!'");
                                }
                                nemico.GiaAffrontato = true; // Mutazione dello stato del nemico per evitare loop di aggressione
                            }
                        }
                        return; // Uscita sicura: la pila temporanea è già stata svuotata sopra
                    }
                    //--- ALTRI OGGETTI GENERALI (es: Boccale, Mestolo) ---
                    else
                    {
                        Console.WriteLine($"\nHai utilizzato l'oggetto: {oggettoTrovato.Nome}!");
                        Logger.ScriviLog($"Usato oggetto: {oggettoTrovato.Nome}");
                        pilaTemporanea.Push(oggettoTrovato); // L'oggetto non si consuma, viene parcheggiato, rimane nell'inventario
                    }
                }
            }

            // 4. RIPRISTINO DELLO STATO DELL'INVENTARIO (Logica di Rollback)
            // Svuotiamo la pila temporanea riversando gli oggetti non consumati (es. Spada o Oggetti Generici) 
            // nuovamente dentro l'inventario principale. Essendo entrambe strutture Stack (LIFO), l'operazione
            // combinata di Pop e Push mantiene inalterato l'ordine originario dei restanti elementi.
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